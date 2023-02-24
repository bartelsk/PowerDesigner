// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.User.SubCommands
{
    [Command(Name = "unblock", Description = "Unblocks a user account.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class Unblock : CmdBase
    {
        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "ln", LongName = "login-name", Description = "Specifies the login name of the user to unblock.", ValueName = "login name", ShowInHelpText = true)]
        public string LoginName { get; set; }

        public Unblock(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Unblocking user account", ConsoleColor.Yellow);

                if (await ConnectAsync())
                {
                    if (_client.UserClient.UserExists(LoginName))
                    {
                        _client.UserClient.UnblockUser(LoginName);

                        Common.User user = _client.UserClient.GetUserInfo(LoginName);

                        OutputNewLine("User details:\r\n", ConsoleColor.Yellow);

                        using (TableWriter writer = new TableWriter(_console, padding: 2))
                        {
                            writer.StartTable(2);
                            WriteTableHeader(writer, "Property", "Value", ConsoleColor.Blue, ConsoleColor.Blue);

                            WriteRow(writer, "Status", user.Status, valueColor: (user.Status == Common.UserStatusEnum.Active) ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
                            WriteRow(writer, "Blocked", user.Blocked);
                            WriteRow(writer, "Disabled", user.Disabled);

                            writer.WriteTable();
                        }
                    }
                    else
                    {
                        OutputError($"A user with login name '{ LoginName }' does not exist.");
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
