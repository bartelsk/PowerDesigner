// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Output;
using PDRepository.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.Branch.SubCommands
{
   [Command(Name = "create", Description = "Creates a branch and assigns access rights.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class Create : CmdBase
    {
      [Required]
      [Option(CommandOptionType.SingleValue, ShortName = "sb", LongName = "source-branch", Description = "The location of the source branch folder in the repository.", ValueName = "folder name", ShowInHelpText = true)]
      public string SourceBranchFolder { get; set; }

      [Required]
      [Option(CommandOptionType.SingleValue, ShortName = "bn", LongName = "branch-name", Description = "The name for the new branch.", ValueName = "branch name", ShowInHelpText = true)]
      public string NewBranchName { get; set; }
            
      [Option(CommandOptionType.SingleValue, ShortName = "ug", LongName = "user-group", Description = "The loginname of a user or a group name that is assigned branch permissions via the UserOrGroupPermission option (optional).", ValueName = "user or group", ShowInHelpText = true)]
      public string UserOrGroup { get; set; }
            
      [Option(CommandOptionType.SingleValue, ShortName = "ugp", LongName = "user-group-permission", ValueName = "permission", ShowInHelpText = true,
         Description = "Permission settings for the user or group for the new branch. Allowed values are: NotSet, Listable, Read, Submit, Write, Full (optional)."
      )]
      public string UserOrGroupPermission { get; set; }            

      [Option(CommandOptionType.SingleValue, ShortName = "rd", LongName = "repo-definition", Description = "Specifies the repository definition used to connect to the repository (optional).", ValueName = "name", ShowInHelpText = true)]
        public string RepoDefinition { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "ru", LongName = "repo-user", Description = "The login name of the account that is used to connect to the repository.", ValueName = "login name", ShowInHelpText = true)]
        public string RepoUser { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "rp", LongName = "repo-password", Description = "The password of the account used to connect to the repository.", ValueName = "password", ShowInHelpText = true)]
        public string RepoPassword { get; set; }

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

            if (await ConnectAsync(RepoDefinition, RepoUser, RepoPassword))
            {
               if (!_client.BranchClient.BranchExists(SourceBranchFolder, NewBranchName))
               {
                  if (string.IsNullOrEmpty(UserOrGroup))
                  {
                     _client.BranchClient.CreateBranch(SourceBranchFolder, NewBranchName);
                  }
                  else
                  {
                     Permission branchPermission = new Permission()
                     {
                        CopyToChildren = true,
                        PermissionType = userOrGroupPermission,
                        UserOrGroupName = UserOrGroup
                     };
                     _client.BranchClient.CreateBranch(SourceBranchFolder, NewBranchName, branchPermission);
                  }

                  Output("\r\nAvailable branches:\r\n", ConsoleColor.Yellow);

                  List<Common.Branch> branches = string.IsNullOrEmpty(UserOrGroup) ? _client.BranchClient.ListBranches(SourceBranchFolder) : _client.BranchClient.ListBranches(SourceBranchFolder, UserOrGroup);

                  using (TableWriter writer = new TableWriter(_console, padding: 2))
                  {
                     writer.StartTable(branches);
                     writer.AddHeaderRow(branches, ConsoleColor.Blue);

                     writer.AddRows(branches);
                     writer.WriteTable();
                  }
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
