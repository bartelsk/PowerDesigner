// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Output;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.Document.SubCommands
{
    [Command(Name = "list", Description = "Enumerates repository documents.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class List : CmdBase
    {
        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "fp", LongName = "folder-path", Description = "The repository folder to query.", ValueName = "folder", ShowInHelpText = true)]
        public string FolderPath { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "r", LongName = "recursive", Description = "Indicates whether to list documents in any sub-folder of the specified repository folder (optional).", ValueName = "true/false", ShowInHelpText = true)]
        public bool Recursive { get; set; }

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
                Output("Listing documents", ConsoleColor.Yellow);

                if (await ConnectAsync(RepoDefinition, RepoUser, RepoPassword))
                {
                    List<Common.Document> documents = _client.DocumentClient.ListDocuments(FolderPath, Recursive);

                    if (documents?.Count > 0)
                    {
                        OutputNewLine("Document details:\r\n", ConsoleColor.Yellow);

                        using (TableWriter writer = new TableWriter(_console, padding: 2))
                        {
                            writer.StartTable(documents);
                            writer.AddHeaderRow(documents, ConsoleColor.Blue);

                            writer.AddRows(documents);
                            writer.WriteTable();
                        }
                    }
                    else
                    {
                        OutputNewLine($"No documents found in folder '{FolderPath}'.");
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
