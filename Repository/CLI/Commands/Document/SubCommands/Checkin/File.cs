// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.Document.SubCommands.Checkin
{
    [Command(Name = "file", Description = "Checks in a single document to a repository folder.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class File : CmdBase
    {
        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "fp", LongName = "folder-path", Description = "The repository folder in which to add the document.", ValueName = "folder", ShowInHelpText = true)]
        public string FolderPath { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "fn", LongName = "file-name", Description = "The fully-qualified name of the file to check in.", ValueName = "file", ShowInHelpText = true)]
        public string FileName { get; set; }
        
        public File(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Checkin document", ConsoleColor.Yellow);

                if (await ConnectAsync())
                {
                    // Register event handler
                    _client.DocumentClient.DocumentCheckedIn += DocumentClient_DocumentCheckedIn;

                    OutputNewLine($"Checking in document '{FileName}' to '{FolderPath}'...", ConsoleColor.Yellow);
                    _client.DocumentClient.CheckInDocument(FolderPath, FileName, out string version);
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
            OutputNewLine($"Checked in document '{e.DocumentName}' version '{e.DocumentVersion}' from file '{e.CheckInFileName}' into folder '{e.DocumentFolder}'.");
        }
    }
}
