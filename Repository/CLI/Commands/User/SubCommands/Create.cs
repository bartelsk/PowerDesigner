﻿// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Utils;
using PDRepository.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.User.SubCommands
{
    [Command(Name = "create", Description = "Creates a user and optionally adds the user account to an existing group.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class Create : CmdBase
    {
        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "ln", LongName = "login-name", Description = "Specifies the login name for the new user.", ValueName = "login name", ShowInHelpText = true)]
        public string LoginName { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "fn", LongName = "full-name", Description = "The real name of the new user.", ValueName = "full name", ShowInHelpText = true)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Option(CommandOptionType.SingleValue, ShortName = "ue", LongName = "email-address", Description = "The email address of the new user.", ValueName = "email", ShowInHelpText = true)]
        public string EmailAddress { get; set; }

        [Option(CommandOptionType.MultipleValue, ShortName = "ur", LongName = "user-right", ValueName = "rights", ShowInHelpText = true, Description =
            "The rights for the new user. Can be specified multiple times for compound rights. Allowed values are: None, Connect, FreezeVersions, LockVersions, ManageBranches, ManageConfigurations, ManageAllDocuments, ManageUsers, ManageRepository, EditPortalObjects, EditPortalExtensions (optional)."
        )]
        public string[] UserRights { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "ug", LongName = "group", Description = "The name of the group to which to add the user (optional).", ValueName = "group", ShowInHelpText = true)]
        public string Group { get; set; }

        public Create(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Creating user account", ConsoleColor.Yellow);

                UserOrGroupRightsEnum userRights = ParseUserOrGroupRights(UserRights);

                if (await ConnectAsync())
                {
                    if (!_client.UserClient.UserExists(LoginName))
                    {
                        string temporaryPassword;
                        if (string.IsNullOrEmpty(Group))
                        {
                            _client.UserClient.CreateUser(LoginName, FullName, EmailAddress, out temporaryPassword, userRights);
                        }
                        else
                        {
                            _client.UserClient.CreateUser(LoginName, FullName, EmailAddress, out temporaryPassword, userRights, Group);
                        }

                        Common.User user = _client.UserClient.GetUserInfo(LoginName);

                        OutputNewLine("New user details:\r\n", ConsoleColor.Yellow);

                        using (TableWriter writer = new TableWriter(_console, padding: 2))
                        {
                            writer.StartTable(2);
                            WriteTableHeader(writer, "Property", "Value", ConsoleColor.Blue, ConsoleColor.Blue);

                            WriteRow(writer, "Full name", user.FullName);
                            WriteRow(writer, "Login name", user.LoginName);
                            WriteRow(writer, "Temporary password", @temporaryPassword);
                            WriteRow(writer, "Status", user.Status, valueColor: (user.Status == UserStatusEnum.Active) ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);

                            writer.WriteTable();
                        }

                        OutputUserRightsAndGroupPermissions(user);
                    }
                    else
                    {
                        OutputError($"A user with login name '{ LoginName }' already exists.");
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
