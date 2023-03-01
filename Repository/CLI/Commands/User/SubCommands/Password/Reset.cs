// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.Password.SubCommands
{
    [Command(Name = "reset", Description = "Resets a user password.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class Reset : CmdBase
    {
        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "ln", LongName = "login-name", Description = "Specifies the login name of the user for which to reset the password.", ValueName = "login name", ShowInHelpText = true)]
        public string LoginName { get; set; }

        public Reset(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Resetting user password", ConsoleColor.Yellow);

                if (await ConnectAsync())
                {
                    if (_client.UserClient.UserExists(LoginName))
                    {
                        string temporaryPassword = _client.UserClient.ResetPassword(LoginName);

                        Common.User user = _client.UserClient.GetUserInfo(LoginName);

                        OutputNewLine("Updated user details:\r\n", ConsoleColor.Yellow);

                        using (TableWriter writer = new TableWriter(_console, padding: 2))
                        {
                            writer.StartTable(2);
                            WriteTableHeader(writer, "Property", "Value", ConsoleColor.Blue, ConsoleColor.Blue);

                            WriteRow(writer, "Full name", user.FullName);
                            WriteRow(writer, "Login name", user.LoginName);
                            WriteRow(writer, "New (temp) password", @temporaryPassword, ConsoleColor.Gray, ConsoleColor.Green);

                            writer.WriteTable();
                        }
                    }
                    else
                    {
                        OutputError($"A user with login name '{ LoginName }' does not exist, or the currently connected repository user does not have permissions to perform this action.");
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                OnException(ex);
                return 1;
            }
        }
    }
}
