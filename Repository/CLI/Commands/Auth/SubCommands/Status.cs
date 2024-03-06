// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Utils;
using System;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.Auth.SubCommands
{
    [Command(Name = "status", Description = "Displays repository connection information.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class Status : CmdBase
    {
        public Status(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Retrieving connection profile", ConsoleColor.Yellow);

                if (ConnectionSettingsExist)
                {
                    LoadConnectionSettings();

                    OutputNewLine("Connection details:\r\n", ConsoleColor.Yellow);

                    using (TableWriter writer = new TableWriter(_console, padding: 2))
                    {
                        writer.StartTable(2);
                        WriteTableHeader(writer, "Property", "Value", ConsoleColor.Blue, ConsoleColor.Blue);

                        WriteRow(writer, "Repository definition", _connectionSettings.RepositoryDefinition);
                        WriteRow(writer, "User name", _connectionSettings.User);
                       
                        writer.WriteTable();
                    }
                }
                else
                {
                    OutputNewLine("No connection profile found!", ConsoleColor.Red);
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                OnException(ex);
                return 1;
            }
        }
    }
}
