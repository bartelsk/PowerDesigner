// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;
using PDRepository.CLI.Utils;
using PDRepository.Common;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PDRepository.CLI
{
   abstract class CmdBase
   {
      protected IConsole _console;
      protected RepositoryClient _client = null;
      protected ConnectionSettings _connectionSettings;

      protected virtual Task<int> OnExecute(CommandLineApplication app)
      {
         return Task.FromResult(0);
      }

      /// <summary>
      /// Creates a connection to the repository based on persisted repository connection information.
      /// </summary>      
      protected async Task<bool> ConnectAsync()
      {
         bool connected = false;
         try
         {
            if (ConnectionSettingsExist)
            {
               Output("\r\nConnecting...");

               LoadConnectionSettings();
               await Task.Run(() =>
               {
                  _client = RepositoryClient.CreateClient(_connectionSettings);
               });

               Output("\r\nConnection:\r\n", ConsoleColor.Yellow);
               using (TableWriter writer = new TableWriter(_console, true, padding: 2))
               {
                  writer.StartTable(2);

                  WriteRow(writer, "Status", "connected", ConsoleColor.Gray, ConsoleColor.DarkGreen);
                  WriteRow(writer, "Repository definition", $"'{(string.IsNullOrEmpty(_client.RepositoryDefinitionName) ? "(none)" : _client.RepositoryDefinitionName)}'", ConsoleColor.Gray, ConsoleColor.DarkGreen);
                  WriteRow(writer, "Repository client library version", _client.Version);

                  writer.WriteTable();
               }
               connected = true;
            }
            else
            {
               OutputError("Not authorized, please create a connection profile using the Auth LogIn command.");
            }
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

      #region Connection settings

      protected string SettingsFolder
      {
         get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.Create)}\\.pdr\\"; }
      }

      protected string SettingsFileName
      {
         get { return $"{SettingsFolder}default.settings"; }
      }

      protected bool ConnectionSettingsExist
      {
         get { return File.Exists(SettingsFileName); }
      }     

      protected void CreateConnectionSettingsFolder()
      {
         if (!Directory.Exists(SettingsFolder))
         {
            Directory.CreateDirectory(SettingsFolder);
         }
      }

      protected void DeleteConnectionProfile()
      {
         if (File.Exists(SettingsFileName))
         {
            File.Delete(SettingsFileName);
         }
      }

      protected void LoadConnectionSettings()
      {        
         using (FileStream stream = File.Open(SettingsFileName, FileMode.Open))
         {
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, false))
            {
               string profileData = Security.Decrypt(reader.ReadString());
               if (!string.IsNullOrEmpty(profileData))
               {
                  string[] profileDataSegments = profileData.Split(';');
                  _connectionSettings = new ConnectionSettings()
                  {
                     RepositoryDefinition = profileDataSegments[0],
                     User = profileDataSegments[1],
                     Password = profileDataSegments[2]
                  };                     
               }
            }
         }         
      }

      protected bool SaveConnectionSettings(ConnectionSettings settings)
      {
         bool result = false;
         CreateConnectionSettingsFolder();
         using (FileStream stream = File.Open(SettingsFileName, FileMode.Create))
         {
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
               writer.Write(Security.Encrypt($"{settings.RepositoryDefinition};{settings.User};{settings.Password}"));
            }
            result = true;
         }
         return result;
      }

      #endregion

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

      protected void OutputNewLine(string data, ConsoleColor foregroundColor = ConsoleColor.White)
      {
         Output(string.Concat("\r\n", data), foregroundColor, ConsoleColor.Black);
      }

      protected void OutputUserRightsAndGroupPermissions(Common.User user)
      {
         OutputNewLine("User privileges:\r\n", ConsoleColor.Yellow);
         OutputCSVAsTable(user.Rights, ';', "Rights");

         OutputNewLine("Group memberships:\r\n", ConsoleColor.Yellow);
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
         _console.Error.Write(Environment.NewLine);
         _console.Error.WriteLine(message);
         _console.ResetColor();
      }

      #endregion
   }
}
