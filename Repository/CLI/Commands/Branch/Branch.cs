// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Commands.Branch.SubCommands;
using System;

namespace PDRepository.CLI.Commands.Branch
{
   [Command(Name = "branch", Description = "Contains commands related to repository branches.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
   [Subcommand(
      typeof(Create),         
      typeof(List)
   )]
   class Branch : CmdBase
   {
      public Branch(IConsole console)
      {
         _console = console;
      }
   }
}
