// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Commands.Auth.SubCommands;
using System;

namespace PDRepository.CLI.Commands.Auth
{
    [Command(Name = "auth", Description = "Contains commands related to the repository connection.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    [Subcommand(
        typeof(LogIn),
        typeof(LogOut)
     )]
    class Auth : CmdBase
    {
        public Auth(IConsole console)
        {
            _console = console;
        }
    }
}
