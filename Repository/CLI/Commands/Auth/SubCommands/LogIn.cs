// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PDRepository.CLI.Commands.Auth.SubCommands
{
    [Command(Name = "login", Description = "Persists repository connection information.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    class LogIn : CmdBase
    {
        [Option(CommandOptionType.SingleValue, ShortName = "rd", LongName = "repo-definition", Description = "Specifies the repository definition used to connect to the repository (optional).", ValueName = "name", ShowInHelpText = true)]
        public string RepoDefinition { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, ShortName = "ru", LongName = "repo-user", Description = "The login name of the account that is used to connect to the repository.", ValueName = "login name", ShowInHelpText = true)]
        public string RepoUser { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "rp", LongName = "repo-password", Description = "The password of the account used to connect to the repository. If omitted, a secure prompt will appear to enter the password.", ValueName = "password", ShowInHelpText = true)]
        public string RepoPassword { get; set; }

        public LogIn(IConsole console)
        {
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                Output("Creating connection profile", ConsoleColor.Yellow);

                if (ConnectionProfileExists)
                {
                    OutputNewLine("Connection profile already exists!", ConsoleColor.Green);
                }
                else
                {
                    ConnectionProfile connectionProfile = new ConnectionProfile()
                    {
                        Password = string.IsNullOrEmpty(RepoPassword) ? Security.SecureStringToString(Prompt.GetPasswordAsSecureString($"Password of user '{RepoUser}':")) : RepoPassword,
                        RepositoryDefinition = RepoDefinition,
                        User = RepoUser
                    };

                    if (SaveConnectionProfile(connectionProfile))
                    {
                        OutputNewLine($"Connection profile successfully created.", ConsoleColor.Green);
                    }
                    else
                    {
                        OutputError("An error occurred while creating a connection profile.");
                    }
                }
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                OnException(ex);
                return 1;
            }
        }
    }
}
