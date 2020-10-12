using Colorify;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Luna.Database
{
    class Mysql
    {
        private static MySqlConnectionStringBuilder connection()
        {
            DotNetEnv.Env.Load();
            MySqlConnectionStringBuilder conn = new MySqlConnectionStringBuilder();
            conn.Server = DotNetEnv.Env.GetString("DB_HOST");
            conn.Port = Convert.ToUInt32(DotNetEnv.Env.GetInt("DB_PORT"));
            conn.UserID = DotNetEnv.Env.GetString("DB_USER");
            conn.Password = DotNetEnv.Env.GetString("DB_PASS");
            conn.Database = DotNetEnv.Env.GetString("DB_NAME");
            return conn;
        }

        private static void Create(string table, string name)
        {
            #region Variables
            DotNetEnv.Env.Load();
            var path = Environment.CurrentDirectory + @"\app\";
            var entity = path + @"entities\";
            #endregion

            #region Create Entity
            if (!Directory.Exists(entity))
                Directory.CreateDirectory(entity);

            if (!File.Exists(entity + name + ".php"))
            {
                //Get Column Info
                var column = GetAllColumn(table);

                var entityFile = File.Create(entity + name + ".php");
                var entityWriter = new StreamWriter(entityFile);
                entityWriter.WriteLine(@"<?php");
                entityWriter.WriteLine(@"namespace App\Entities");
                entityWriter.WriteLine(@"{");
                entityWriter.WriteLine(@"    /**");
                entityWriter.WriteLine("     * @Entity");
                entityWriter.WriteLine("     * @Table(name=\"" + table + "\")");
                entityWriter.WriteLine(@"     */");
                entityWriter.WriteLine(@"    class " + name);
                entityWriter.WriteLine(@"    {");

                //Create protected variable
                foreach (var item in column)
                {
                    var type = AnnotationType(item.Type);
                    var key = (item.Key.ToLower() == "pri") ? " @Id" : "";
                    var fk = (item.Key.ToLower() == "mul") ? true : false;
                    var extra = (item.Extra.ToLower() == "auto_increment") ? " @GeneratedValue" : "";

                    entityWriter.WriteLine(@"        /**");
                    if (!fk)
                    {
                        entityWriter.WriteLine("         *" + key + " @Column(type=\"" + type + "\")" + extra);
                    }
                    else
                    {
                        var forekey = GetForekeyColumn(item.Field);
                        foreach (var item2 in forekey)
                        {
                            entityWriter.WriteLine("         * @ManyToOne(targetEntity=\"" + item2.referenced_table_name + "\")");
                            entityWriter.WriteLine("         * @JoinColumn(name=\"" + item.Field + "\", referencedColumnName=\"" + item2.referenced_column_name + "\")");
                        }
                    }
                    entityWriter.WriteLine(@"         */");
                    entityWriter.WriteLine(@"        protected $" + item.Field + ";");
                    entityWriter.WriteLine(@"");
                }

                //Create Get and Set
                foreach (var item in column)
                {
                    var isDate = "";
                    var key = (item.Key.ToLower() == "pri") ? true : false;
                    var field = item.Field.ToLower();
                    field = char.ToUpper(field[0]) + field.Substring(1);

                    if (item.Type.Contains("date") || item.Type.Contains("datetime"))
                    {
                        if(item.Type == "date")
                        {
                            isDate = "Date ";
                        }
                        else
                        {
                            isDate = "DateTime ";
                        }
                    }

                    if (!key)
                    {
                        entityWriter.WriteLine(@"        public function get" + field + "()");
                        entityWriter.WriteLine(@"        {");
                        entityWriter.WriteLine(@"            return $this->" + item.Field + ";");
                        entityWriter.WriteLine(@"        }");
                        entityWriter.WriteLine(@"");
                        entityWriter.WriteLine(@"        public function set" + field + "("+ isDate + "$" + item.Field + ")");
                        entityWriter.WriteLine(@"        {");
                        entityWriter.WriteLine(@"            $this->" + item.Field + " = $" + item.Field + ";");
                        entityWriter.WriteLine(@"        }");
                        entityWriter.WriteLine(@"");
                    }
                    else
                    {
                        entityWriter.WriteLine(@"        public function getId()");
                        entityWriter.WriteLine(@"        {");
                        entityWriter.WriteLine(@"            return $this->" + item.Field + ";");
                        entityWriter.WriteLine(@"        }");
                        entityWriter.WriteLine(@"");
                    }
                }

                entityWriter.WriteLine(@"    }");
                entityWriter.WriteLine(@"}");

                entityWriter.Dispose();
                Program._colorify.WriteLine("Entity successfully created!", Colors.bgSuccess);
                Program._colorify.WriteLine(entity + name + ".php", Colors.bgMuted);
            }
            else
            {
                Program._colorify.WriteLine(name + ".php already exists and so was not generated.", Colors.bgDanger);
                Program._colorify.WriteLine(entity + name + ".php", Colors.bgMuted);
            }
            #endregion
        }

        public static void Generator(string name)
        {
            try
            {
                
                if (name != "*")
                {
                    Create(name, Function.Util.FormatName(name));
                }
                else
                {
                    IEnumerable<Table> list = new List<Table>(GetAllTable());
                    foreach (var item in list)
                    {
                        Create(item.Name, Function.Util.FormatName(item.Name));
                    }
                }
            }
            catch (Exception)
            {
                Program._colorify.WriteLine("Table '" + name.ToLower() + "' does not exist.", Colors.bgDanger);
            }
            
        }

        public static bool ExistTable(string name)
        {
            try
            {
                GetAllColumn(name);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static IEnumerable<string> FieldTable(string name)
        {
            try
            {
                var columns = GetAllColumn(name);
                List<string> list = new List<string>();
                foreach (var item in columns)
                {
                    if(item.Key.ToLower() != "pri")
                    {
                        if (item.Type.Contains("date") || item.Type.Contains("datetime"))
                        {
                            list.Add(item.Field.ToLower() + "[" + item.Type.ToUpper() + "]");
                        }
                        else
                        {
                            list.Add(item.Field.ToLower());
                        }
                    }
                    else
                    {
                        list.Add("id");
                    }
                }
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string AnnotationType(string type)
        {
            if (type.ToUpper() == "TINYINT(1)")
                type = "BOOLEAN";

            if (type.Contains("(") && type.Contains(")"))
            {
                int position = type.IndexOf("(");
                type = type.Substring(0, position);
            }

            switch (type.ToUpper())
            {
                //Number
                case "TINYINT":
                case "SMALLINT":
                    return "smallint";
                case "INT":
                case "MEDIUMINT":
                    return "integer";
                case "BIGINT":
                    return "bigint";
                case "DECIMAL":
                    return "decimal";
                case "FLOAT":
                case "DOUBLE":
                    return "float";
                //Text
                case "CHAR":
                case "VARCHAR":
                    return "string";
                case "TINYTEXT":
                case "TEXT":
                case "MEDIUMTEXT":
                case "LONGTEXT":
                    return "text";
                //Blob
                case "TINYBLOB":
                case "BLOB":
                case "MEDIUMBLOB":
                case "LONGBLOB":
                    return "blob";
                //Boolean
                case "BOOL":
                case "BOOLEAN":
                    return "boolean";
                //Date Time
                case "DATE":
                    return "date";
                case "TIME":
                    return "time";
                case "DATETIME":
                    return "datetime";
                case "TIMESTAMP":
                    return "datetimetz";
                default:
                    return "string";
            }
        }

        private static void ShowInfo()
        {
            string SqlTable = "SHOW TABLES";

            using (var db = new MySqlConnection(connection().ToString()))
            {
                var table = db.Query<string>(SqlTable);
                foreach (var itemT in table)
                {
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine("Table: " + itemT);
                    Console.WriteLine("--------------------------------");

                    string SqlColumn = String.Format("SHOW COLUMNS FROM {0}", itemT);

                    var column = db.Query<Column>(SqlColumn);
                    foreach (var itemC in column)
                    {
                        Console.WriteLine("Field:" + itemC.Field
                                        + " | Type: " + itemC.Type
                                        + " | Null: " + itemC.Null
                                        + " | Key: " + itemC.Key
                                        + " | Default:" + itemC.Default
                                        + " | Extra:" + itemC.Extra);
                    }

                    string SqlFK = String.Format("SELECT table_name, column_name, constraint_name, referenced_table_name, referenced_column_name" +
                                                " FROM information_schema.key_column_usage" +
                                                " WHERE referenced_table_schema = '{0}' AND referenced_table_name = '{1}'", DotNetEnv.Env.GetString("DB_NAME"), itemT);

                    var foreign_key = db.Query<FkTable>(SqlFK);
                    foreach (var itemF in foreign_key)
                    {
                        Console.WriteLine("TableName:" + itemF.table_name
                                        + " | ColumnName: " + itemF.column_name
                                        + " | ConstraintName: " + itemF.constraint_name
                                        + " | ReferencedTableName: " + itemF.referenced_table_name
                                        + " | ReferencedColumnName:" + itemF.referenced_column_name);
                    }
                }



                db.Dispose();
            }

        }

        private static IEnumerable<Table> GetAllTable()
        {
            string sql = "SHOW TABLES";
            using (var db = new MySqlConnection(connection().ToString()))
            {
                var ret = db.Query<string>(sql);
                List<Table> list = new List<Table>();
                foreach (var item in ret)
                {
                    Table table = new Table();
                    table.Name = item;
                    list.Add(table);
                }
                db.Dispose();
                return list;
            }
        }

        private static IEnumerable<Column> GetAllColumn(string table)
        {
            string sql = String.Format("SHOW COLUMNS FROM {0}", table);
            using (var db = new MySqlConnection(connection().ToString()))
            {
                var list = db.Query<Column>(sql);
                db.Dispose();
                return list;
            }
        }

        private static IEnumerable<FkTable> GetForekeyTable(string table)
        {
            string sql = String.Format("SELECT table_name, column_name, constraint_name, referenced_table_name, referenced_column_name" +
                                        " FROM information_schema.key_column_usage" +
                                        " WHERE referenced_table_schema = '{0}' AND referenced_table_name = '{1}'", DotNetEnv.Env.GetString("DB_NAME"), table);

            using (var db = new MySqlConnection(connection().ToString()))
            {
                var list = db.Query<FkTable>(sql);
                db.Dispose();
                return list;
            }
        }

        private static IEnumerable<FkColumn> GetForekeyColumn(string column)
        {
            string sql = String.Format("SELECT referenced_table_name, referenced_column_name" +
                                        " FROM information_schema.key_column_usage" +
                                        " WHERE referenced_table_schema = '{0}' AND referenced_column_name = '{1}' LIMIT 1", DotNetEnv.Env.GetString("DB_NAME"), column);

            using (var db = new MySqlConnection(connection().ToString()))
            {
                var list = db.Query<FkColumn>(sql);
                db.Dispose();
                return list;
            }
        }

        private class FkTable
        {
            public string table_name { get; set; }
            public string column_name { get; set; }
            public string constraint_name { get; set; }
            public string referenced_table_name { get; set; }
            public string referenced_column_name { get; set; }
        }

        private class FkColumn
        {
            public string referenced_table_name { get; set; }
            public string referenced_column_name { get; set; }
        }
        private class Table
        {
            public string Name { get; set; }
        }

        private class Column
        {
            public string Field { get; set; }
            public string Type { get; set; }
            public string Null { get; set; }
            public string Key { get; set; }
            public string Default { get; set; }
            public string Extra { get; set; }
        }

    }
}
