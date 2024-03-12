// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        public List(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Listing documents", ConsoleColor.Yellow);

                if (await ConnectAsync())
                {
                    if (_client.DocumentClient.FolderExists(FolderPath))
                    {
                        List<Common.Document> documents = _client.DocumentClient.ListDocuments(FolderPath, Recursive)?.OrderBy(d => d.Location).ToList();

                        if (documents?.Count > 0)
                        {
                            OutputNewLine("Document details:\r\n", ConsoleColor.Yellow);

                            using (TableWriter writer = new TableWriter(_console, padding: 2))
                            {
                                writer.StartTable(4);
                                writer.StartRow(true);
                                writer.AddColumn("Name", ConsoleColor.Blue);
                                writer.AddColumn("Type", ConsoleColor.Blue);
                                writer.AddColumn("Version", ConsoleColor.Blue);
                                writer.AddColumn("Location", ConsoleColor.Blue);
                                writer.EndRow();

                                documents.ForEach(doc =>
                                {
                                    writer.StartRow();
                                    writer.AddColumn(doc.Name);
                                    writer.AddColumn(doc.ClassName);
                                    writer.AddColumn(doc.Version);
                                    writer.AddColumn(doc.Location);
                                    writer.EndRow();
                                });
                                writer.WriteTable();
                            }
                        }
                        else
                        {
                            OutputNewLine($"No documents found in folder '{FolderPath}'.");
                        }
                    }
                    else
                    {
                        OutputError($"The folder '{FolderPath}' does not exist.");
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
