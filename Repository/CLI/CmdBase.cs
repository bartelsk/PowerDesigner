// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.Common;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace PDRepository.CLI
{
    abstract class CmdBase
    {
        protected IConsole _console;
        protected RepositoryClient _client = null;

        protected virtual Task<int> OnExecute(CommandLineApplication app)
        {
            return Task.FromResult(0);
        }

        protected async Task<bool> ConnectAsync(string repoDefinition, string repoUser, string repoPassword)
        {
            bool connected = false;
            try
            {
                ConnectionSettings connectionSettings = new ConnectionSettings()
                {
                    Password = repoPassword,
                    RepositoryDefinition = repoDefinition,
                    User = repoUser
                };

                Output("\r\nConnecting...");
                await Task.Run(() =>
                {
                    _client = RepositoryClient.CreateClient(connectionSettings);
                });

                OutputToConsole($"Connection successful!", ConsoleColor.Green);
                if (!string.IsNullOrEmpty(repoDefinition))
                {
                    OutputToConsole($"Using repository definition '{ _client.RepositoryDefinitionName }'", ConsoleColor.Green);
                }

                Output($"\r\nPowerDesigner Repository CLI version: { Assembly.GetExecutingAssembly().GetName().Version.ToString(4) }");
                Output($"PowerDesigner Repository Client Library version: { _client.Version }\r\n");

                connected = true;
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
            return connected;
        }

        protected void OnException(Exception ex)
        {
            OutputError(ex.Message);
        }

        protected void Output(string data)
        {
            OutputToConsole(data);
        }

        protected void OutputToConsole(string data, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            _console.BackgroundColor = backgroundColor;
            _console.ForegroundColor = foregroundColor;
            _console.Out.WriteLine(data);
            _console.ResetColor();
        }

        protected void OutputError(string message)
        {
            _console.BackgroundColor = ConsoleColor.Red;
            _console.ForegroundColor = ConsoleColor.White;
            _console.Error.WriteLine(message);
            _console.ResetColor();
        }
    }
}
