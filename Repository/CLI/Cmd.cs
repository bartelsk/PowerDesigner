// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Commands.Branch;
using PDRepository.CLI.Commands.Document;
using PDRepository.CLI.Commands.User;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace PDRepository.CLI
{
    [Command(Name = "pdr", Description = "PowerDesigner Repository CLI", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    [VersionOptionFromMember("--version", ShortName = "v", LongName = "version", MemberName = nameof(GetVersion), ShowInHelpText = false)]
    [Subcommand(
      typeof(Branch),
      typeof(Document),
      typeof(User)
    )]
    class Cmd : CmdBase
    {
        public Cmd(IConsole console)
        {
            _console = console;
        }

        protected override Task<int> OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return Task.FromResult(0);
        }

        private static string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
        }
    }
}
