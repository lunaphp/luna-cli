using Colorify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Luna.Process
{
    class Create
    {
        public static void Page(string name, bool crud = false, IEnumerable<string> columns = null)
        {
            #region Variables
            DotNetEnv.Env.Load();

            var path = Environment.CurrentDirectory + @"\app\";
            var controllers = path + @"controllers\";
            var models = path + @"models\";
            var views = path + @"views\" + name.ToLower() + @"\";
            var route = path + @"config\route.php";
            var url = "http://" + (DotNetEnv.Env.GetString("APP_URL_BASE").Replace("http://", "").Replace("https://", "") + DotNetEnv.Env.GetString("APP_URL_PATH")).Replace("//", "/");

            if (Regex.IsMatch(name.Replace("_", ""), @"[^a-zA-Z0-9]"))
            {
                Program._colorify.WriteLine("Only letters, numbers and underline are allowed in the name!", Colors.bgDanger);
                return;
            }
            #endregion

            #region Create Controller
            if (!Directory.Exists(controllers))
                Directory.CreateDirectory(controllers);

            if (!File.Exists(controllers + Function.Util.FormatName(name) + "Controller.php"))
            {
                var controllerFile = File.Create(controllers + Function.Util.FormatName(name) + "Controller.php");
                var controllerWriter = new StreamWriter(controllerFile);
                controllerWriter.WriteLine(@"<?php");
                controllerWriter.WriteLine(@"namespace App\Controllers");
                controllerWriter.WriteLine(@"{");
                controllerWriter.WriteLine(@"    use App\Models\" + Function.Util.FormatName(name) + "Model;");
                controllerWriter.WriteLine(@"    use System\Core\View;");
                controllerWriter.WriteLine(@"    use System\Core\Controller;");
                controllerWriter.WriteLine(@"");
                controllerWriter.WriteLine(@"    class " + Function.Util.FormatName(name) + "Controller extends Controller");
                controllerWriter.WriteLine(@"    {");
                controllerWriter.WriteLine(@"        public function __construct()");
                controllerWriter.WriteLine(@"        {");
                controllerWriter.WriteLine(@"            parent::__construct();");
                controllerWriter.WriteLine(@"        }");
                if (crud)
                {
                    controllerWriter.WriteLine(@"");
                    controllerWriter.WriteLine(@"        public function Index()");
                    controllerWriter.WriteLine(@"        {");
                    controllerWriter.WriteLine(@"            View::render('" + name.ToLower() + "/index', true, array('" + name.ToLower() + "' => " + Function.Util.FormatName(name) + "Model::list" + Function.Util.FormatName(name) + "()));");
                    controllerWriter.WriteLine(@"        }");
                    controllerWriter.WriteLine(@"");
                    controllerWriter.WriteLine(@"        public function Create()");
                    controllerWriter.WriteLine(@"        {");
                    controllerWriter.WriteLine(@"            View::render('" + name.ToLower() + "/add', true, array('" + name.ToLower() + "' => " + Function.Util.FormatName(name) + "Model::insert" + Function.Util.FormatName(name) + "()));");
                    controllerWriter.WriteLine(@"        }");
                    controllerWriter.WriteLine(@"");
                    controllerWriter.WriteLine(@"        public function Edit()");
                    controllerWriter.WriteLine(@"        {");
                    controllerWriter.WriteLine(@"            View::render('" + name.ToLower() + "/edit', true, array('" + name.ToLower() + "' => " + Function.Util.FormatName(name) + "Model::update" + Function.Util.FormatName(name) + "()));");
                    controllerWriter.WriteLine(@"        }");
                    controllerWriter.WriteLine(@"");
                    controllerWriter.WriteLine(@"        public function Delete()");
                    controllerWriter.WriteLine(@"        {");
                    controllerWriter.WriteLine(@"            View::render('" + name.ToLower() + "/delete', true, array('" + name.ToLower() + "' => " + Function.Util.FormatName(name) + "Model::delete" + Function.Util.FormatName(name) + "()));");
                    controllerWriter.WriteLine(@"        }");
                }
                else
                {
                    controllerWriter.WriteLine(@"");
                    controllerWriter.WriteLine(@"        public function Index()");
                    controllerWriter.WriteLine(@"        {");
                    controllerWriter.WriteLine(@"            View::render('" + name.ToLower() + "/index', true);");
                    controllerWriter.WriteLine(@"        }");
                }
                controllerWriter.WriteLine(@"    }");
                controllerWriter.WriteLine(@"}");
                controllerWriter.Dispose();
                Program._colorify.WriteLine("Controller successfully created!", Colors.bgSuccess);
                Program._colorify.WriteLine(controllers + Function.Util.FormatName(name) + "Controller.php", Colors.bgMuted);
            }
            else
            {
                Program._colorify.WriteLine(Function.Util.FormatName(name) + "Controller.php already exists and so was not generated.", Colors.bgDanger);
                Program._colorify.WriteLine(controllers + Function.Util.FormatName(name) + "Controller.php", Colors.bgMuted);
            }
            #endregion

            #region Create Model
            if (!Directory.Exists(models))
                Directory.CreateDirectory(models);

            if (!File.Exists(models + Function.Util.FormatName(name) + "Model.php"))
            {
                var modelFile = File.Create(models + Function.Util.FormatName(name) + "Model.php");
                var modelWriter = new StreamWriter(modelFile);
                modelWriter.WriteLine(@"<?php");
                modelWriter.WriteLine(@"namespace App\Models");
                modelWriter.WriteLine(@"{");
                modelWriter.WriteLine(@"    use System\Core\Model;");
                modelWriter.WriteLine(@"    class " + Function.Util.FormatName(name) + "Model extends Model");
                modelWriter.WriteLine(@"    {");
                modelWriter.WriteLine(@"        public function __construct()");
                modelWriter.WriteLine(@"        {");
                modelWriter.WriteLine(@"            parent::__construct();");
                modelWriter.WriteLine(@"        }");
                if (crud)
                {
                    string varName = name.Replace("_", "").ToLower();
                    modelWriter.WriteLine(@"");
                    modelWriter.WriteLine(@"        public static function list" + Function.Util.FormatName(name) + "()");
                    modelWriter.WriteLine(@"        {");
                    modelWriter.WriteLine(@"            require __DIR__.'/../config/bootstrap.php';");
                    modelWriter.WriteLine(@"            if(isset($entityManager)){");
                    modelWriter.WriteLine(@"                $" + varName + @"Repository = $entityManager->getRepository('\App\Entities\" + Function.Util.FormatName(name) + "');");
                    modelWriter.WriteLine(@"                $" + varName + " = $" + varName + "Repository->findAll();");
                    modelWriter.WriteLine(@"                return $" + varName + ";");
                    modelWriter.WriteLine(@"            }else{");
                    modelWriter.WriteLine(@"                return null;");
                    modelWriter.WriteLine(@"            }");
                    modelWriter.WriteLine(@"        }");
                    modelWriter.WriteLine(@"");
                    modelWriter.WriteLine(@"        public static function insert" + Function.Util.FormatName(name) + "()");
                    modelWriter.WriteLine(@"        {");
                    modelWriter.WriteLine(@"            require __DIR__.'/../config/bootstrap.php';");
                    modelWriter.WriteLine(@"        }");
                    modelWriter.WriteLine(@"");
                    modelWriter.WriteLine(@"        public static function update" + Function.Util.FormatName(name) + "()");
                    modelWriter.WriteLine(@"        {");
                    modelWriter.WriteLine(@"            require __DIR__.'/../config/bootstrap.php';");
                    modelWriter.WriteLine(@"        }");
                    modelWriter.WriteLine(@"");
                    modelWriter.WriteLine(@"        public static function delete" + Function.Util.FormatName(name) + "()");
                    modelWriter.WriteLine(@"        {");
                    modelWriter.WriteLine(@"            require __DIR__.'/../config/bootstrap.php';");
                    modelWriter.WriteLine(@"        }");
                }
                modelWriter.WriteLine(@"    }");
                modelWriter.WriteLine(@"}");
                modelWriter.Dispose();
                Program._colorify.WriteLine("Model successfully created!", Colors.bgSuccess);
                Program._colorify.WriteLine(models + Function.Util.FormatName(name) + "Model.php", Colors.bgMuted);
            }
            else
            {
                Program._colorify.WriteLine(Function.Util.FormatName(name) + "Model.php already exists and so was not generated.", Colors.bgDanger);
                Program._colorify.WriteLine(models + Function.Util.FormatName(name) + "Model.php", Colors.bgMuted);
            }
            #endregion

            #region Create View
            if (!Directory.Exists(views))
                Directory.CreateDirectory(views);

            if (crud)
            {
                if (!File.Exists(views + "index.html"))
                {
                    var viewFile = File.Create(views + "index.html");
                    var viewWriter = new StreamWriter(viewFile);
                    viewWriter.WriteLine("{% extends \"templates/default/base.html\" %}");
                    viewWriter.WriteLine("{% block title %}List - " + Function.Util.FormatName(name.Replace("_", "#")) + "{% endblock %}");
                    viewWriter.WriteLine("{% block body %}");
                    viewWriter.WriteLine("<h1><b>" + Function.Util.FormatName(name.Replace("_", "#")) + " - List</b></h1>");
                    viewWriter.WriteLine("{% for item in " + name.ToLower() + " %}");
                    foreach (var item in columns)
                    {
                        var column = item.Replace("[DATE]", "|date(\"" + DotNetEnv.Env.GetString("APP_DATE") + "\")");
                        column = column.Replace("[DATETIME]", "|date(\"" + DotNetEnv.Env.GetString("APP_DATETIME") + "\")");

                        viewWriter.WriteLine("{{ item." + column + " }}");
                    }
                    viewWriter.WriteLine("<br>");
                    viewWriter.WriteLine("{% else %}");
                    viewWriter.WriteLine("No " + Function.Util.FormatName(name.Replace("_", "#")).ToLower() + " have been found.");
                    viewWriter.WriteLine("{% endfor %}");
                    viewWriter.WriteLine("{% endblock %}");
                    viewWriter.Dispose();
                    Program._colorify.WriteLine("View successfully created!", Colors.bgSuccess);
                    Program._colorify.WriteLine(views + "index.html", Colors.bgMuted);
                }
                else
                {
                    Program._colorify.WriteLine(Function.Util.FormatName(name) + @"/index.html already exists and so was not generated.", Colors.bgDanger);
                    Program._colorify.WriteLine(views + "index.html", Colors.bgMuted);
                }

                if (!File.Exists(views + "add.html"))
                {
                    var viewFile = File.Create(views + "add.html");
                    var viewWriter = new StreamWriter(viewFile);
                    viewWriter.WriteLine("{% extends \"templates/default/base.html\" %}");
                    viewWriter.WriteLine("{% block title %}Add - " + Function.Util.FormatName(name.Replace("_", "#")) + "{% endblock %}");
                    viewWriter.WriteLine("{% block body %}");
                    viewWriter.WriteLine("<h1><b>" + Function.Util.FormatName(name.Replace("_", "#")) + " - Add</b></h1>");
                    viewWriter.WriteLine("{% endblock %}");
                    viewWriter.Dispose();
                    Program._colorify.WriteLine("View successfully created!", Colors.bgSuccess);
                    Program._colorify.WriteLine(views + "add.html", Colors.bgMuted);
                }
                else
                {
                    Program._colorify.WriteLine(Function.Util.FormatName(name) + @"/add.html already exists and so was not generated.", Colors.bgDanger);
                    Program._colorify.WriteLine(views + "add.html", Colors.bgMuted);
                }

                if (!File.Exists(views + "edit.html"))
                {
                    var viewFile = File.Create(views + "edit.html");
                    var viewWriter = new StreamWriter(viewFile);
                    viewWriter.WriteLine("{% extends \"templates/default/base.html\" %}");
                    viewWriter.WriteLine("{% block title %}Edit - " + Function.Util.FormatName(name.Replace("_", "#")) + "{% endblock %}");
                    viewWriter.WriteLine("{% block body %}");
                    viewWriter.WriteLine("<h1><b>" + Function.Util.FormatName(name.Replace("_", "#")) + " - Edit</b></h1>");
                    viewWriter.WriteLine("{% endblock %}");
                    viewWriter.Dispose();
                    Program._colorify.WriteLine("View successfully created!", Colors.bgSuccess);
                    Program._colorify.WriteLine(views + "edit.html", Colors.bgMuted);
                }
                else
                {
                    Program._colorify.WriteLine(Function.Util.FormatName(name) + @"/edit.html already exists and so was not generated.", Colors.bgDanger);
                    Program._colorify.WriteLine(views + "edit.html", Colors.bgMuted);
                }

                if (!File.Exists(views + "delete.html"))
                {
                    var viewFile = File.Create(views + "delete.html");
                    var viewWriter = new StreamWriter(viewFile);
                    viewWriter.WriteLine("{% extends \"templates/default/base.html\" %}");
                    viewWriter.WriteLine("{% block title %}Delete - " + Function.Util.FormatName(name.Replace("_", "#")) + "{% endblock %}");
                    viewWriter.WriteLine("{% block body %}");
                    viewWriter.WriteLine("<h1><b>" + Function.Util.FormatName(name.Replace("_", "#")) + " - Delete</b></h1>");
                    viewWriter.WriteLine("{% endblock %}");
                    viewWriter.Dispose();
                    Program._colorify.WriteLine("View successfully created!", Colors.bgSuccess);
                    Program._colorify.WriteLine(views + "delete.html", Colors.bgMuted);
                }
                else
                {
                    Program._colorify.WriteLine(Function.Util.FormatName(name) + @"/delete.html already exists and so was not generated.", Colors.bgDanger);
                    Program._colorify.WriteLine(views + "delete.html", Colors.bgMuted);
                }
            }
            else
            {
                if (!File.Exists(views + "index.html"))
                {
                    var viewFile = File.Create(views + "index.html");
                    var viewWriter = new StreamWriter(viewFile);
                    viewWriter.WriteLine("{% extends \"templates/default/base.html\" %}");
                    viewWriter.WriteLine("{% block title %}" + Function.Util.FormatName(name.Replace("_", "#")) + "{% endblock %}");
                    viewWriter.WriteLine("{% block body %}");
                    viewWriter.WriteLine("<h1>Welcome Page <b>" + Function.Util.FormatName(name.Replace("_", "#")) + "</b></h1>");
                    viewWriter.WriteLine("{% endblock %}");
                    viewWriter.Dispose();
                    Program._colorify.WriteLine("View successfully created!", Colors.bgSuccess);
                    Program._colorify.WriteLine(views + "index.html", Colors.bgMuted);
                }
                else
                {
                    Program._colorify.WriteLine(Function.Util.FormatName(name) + @"/index.html already exists and so was not generated.", Colors.bgDanger);
                    Program._colorify.WriteLine(views + "index.html", Colors.bgMuted);
                }
            }
            #endregion

            #region Create Route
            if (crud)
            {
                RouteCreate(name, Function.Util.FormatName(name), "Index", "GET", path);
                RouteCreate(name + "/add", Function.Util.FormatName(name), "Create", "GET", path);
                RouteCreate(name + "/edit", Function.Util.FormatName(name), "Edit", "GET", path);
                RouteCreate(name + "/delete", Function.Util.FormatName(name), "Delete", "GET", path);
            }
            else
            {
                RouteCreate(name, Function.Util.FormatName(name), "Index", "GET", path);
            }
            #endregion
        }

        public static void Entity(string name)
        {
            DotNetEnv.Env.Load();
            switch (DotNetEnv.Env.GetString("DB_DRIVER"))
            {
                case null:
                case "":
                    Program._colorify.WriteLine("Please, check the .env file to see if the database variables are commented out with # or are incorrect.", Colors.bgDanger);
                    break;
                case "mysql":
                case "pdo_mysql":
                    try
                    {
                        Database.Mysql.Generator(name);
                    }
                    catch (Exception)
                    {
                        Program._colorify.WriteLine("The connection to MySql failed, make sure the MySql service has started.", Colors.bgDanger);
                    }
                    break;
                default:
                    Program._colorify.WriteLine("The selected database \"" + DotNetEnv.Env.GetString("DB_DRIVER") + "\" is not yet supported for automatic generations, only Mysql.", Colors.bgDanger);
                    break;
            }
        }

        public static void Crud(string name)
        {
            if (Database.Mysql.ExistTable(name))
            {
                var columns = Database.Mysql.FieldTable(name);
                Entity(name);
                Page(name, true, columns);
                Program._colorify.WriteLine("Crud " + name.ToLower() + " successfully created!", Colors.bgSuccess);
            }
            else
            {
                Program._colorify.WriteLine("Table '" + name.ToLower() + "' does not exist.", Colors.bgDanger);
            }
        }

        public static void Route(string name, string controller, string function, string method)
        {
            #region Variables
            DotNetEnv.Env.Load();
            var path = Environment.CurrentDirectory + @"\app\";
            controller = controller.Replace("Controller", "");
            var controllerFile = path + @"controllers\" + Function.Util.FormatName(controller) + "Controller.php";
            #endregion

            #region Generate Route
            if (!File.Exists(controllerFile))
            {
                Program._colorify.WriteLine("The controller is invalid or does not exist", Colors.bgDanger);
                return;
            }

            if (function == "*")
            {
                var Func = new List<string>();
                var controllerFunc = File.ReadAllLines(controllerFile);
                foreach (var item in controllerFunc)
                {
                    var line = item.Trim();
                    if (line.Contains("public function"))
                    {
                        if (!line.Contains("construct"))
                        {
                            Func.Add(line.Replace("public", "").Replace("function", "").Replace("()", "").Trim());
                        }
                    }
                }

                foreach (var item in Func)
                {
                    if (item != "Index")
                    {
                        RouteCreate(name + "/" + item.ToLower(), controller, item.ToLower(), method, path);
                    }
                    else
                    {
                        RouteCreate(name, controller, item.ToLower(), method, path);
                        RouteCreate(name + "/", controller, item.ToLower(), method, path);
                    }
                }
                return;
            }
            else
            {
                RouteCreate(name, controller, function, method, path);
                return;
            }
            #endregion
        }

        private static void RouteCreate(string name, string controller, string function, string method, string path)
        {
            #region Variables
            var route = path + @"config\route.php";
            var routeFile = File.ReadAllLines(route);
            var routeCount = 1;
            var routePos = 0;
            var routeExist = false;
            var url = "http://" + (DotNetEnv.Env.GetString("APP_URL_BASE").Replace("http://", "").Replace("https://", "") + DotNetEnv.Env.GetString("APP_URL_PATH")).Replace("//", "/");
            #endregion

            #region Create Route
            switch (method.ToUpper())
            {
                case "GET":
                    method = "'get'";
                    break;
                case "POST":
                    method = "'post'";
                    break;
                case "*":
                    method = "['get', 'post']";
                    break;
                default:
                    method = "'get'";
                    break;
            }

            foreach (var item in routeFile)
            {
                if (item.Trim() == "public static function add()")
                {
                    routePos = routeCount;
                }
                if (item.Trim() == "Router::add(getenv('APP_URL_PATH').'" + name.ToLower() + "',function() {")
                {
                    routeExist = true;
                }
                routeCount++;
            }

            if (routeFile[routePos].Trim() == "{")
            {
                if (!routeExist)
                {
                    var line = Convert.ToInt32(routePos);
                    Function.Util.InsertLine(route, line + 1, "            //Route \"" + name.ToUpper() + "\" created automatically by LunaCLI");
                    Function.Util.InsertLine(route, line + 2, "            Router::add(getenv('APP_URL_PATH').'/" + name.ToLower() + "',function() {");
                    Function.Util.InsertLine(route, line + 3, "                Page::render('" + Function.Util.FormatName(controller) + "')->" + Function.Util.FormatName(function) + "();");
                    Function.Util.InsertLine(route, line + 4, "            }, " + method + ");");
                    Function.Util.InsertLine(route, line + 5, "");

                    Program._colorify.WriteLine("Route successfully created!", Colors.bgSuccess);
                    Program._colorify.WriteLine(url + name.ToLower(), Colors.bgMuted);
                    return;
                }
                else
                {
                    Program._colorify.WriteLine("Route \"" + url + name.ToLower() + "\" already exists and so was not generated.", Colors.bgDanger);
                }
            }
            else
            {
                Program._colorify.WriteLine("Could not create route automatically. The route.php file is not default.", Colors.bgDanger);
            }
            #endregion
        }
    }
}
