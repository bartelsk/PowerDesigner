// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using System;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.Branch.SubCommands
{
   [Command(Name = "create", Description = "Creates a branch and assigns access rights.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
   class Create : CmdBase
   {
      public Create(IConsole console)
      {
         _console = console;
      }

      protected override async Task<int> OnExecute(CommandLineApplication app)
      {
         try
         {
            Output("Hello from branch create!");
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
