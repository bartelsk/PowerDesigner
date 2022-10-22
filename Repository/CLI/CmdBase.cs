// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using System;
using System.Threading.Tasks;

namespace PDRepository.CLI
{
   abstract class CmdBase
   {
      protected IConsole _console;

      [Option(CommandOptionType.SingleValue, ShortName = "rd", LongName = "repo-definition", Description = "Specifies the repository definition used to connect to the repository (optional).", ValueName = "name", ShowInHelpText = true)]
      public string RepoDefinition { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "ru", LongName = "repo-user", Description = "The login name of the account that is used to connect to the repository.", ValueName = "login name", ShowInHelpText = true)]
      public string RepoUser { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "rp", LongName = "repo-password", Description = "The password of the account.", ValueName = "password", ShowInHelpText = true)]
      public string RepoPassword { get; set; }

      protected virtual Task<int> OnExecute(CommandLineApplication app)
      {
         return Task.FromResult(0);
      }

      protected void OnException(Exception ex)
      {         
         OutputError(ex.Message);         
      }

      protected void Output(string data)
      {
         OutputToConsole(data);
      }

      protected void OutputToConsole(string data, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
      {
         _console.BackgroundColor = backgroundColor;
         _console.ForegroundColor = foregroundColor;
         _console.Out.WriteLine(data);
         _console.ResetColor();
      }

      protected void OutputError(string message)
      {
         _console.BackgroundColor = ConsoleColor.Red;
         _console.ForegroundColor = ConsoleColor.White;
         _console.Error.WriteLine(message);
         _console.ResetColor();
      }
   }
}
