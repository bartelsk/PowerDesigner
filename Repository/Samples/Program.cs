// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
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

                RepositorySettings settings = new RepositorySettings()
                {
                    Password = ConfigurationManager.AppSettings["PDRepoPassword"],
                    User = ConfigurationManager.AppSettings["PDRepoUser"]
                };

                // Start PD and connect to repo
                Console.WriteLine("Connecting...");
                client = RepositoryClient.CreateClient(settings);

                // Run branch samples
                //BranchSamples.ListBranches(client);
                //BranchSamples.ListBranchesWithPermissions(client);
                //BranchSamples.BranchExists(client);
                //BranchSamples.CreateBranch(client);
                //BranchSamples.CreateBranchWithPermissions(client);

                // Run document samples             
                //DocumentSamples.ListDocuments(client);
                //DocumentSamples.DocumentExists(client);
                //DocumentSamples.GetDocumentInfo(client);
                //DocumentSamples.CheckOutDocument(client);
                //DocumentSamples.CheckOutDocumentOtherVersion(client);
                //DocumentSamples.CheckOutDocuments(client);
                //DocumentSamples.CheckOutDocumentsRecursively(client);
                //DocumentSamples.CheckOutDocumentsRecursivelyMimicingRepoStructure(client);
                //DocumentSamples.FreezeDocument(client);
                //DocumentSamples.UnfreezeDocument(client);
                DocumentSamples.LockDocument(client);
                DocumentSamples.UnlockDocument(client);
                //DocumentSamples.GetDocumentPermissions(client);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception: {ex.Message}");
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
