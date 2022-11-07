// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.User.SubCommands
{
    [Command(Name = "create", Description = "Creates a user and adds the user account to a group.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class Create : CmdBase
    {
        [Option(CommandOptionType.SingleValue, ShortName = "rd", LongName = "repo-definition", Description = "Specifies the repository definition used to connect to the repository (optional).", ValueName = "name", ShowInHelpText = true)]
        public string RepoDefinition { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "ru", LongName = "repo-user", Description = "The login name of the account that is used to connect to the repository.", ValueName = "login name", ShowInHelpText = true)]
        public string RepoUser { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "rp", LongName = "repo-password", Description = "The password of the account used to connect to the repository.", ValueName = "password", ShowInHelpText = true)]
        public string RepoPassword { get; set; }
                
        [Option(CommandOptionType.SingleValue, ShortName = "ur", LongName = "user-right", Description = "The rights for the new user.", ValueName = "rights", ShowInHelpText = true)]
        public UserOrGroupRightsEnum UserRights { get; set; }

        [Required]
        [EmailAddress]        
        [Option(CommandOptionType.SingleValue, ShortName = "ue", LongName = "email-address", Description = "The email address of the new user.", ValueName = "email", ShowInHelpText = true)]
        public string EmailAddress { get; set; }

        public Create(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Creating user account", ConsoleColor.Yellow);

                if (await ConnectAsync(RepoDefinition, RepoUser, RepoPassword))
                {
                    
                }
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
