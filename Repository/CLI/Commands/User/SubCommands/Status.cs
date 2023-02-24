// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.User.SubCommands
{
    [Command(Name = "status", Description = "Returns the user account status.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class Status : CmdBase
    {
        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "ln", LongName = "login-name", Description = "Specifies the login name of the user for which to get its status.", ValueName = "login name", ShowInHelpText = true)]
        public string LoginName { get; set; }

        public Status(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Retrieving user account status", ConsoleColor.Yellow);

                if (await ConnectAsync())
                {
                    if (_client.UserClient.UserExists(LoginName))
                    {
                        Common.User user = _client.UserClient.GetUserInfo(LoginName);

                        OutputNewLine("User details:\r\n", ConsoleColor.Yellow);

                        using (TableWriter writer = new TableWriter(_console, padding: 2))
                        {
                            writer.StartTable(2);
                            WriteTableHeader(writer, "Property", "Value", ConsoleColor.Blue, ConsoleColor.Blue);

                            WriteRow(writer, "Name", user.FullName);
                            WriteRow(writer, "Comment", !string.IsNullOrEmpty(user.Comment) ? user.Comment : "(none)");
                            WriteRow(writer, "Status", user.Status, valueColor: (user.Status == Common.UserStatusEnum.Active) ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
                            WriteRow(writer, "Blocked", user.Blocked);
                            WriteRow(writer, "Disabled", user.Disabled);
                            WriteRow(writer, "Last login date", user.LastLoginDate);
                            WriteRow(writer, "Last modified date", user.LastModifiedDate);

                            writer.WriteTable();
                        }

                        OutputUserRightsAndGroupPermissions(user);
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
