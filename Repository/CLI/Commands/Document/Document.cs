// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Commands.Document.SubCommands;
using System;

namespace PDRepository.CLI.Commands.Document
{
    [Command(Name = "document", Description = "Contains commands related to repository documents.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    [Subcommand(
        typeof(Checkin),
        typeof(Checkout),
        typeof(Info),
        typeof(List)
     )]
    class Document : CmdBase
    {
        public Document(IConsole console)
        {
            _console = console;
        }
    }
}
