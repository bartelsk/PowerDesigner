// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public List(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Listing branches", ConsoleColor.Yellow);

                if (await ConnectAsync())
                {
                    List<Common.Branch> branches = string.IsNullOrEmpty(UserOrGroupNameFilter) ? _client.BranchClient.ListBranches(RootFolder) : _client.BranchClient.ListBranches(RootFolder, UserOrGroupNameFilter);

                    if (branches != null)
                    {
                        OutputNewLine("Branch details:\r\n", ConsoleColor.Yellow);

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
                        OutputNewLine($"No branches found relative to folder '{ RootFolder}'.");
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
