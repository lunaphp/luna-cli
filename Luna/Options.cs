using CommandLine;
using System.Collections.Generic;

namespace Luna
{
    class Options
    {
        [Option('n', "new", HelpText = "Command create new project, template or api."+
            "\nNEW PROJECT: luna -n project [NAME]"+
            "\nNEW TEMPLATE: luna -n template [TEMPLATE]"+
            "\nNEW API: luna -n api [NAME]")]
        public IEnumerable<string> NewCommand { get; set; }

        [Option('c', "create", HelpText = "Command create page, crud, entity or helper."+
            "\nCREATE PAGE: luna -c page [NAME]"+
            "\nCREATE CRUD: luna -c route [NAME URL] [CONTROLLER] [FUNCTION] [METHOD GET, POST or * FOR ALL]" +
            "\nCREATE CRUD: luna -c crud [TABLE]"+
            "\nCREATE PAGE: luna -c entity [TABLE] or [* FOR ALL]"+
            "\nCREATE PAGE: luna -c helper [NAME]")]
        public IEnumerable<string> CreateCommand { get; set; }
    }
}
