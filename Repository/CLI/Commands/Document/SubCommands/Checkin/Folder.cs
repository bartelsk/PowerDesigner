// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.Document.SubCommands.Checkin
{    
    [Command(Name = "folder", Description = "Checks in multiple documents into a repository folder.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class Folder : CmdBase
    {
        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "fp", LongName = "folder-path", Description = "The repository folder in which to add the documents.", ValueName = "folder", ShowInHelpText = true)]
        public string FolderPath { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "sf", LongName = "source-folder", Description = "The folder on disc that contains the files to check in.", ValueName = "folder", ShowInHelpText = true)]
        public string SourceFolderPath { get; set; }

        public Folder(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Checkin documents", ConsoleColor.Yellow);

                if (await ConnectAsync())
                {
                    // Register event handler
                    _client.DocumentClient.DocumentCheckedIn += DocumentClient_DocumentCheckedIn;

                    OutputNewLine($"Checking in documents from '{SourceFolderPath}' to '{FolderPath}'...\r\n", ConsoleColor.Yellow);
                    _client.DocumentClient.CheckInDocuments(FolderPath, SourceFolderPath);
                }
                return 0;
            }
            catch (Exception ex)
            {
                OnException(ex);
                return 1;
            }
        }

        private void DocumentClient_DocumentCheckedIn(object sender, CheckInEventArgs e)
        {
            Output($"Checked in document '{e.DocumentName}' version '{e.DocumentVersion}' from file '{e.CheckInFileName}' into folder '{e.DocumentFolder}'.");
        }
    }
}
