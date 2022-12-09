// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Output;
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

        /// <summary>
        /// Creates a connection to the repository.
        /// </summary>
        /// <param name="repoDefinition">The name of the repository definition.</param>
        /// <param name="repoUser">The name of the repository user.</param>
        /// <param name="repoPassword">The password of the repository user.</param>
        /// <returns></returns>
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

                Output("\r\nConnection:\r\n", ConsoleColor.Yellow);
                using (TableWriter writer = new TableWriter(_console, true, padding: 2))
                {
                    writer.StartTable(2);

                    WriteRow(writer, "Status", "connected", ConsoleColor.Gray, ConsoleColor.DarkGreen);
                    WriteRow(writer, "Repository definition", $"'{(string.IsNullOrEmpty(repoDefinition) ? "(none)" : _client.RepositoryDefinitionName)}'", ConsoleColor.Gray, ConsoleColor.DarkGreen);
                    WriteRow(writer, "Repository client library version", _client.Version);

                    writer.WriteTable();
                }
                connected = true;
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
            return connected;
        }

        /// <summary>
        /// Parses the specified array with user or group rights into a <see cref="UserOrGroupRightsEnum"/>.
        /// </summary>
        /// <param name="rights">An array containing user or group rights.</param>
        /// <returns>A <see cref="UserOrGroupRightsEnum"/>.</returns>
        protected UserOrGroupRightsEnum ParseUserOrGroupRights(string[] rights)
        {
            UserOrGroupRightsEnum parsedRights = UserOrGroupRightsEnum.None;
            if (rights != null)
            {
                for (int i = 0; i < rights.Length; i++)
                {
                    if (Enum.TryParse(rights[i], true, out UserOrGroupRightsEnum result))
                    {
                        parsedRights |= result;
                    }
                }
            }
            return parsedRights;
        }

        /// <summary>
        /// Parses the specified permission into a <see cref="PermissionTypeEnum"/>.
        /// </summary>
        /// <param name="permission">The permission string.</param>
        /// <returns>A <see cref="PermissionTypeEnum"/>.</returns>
        protected PermissionTypeEnum ParsePermissionType(string permission)
        {
            PermissionTypeEnum parsedPermission = PermissionTypeEnum.NotSet;
            if (Enum.TryParse(permission, true, out PermissionTypeEnum result))
            {
                parsedPermission = result;
            }
            return parsedPermission;
        }

        #region Output

        protected void OnException(Exception ex)
        {
            OutputError(ex.Message);
        }

        protected void Output(string data)
        {
            Output(data, ConsoleColor.Gray, ConsoleColor.Black);
        }

        protected void Output(string data, ConsoleColor foregroundColor = ConsoleColor.White)
        {
            Output(data, foregroundColor, ConsoleColor.Black);
        }

        protected void OutputUserRightsAndGroupPermissions(Common.User user)
        {
            Output("\r\nUser privileges:\r\n", ConsoleColor.Yellow);
            OutputCSVAsTable(user.Rights, ';', "Rights");

            Output("\r\nGroup memberships:\r\n", ConsoleColor.Yellow);
            OutputCSVAsTable(user.GroupMembership, ';', "Groups");
        }

        protected void OutputTypeAsTable<T>(T source)
        {
            using (TableWriter writer = new TableWriter(_console, padding: 2))
            {
                writer.StartTable(source);
                writer.AddHeaderRow(source, ConsoleColor.Blue);
                writer.AddRow(source);
                writer.WriteTable();
            }
        }

        protected void OutputCSVAsTable(string csvData, char separator, string columnHeader)
        {
            using (TableWriter writer = new TableWriter(_console, padding: 2))
            {
                writer.StartTable(1);
                writer.StartRow(true);
                writer.AddColumn(columnHeader, ConsoleColor.Blue);
                writer.EndRow();

                writer.AddCSVData(csvData, separator);

                writer.WriteTable();
            }
        }

        protected void WriteTableHeader(TableWriter writer, string columnName1, string columnName2, ConsoleColor column1Color = ConsoleColor.Gray, ConsoleColor column2Color = ConsoleColor.Gray)
        {
            writer.StartRow(true);
            writer.AddColumn(columnName1, column1Color);
            writer.AddColumn(columnName2, column2Color);
            writer.EndRow();
        }

        protected void WriteRow<T>(TableWriter writer, string property, T value, ConsoleColor propertyColor = ConsoleColor.Gray, ConsoleColor valueColor = ConsoleColor.Gray)
        {
            writer.StartRow();
            writer.AddColumn(property, propertyColor);
            writer.AddColumn(value.ToString(), valueColor);
            writer.EndRow();
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
            _console.Error.WriteLine(Environment.NewLine);
            _console.Error.WriteLine(message);
            _console.ResetColor();
        }

        #endregion
    }
}
