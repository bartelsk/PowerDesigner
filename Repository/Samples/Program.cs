﻿// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using System;
using System.Configuration;

namespace PDRepository.Samples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RepositoryClient client = null;
            try
            {
                Console.WriteLine("PowerDesigner Repository Client");
                Console.WriteLine("===============================\r\n");

                // Get repository connection settings
                ConnectionSettings connectionSettings = new ConnectionSettings()
                {
                    Password = ConfigurationManager.AppSettings["PDRepoPassword"],
                    RepositoryDefinition = ConfigurationManager.AppSettings["PDRepoDefinition"],
                    User = ConfigurationManager.AppSettings["PDRepoUser"]
                };

                // Start PowerDesigner and connect to the repository
                Console.WriteLine("Connecting...");
                client = RepositoryClient.CreateClient(connectionSettings);

                Console.WriteLine($"Connection successful - repository definition '{ client.RepositoryDefinitionName }'");
                Console.WriteLine($"Client assembly version: { client.Version }\r\n");

                // Run branch samples
                //BranchSamples.ListBranches(client);
                //BranchSamples.ListBranchesWithPermissions(client);
                //BranchSamples.BranchExists(client);
                //BranchSamples.CreateBranch(client);
                //BranchSamples.CreateBranchWithPermissions(client);
                //BranchSamples.GetBranchPermissions(client);
                //BranchSamples.SetBranchPermission(client);
                //BranchSamples.DeleteBranchPermission(client);

                // Run document samples             
                //DocumentSamples.ListDocuments(client);
                //DocumentSamples.DocumentExists(client);
                //DocumentSamples.GetDocumentInfo(client);
                //DocumentSamples.CheckInFile(client);
                //DocumentSamples.CheckOutDocument(client);
                //DocumentSamples.CheckOutDocumentOtherVersion(client);
                //DocumentSamples.CheckOutDocuments(client);
                //DocumentSamples.CheckOutDocumentsRecursively(client);
                //DocumentSamples.CheckOutDocumentsRecursivelyMimicingRepoStructure(client);
                //DocumentSamples.FreezeDocument(client);
                //DocumentSamples.UnfreezeDocument(client);
                //DocumentSamples.LockDocument(client);
                //DocumentSamples.UnlockDocument(client);
                //DocumentSamples.DeleteDocument(client);
                //DocumentSamples.DeleteDocumentVersion(client);
                //DocumentSamples.GetDocumentPermissions(client);
                //DocumentSamples.SetDocumentPermission(client);
                //DocumentSamples.DeleteDocumentPermission(client);

                // Run user / group samples                
                //UserAndGroupSamples.ListUsers(client);
                //UserAndGroupSamples.UserExists(client);
                //UserAndGroupSamples.CreateUser(client);
                //UserAndGroupSamples.DeleteUser(client);
                //UserAndGroupSamples.CreateUserAndAddToGroup(client);

                //UserAndGroupSamples.ListGroups(client);
                //UserAndGroupSamples.GroupExists(client);
                //UserAndGroupSamples.CreateGroup(client);
                //UserAndGroupSamples.GetGroupRights(client);
                //UserAndGroupSamples.DeleteGroup(client);

                UserAndGroupSamples.GetUserGroups(client);

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            finally
            {
                client?.Dispose();
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }
}
