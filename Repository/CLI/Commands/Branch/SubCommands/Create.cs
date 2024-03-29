﻿// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.Branch.SubCommands
{
    [Command(Name = "create", Description = "Creates a branch based on a base branch and assigns access rights.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class Create : CmdBase
    {
        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "bb", LongName = "base-branch", Description = "The path of the base branch folder in the repository. The contents of the base branch will be copied into the new branch.", ValueName = "path", ShowInHelpText = true)]
        public string BaseBranchFolder { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "bn", LongName = "branch-name", Description = "The name for the new branch.", ValueName = "name", ShowInHelpText = true)]
        public string NewBranchName { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "ug", LongName = "user-group", Description = "Either the login name of a user or a group name that is assigned branch permissions via the 'user-group-permission' option (optional).", ValueName = "user or group", ShowInHelpText = true)]
        public string UserOrGroup { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "ugp", LongName = "user-group-permission", ValueName = "permission", ShowInHelpText = true,
           Description = "Permissions for the user or group for the new branch. Allowed values are: NotSet, Listable, Read, Submit, Write, Full (optional)."
        )]
        public string UserOrGroupPermission { get; set; }

        public Create(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Creating branch", ConsoleColor.Yellow);

                PermissionTypeEnum userOrGroupPermission = ParsePermissionType(UserOrGroupPermission);

                if (await ConnectAsync())
                {
                    if (!_client.BranchClient.BranchExists(BaseBranchFolder, NewBranchName))
                    {
                        if (string.IsNullOrEmpty(UserOrGroup))
                        {
                            _client.BranchClient.CreateBranch(BaseBranchFolder, NewBranchName);
                        }
                        else
                        {
                            Permission branchPermission = new Permission()
                            {
                                CopyToChildren = true,
                                PermissionType = userOrGroupPermission,
                                UserOrGroupName = UserOrGroup
                            };
                            _client.BranchClient.CreateBranch(BaseBranchFolder, NewBranchName, branchPermission);
                        }
                        OutputNewLine($"Branch created.");
                    }
                    else
                    {
                        OutputError($"A branch called '{NewBranchName}' already exists.");
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
