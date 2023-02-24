// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using System;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.Auth.SubCommands
{
    [Command(Name = "logout", Description = "Removes persisted repository connection information.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class LogOut : CmdBase
    {
        public LogOut(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Removing connection profile", ConsoleColor.Yellow);

                if (ConnectionSettingsExist)
                {
                    DeleteConnectionSettingsProfile();
                    OutputNewLine("Successfully removed connection profile.", ConsoleColor.Green);
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
