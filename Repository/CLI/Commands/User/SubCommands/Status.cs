// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using System;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.User.SubCommands
{
   [Command(Name = "status", Description = "Returns the user account status.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
   class Status : CmdBase
   {
      [Option(CommandOptionType.SingleValue, ShortName = "l", LongName = "login-name", Description = "Specifies the login name of the user.", ValueName = "Login name", ShowInHelpText = true)]
      public string LoginName { get; set; }

      public Status(IConsole console)
      {
         _console = console;
      }

      protected override async Task<int> OnExecute(CommandLineApplication app)
      {
         try
         {
            Output("Hello from user status!");
            Output($"RepoDef: { RepoDefinition } - RepoUser: { RepoUser } - RepoPassword: { RepoPassword }");

            Output($"Login name: { LoginName }");


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
