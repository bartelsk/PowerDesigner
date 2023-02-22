// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.Document.SubCommands
{
    [Command(Name = "info", Description = "Returns repository document information.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class Info : CmdBase
    {
        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "fp", LongName = "folder-path", Description = "The repository folder that contains the document.", ValueName = "folder", ShowInHelpText = true)]
        public string FolderPath { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "dn", LongName = "document-name", Description = "The name of the document.", ValueName = "name", ShowInHelpText = true)]
        public string DocumentName { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "rd", LongName = "repo-definition", Description = "Specifies the repository definition used to connect to the repository (optional).", ValueName = "name", ShowInHelpText = true)]
        public string RepoDefinition { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "ru", LongName = "repo-user", Description = "The login name of the account that is used to connect to the repository.", ValueName = "login name", ShowInHelpText = true)]
        public string RepoUser { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "rp", LongName = "repo-password", Description = "The password of the account used to connect to the repository.", ValueName = "password", ShowInHelpText = true)]
        public string RepoPassword { get; set; }

        public Info(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Retrieving document information", ConsoleColor.Yellow);

                if (await ConnectAsync(RepoDefinition, RepoUser, RepoPassword))
                {
                    if (_client.DocumentClient.DocumentExists(FolderPath, DocumentName))
                    {
                        Common.Document doc = _client.DocumentClient.GetDocumentInfo(FolderPath, DocumentName);
                        if (doc != null)
                        {
                            OutputNewLine("Document details:\r\n", ConsoleColor.Yellow);

                            using (TableWriter writer = new TableWriter(_console, padding: 2))
                            {
                                writer.StartTable(2);
                                WriteTableHeader(writer, "Property", "Value", ConsoleColor.Blue, ConsoleColor.Blue);

                                WriteRow(writer, "Name", doc.Name);
                                WriteRow(writer, "Class name", doc.ClassName);
                                WriteRow(writer, "Extraction file name", doc.ExtractionFileName);
                                WriteRow(writer, "Is frozen", doc.IsFrozen);
                                WriteRow(writer, "Is locked", doc.IsLocked);
                                WriteRow(writer, "Location", doc.Location);
                                WriteRow(writer, "Object type", doc.ObjectType);
                                WriteRow(writer, "Version", doc.Version);
                                WriteRow(writer, "Version comment", !string.IsNullOrEmpty(doc.VersionComment) ? doc.VersionComment : "(none)");

                                writer.WriteTable();
                            }
                        }
                    }
                    else
                    {
                        OutputError($"The document '{DocumentName}' does not exist in folder '{FolderPath}'.");
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
