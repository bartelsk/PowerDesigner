// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.Common;
using System;
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

                Output("\r\nConnection:\r\n", ConsoleColor.Magenta);
                Output("  Status: connected", ConsoleColor.DarkGreen);
                Output($"  Repository definition: '{ (string.IsNullOrEmpty(repoDefinition) ? "(none)" : _client.RepositoryDefinitionName) }'", ConsoleColor.DarkGreen);
                Output($"  Repository client library version: { _client.Version }\r\n", ConsoleColor.DarkGreen);

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
            Output(data, ConsoleColor.White, ConsoleColor.Black);
        }

        protected void Output(string data, ConsoleColor foregroundColor = ConsoleColor.White)
        {
            Output(data, foregroundColor, ConsoleColor.Black);
        }

        protected void OutputTableRow<T>(string property, T value, int tabCount = 2, ConsoleColor foregroundColor = ConsoleColor.White)
        {
            Output($"  { property }{ new String('\t', tabCount) }{ value }", foregroundColor);
        }

        protected void OutputTableRowCSV(string data, string separator, ConsoleColor foregroundColor = ConsoleColor.White)
        {
            Output($"  { data.Replace(separator, "\r\n  ") }", foregroundColor);
        }

        protected void OutputTableRowSeparator(char separator, int amount)
        {
            Output($"  { new String(separator, amount) }");
        }

        protected void Output(string data, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
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
