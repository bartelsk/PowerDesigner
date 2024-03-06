// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Commands.Document.SubCommands.Checkin;
using System;

namespace PDRepository.CLI.Commands.Document
{
    [Command(Name = "checkin", Description = "Contains commands related to checking in repository documents.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    [Subcommand(
       typeof(File),
       typeof(Folder)
     )]
    class Checkin : CmdBase
    {
        public Checkin(IConsole console)
        {
            _console = console;
        }
    }
}
