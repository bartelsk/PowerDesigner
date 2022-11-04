// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Commands.User.SubCommands;
using System;

namespace PDRepository.CLI.Commands.User
{
    [Command(Name = "user", Description = "Contains commands related to repository users.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    [Subcommand(
       typeof(Create),
       typeof(Status),
       typeof(Unblock)
    )]
    class User : CmdBase
    {
        public User(IConsole console)
        {
            _console = console;
        }
    }
}
