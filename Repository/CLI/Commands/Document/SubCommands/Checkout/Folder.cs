// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.Document.SubCommands
{
   [Command(Name = "folder", Description = "Checks out all documents in a repository folder.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
   class Folder : CmdBase
   {
      [Required]
      [Option(CommandOptionType.SingleValue, ShortName = "fp", LongName = "folder-path", Description = "The repository folder from which to retrieve the documents.", ValueName = "folder", ShowInHelpText = true)]
      public string FolderPath { get; set; }

      [Required]
      [Option(CommandOptionType.SingleValue, ShortName = "tf", LongName = "target-folder", Description = "The folder on disc to use as the check-out location for the documents.", ValueName = "folder", ShowInHelpText = true)]
      public string TargetFolderPath { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "r", LongName = "recursive", Description = "Indicates whether to retrieve documents in any sub-folder of the specified repository folder (optional).", ValueName = "false", ShowInHelpText = true)]
      public bool Recursive { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "ps", LongName = "preserve-structure", Description = "Indicates whether to mimic the repository folder structure on the local disc when checking out. Applies to recursive check-outs only. (optional).", ValueName = "false", ShowInHelpText = true)]
      public bool PreserveFolderStructure {  get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "dn", LongName = "document-name", Description = "The name of the document.", ValueName = "user or group", ShowInHelpText = true)]
      public string DocumentName { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "rd", LongName = "repo-definition", Description = "Specifies the repository definition used to connect to the repository (optional).", ValueName = "name", ShowInHelpText = true)]
      public string RepoDefinition { get; set; }

      [Required]
      [Option(CommandOptionType.SingleValue, ShortName = "ru", LongName = "repo-user", Description = "The login name of the account that is used to connect to the repository.", ValueName = "login name", ShowInHelpText = true)]
      public string RepoUser { get; set; }

      [Required]
      [Option(CommandOptionType.SingleValue, ShortName = "rp", LongName = "repo-password", Description = "The password of the account used to connect to the repository.", ValueName = "password", ShowInHelpText = true)]
      public string RepoPassword { get; set; }

      public Folder(IConsole console)
      {
         _console = console;
      }

      protected override async Task<int> OnExecute(CommandLineApplication app)
      {
         try
         {
            Output("Checkout folder", ConsoleColor.Yellow);

            if (await ConnectAsync(RepoDefinition, RepoUser, RepoPassword))
            {
               // Register event handler
               _client.DocumentClient.DocumentCheckedOut += DocumentClient_DocumentCheckedOut;

               // Check out document
               Output($"Checking out all document in folder '{FolderPath}' into '{TargetFolderPath}'...");
               _client.DocumentClient.CheckOutDocuments(FolderPath, TargetFolderPath, Recursive, PreserveFolderStructure);
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
         Output($"Checked out document '{e.DocumentName}' to file '{e.CheckOutFileName}'");
      }
   }
}
