// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.Branch.SubCommands
{
    [Command(Name = "list", Description = "Enumerates existing branches.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class List : CmdBase
    {
        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "rf", LongName = "root-folder", Description = "The repository folder from which to start the enumeration.", ValueName = "folder", ShowInHelpText = true)]
        public string RootFolder { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "ug", LongName = "filter", Description = "A user login or group name used to filter branches based on access permission (optional).", ValueName = "user or group", ShowInHelpText = true)]
        public string UserOrGroupNameFilter { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "rd", LongName = "repo-definition", Description = "Specifies the repository definition used to connect to the repository (optional).", ValueName = "name", ShowInHelpText = true)]
        public string RepoDefinition { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "ru", LongName = "repo-user", Description = "The login name of the account that is used to connect to the repository.", ValueName = "login name", ShowInHelpText = true)]
        public string RepoUser { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "rp", LongName = "repo-password", Description = "The password of the account used to connect to the repository.", ValueName = "password", ShowInHelpText = true)]
        public string RepoPassword { get; set; }        

        public List(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Listing branches", ConsoleColor.Yellow);

                if (await ConnectAsync(RepoDefinition, RepoUser, RepoPassword))
                {
                    List<Common.Branch> branches = string.IsNullOrEmpty(UserOrGroupNameFilter) ? _client.BranchClient.ListBranches(RootFolder) : _client.BranchClient.ListBranches(RootFolder, UserOrGroupNameFilter);

                    if (branches != null)
                    {
                        int maxLength = branches.Max(b => b.Name.Length);
                        int padding = 3;

                        Output("\r\nBranch details:\r\n", ConsoleColor.Blue);
                        OutputTableRowSpace("Name", "Relative path", maxLength, padding, ConsoleColor.DarkGreen);
                        OutputTableRowSpace("----", "-------------", maxLength, padding, ConsoleColor.DarkGreen);

                        

                        branches.ForEach(b =>
                        {
                            OutputTableRowSpace(b.Name, b.RelativePath, maxLength, padding);

                        });
                    }
                    else
                    {
                        Output($"No branches found relative to folder '{ RootFolder} '.");
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
