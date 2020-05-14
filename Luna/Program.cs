using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CommandLine;
using Colorify;
using Colorify.UI;
using ToolBox.Platform;
using System.IO;

namespace Luna
{
    class Program
    {
        public static Format _colorify { get; set; }

        static void Main(string[] args)
        {
            switch (OS.GetCurrent())
            {
                case "win":
                case "gnu":
                    _colorify = new Format(Theme.Dark);
                    break;
                case "mac":
                    _colorify = new Format(Theme.Light);
                    break;
            }

            Parser.Default.ParseArguments<Options>(args)
              .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts))
              .WithNotParsed<Options>((errs) => HandleParseError(errs));

            var arg = args.FirstOrDefault<string>();
            switch (arg)
            {
                case "-n":
                case "--new":
                    if (args.Count() >= 2)
                    {
                        if (args.Count() >= 3)
                        {
                            switch (args[1].ToString())
                            {
                                case "project":
                                    Process.GitHub.Download("lunaphp", "framework", args[2].ToString());
                                    break;
                                case "template":
                                    if (Function.Validation.ExistDirectory())
                                    {
                                        _colorify.WriteLine("Tempalte \"" + args[2].ToString() + "\" successfully created!", Colors.bgSuccess);
                                    }
                                    else
                                    {
                                        Function.Alert.ExistDirectory();
                                    }
                                    break;
                                case "api":
                                    if (Function.Validation.ExistDirectory())
                                    {
                                        _colorify.WriteLine("Api \"" + args[2].ToString() + "\" successfully created!", Colors.bgSuccess);
                                    }
                                    else
                                    {
                                        Function.Alert.ExistDirectory();
                                    }
                                    break;
                                default:
                                    _colorify.WriteLine("Command \"" + args[1].ToString() + "\" is not valid, use one of these commands: project, template or api.", Colors.bgDanger);
                                    _colorify.ResetColor();
                                    break;
                            }
                        }
                        else
                        {
                            _colorify.WriteLine("\nCreating a new project or derivations with null or empty name is not allowed.", Colors.bgDanger);
                            _colorify.ResetColor();
                        }
                    }
                    break;
                case "-c":
                case "--create":
                    if (args.Count() >= 2)
                    {
                        if (args.Count() >= 3)
                        {
                            if (Function.Validation.ExistDirectory())
                            {
                                if (!File.Exists(".env")) {
                                    Function.Alert.ExistFileEnv();
                                    break;
                                }

                                switch (args[1].ToString())
                                {
                                    case "page":
                                        Process.Create.Page(args[2].ToString());
                                        break;
                                    case "crud":
                                        Process.Create.Crud(args[2].ToString());
                                        break;
                                    case "entity":
                                        Process.Create.Entity(args[2].ToString());
                                        break;
                                    case "route":
                                        if (args.Count() >= 6)
                                        {
                                            Process.Create.Route(args[2].ToString(), args[3].ToString(), args[4].ToString(), args[5].ToString());
                                        }
                                        else
                                        {
                                            _colorify.WriteLine("There are missing parameters make sure it is in the pattern below:", Colors.bgDanger);
                                            _colorify.WriteLine("luna -c route [NAME URL] [CONTROLLER] [FUNCTION] [METHOD GET, POST or * FOR ALL]", Colors.bgDanger);
                                            break;
                                        }
                                        break;
                                    case "helper":
                                        _colorify.WriteLine("\nHelper " + args[2].ToString() + " successfully created!", Colors.bgSuccess);
                                        break;
                                    default:
                                        _colorify.WriteLine("Command \"" + args[1].ToString() + "\" is not valid, use one of these commands: page, crud, entity or helper.", Colors.bgDanger);
                                        _colorify.ResetColor();
                                        break;
                                }
                            }
                            else
                            {
                                Function.Alert.ExistDirectory();
                            }
                            
                        }
                        else
                        {
                            _colorify.WriteLine("\nCreating a new page or derivations with null or empty name is not allowed.", Colors.bgDanger);
                            _colorify.ResetColor();
                        }
                    }
                    break;
                case "--help":
                    break;
                case "--version":
                    break;
                default:
                    if (string.IsNullOrEmpty(arg))
                    {
                        Header();
                    }
                    else if (!arg.Contains("-"))
                    {
                        Console.WriteLine("\nCommand entered is invalid or does not exist, please type --help to see options.");
                    }
                    break;
            }
        }

        private static void Header()
        {
            _colorify.Clear();
            _colorify.WriteLine(@"                                                               ", Colors.bgInfo);
            _colorify.WriteLine(@"                                                   /\          ", Colors.bgInfo);
            _colorify.WriteLine(@"                                                  / |\         ", Colors.bgInfo);
            _colorify.WriteLine(@"  ___         ___    ___   _____     ___         /  ||\        ", Colors.bgInfo);
            _colorify.WriteLine(@" |   |       |   |  |   | |     \   |   |       /   |||\       ", Colors.bgInfo);
            _colorify.WriteLine(@" |   |       |   |  |   | |      \  |   |      /    ||||\      ", Colors.bgInfo);
            _colorify.WriteLine(@" |   |       |   |  |   | |   |   \ |   |     /     |||||\     ", Colors.bgInfo);
            _colorify.WriteLine(@" |   |       |   |  |   | |   |\   \|   |    /      ||||||\    ", Colors.bgInfo);
            _colorify.WriteLine(@" |   |_____  |   |__|   | |   | \   \   |   /     / \||||||\   ", Colors.bgInfo);
            _colorify.WriteLine(@" |         | |          | |   |  \      |  /     /   \||||||\  ", Colors.bgInfo);
            _colorify.WriteLine(@" |_________| |__________| |___|   \_____| /_____/     \||||||\ ", Colors.bgInfo);
            _colorify.WriteLine(@"                                                               ", Colors.bgInfo);
            _colorify.WriteLine("", Colors.bgWarning);
            _colorify.WriteLine("  Welcome to the Luna Php Framework ", Colors.bgWarning);
            _colorify.WriteLine("  LunaCLI Version: " + Version(), Colors.bgWarning);
            _colorify.WriteLine("  http://lunaphp.com | https://github.com/lunaphp", Colors.bgWarning);
            _colorify.WriteLine("", Colors.bgWarning);
            _colorify.ResetColor();
        }

        private static string Version()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

       

        private static int RunOptionsAndReturnExitCode(Options options)
        {
            return 0;
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
        }
    }
}
