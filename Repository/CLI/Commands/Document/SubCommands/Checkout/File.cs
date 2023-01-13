// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.Document.SubCommands
{
    [Command(Name = "file", Description = "Checks out a single document in a repository folder.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class File : CmdBase
    {
        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "fp", LongName = "folder-path", Description = "The repository folder from which to retrieve the document.", ValueName = "folder", ShowInHelpText = true)]
        public string FolderPath { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "tf", LongName = "target-folder", Description = "The folder on disc to use as the check-out location for the document.", ValueName = "folder", ShowInHelpText = true)]
        public string TargetFolderPath { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "dn", LongName = "document-name", Description = "The name of the document to check out.", ValueName = "name", ShowInHelpText = true)]
        public string DocumentName { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "dv", LongName = "document-version", Description = "The document version. The latest version of the document will be checked out if the specified document version does not exist. The version must also belong to the same branch as the current object (optional).", ValueName = "version", ShowInHelpText = true)]
        public int DocumentVersion { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "rd", LongName = "repo-definition", Description = "Specifies the repository definition used to connect to the repository (optional).", ValueName = "name", ShowInHelpText = true)]
        public string RepoDefinition { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "ru", LongName = "repo-user", Description = "The login name of the account that is used to connect to the repository.", ValueName = "login name", ShowInHelpText = true)]
        public string RepoUser { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "rp", LongName = "repo-password", Description = "The password of the account used to connect to the repository.", ValueName = "password", ShowInHelpText = true)]
        public string RepoPassword { get; set; }

        public File(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Checkout document", ConsoleColor.Yellow);

                if (await ConnectAsync(RepoDefinition, RepoUser, RepoPassword))
                {
                    // Register event handler
                    _client.DocumentClient.DocumentCheckedOut += DocumentClient_DocumentCheckedOut;

                    // Check out document
                    if (DocumentVersion == 0)
                    {
                        OutputNewLine($"Checking out document '{DocumentName}' to '{TargetFolderPath}'...", ConsoleColor.Yellow);
                        _client.DocumentClient.CheckOutDocument(FolderPath, DocumentName, TargetFolderPath);
                    }
                    else
                    {
                        OutputNewLine($"Attempting to check out version '{DocumentVersion}' of document '{DocumentName}' to '{TargetFolderPath}'...");                        
                        _client.DocumentClient.CheckOutDocument(FolderPath, DocumentName, TargetFolderPath, DocumentVersion);
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

        private void DocumentClient_DocumentCheckedOut(object sender, CheckOutEventArgs e)
        {
            OutputNewLine($"Checked out document '{e.DocumentName}' version '{e.DocumentVersion}' to file '{e.CheckOutFileName}'.");
        }
    }
}
