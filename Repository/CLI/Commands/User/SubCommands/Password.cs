// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Commands.Password.SubCommands;
using System;

namespace PDRepository.CLI.Commands.User
{
    [Command(Name = "password", Description = "Contains commands related to repository user passwords.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    [Subcommand(
       typeof(Reset)       
    )]
    class Password : CmdBase
    {
        public Password(IConsole console)
        {
            _console = console;
        }
    }
}
