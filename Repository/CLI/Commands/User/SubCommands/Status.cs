// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.User.SubCommands
{
    [Command(Name = "status", Description = "Returns the user account status.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class Status : CmdBase
    {
        [Option(CommandOptionType.SingleValue, ShortName = "rd", LongName = "repo-definition", Description = "Specifies the repository definition used to connect to the repository (optional).", ValueName = "name", ShowInHelpText = true)]
        public string RepoDefinition { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "ru", LongName = "repo-user", Description = "The login name of the account that is used to connect to the repository.", ValueName = "login name", ShowInHelpText = true)]
        public string RepoUser { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "rp", LongName = "repo-password", Description = "The password of the account used to connect to the repository.", ValueName = "password", ShowInHelpText = true)]
        public string RepoPassword { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "l", LongName = "login-name", Description = "Specifies the login name of the user for which to get its status.", ValueName = "login name", ShowInHelpText = true)]
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

                // check actual status
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
