﻿// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Commands.Document.SubCommands.Checkout;
using System;

namespace PDRepository.CLI.Commands.Document
{
    [Command(Name = "checkout", Description = "Contains commands related to checking out repository documents.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    [Subcommand(
       typeof(File),
       typeof(Folder)
     )]
    class Checkout : CmdBase
    {
        public Checkout(IConsole console)
        {
            _console = console;
        }
    }
}
