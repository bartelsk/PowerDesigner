﻿// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDRepository;
using PDRepository.Common;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace DocumentSamples
{
    [TestClass]
    public class DocumentSamples
    {
        #region Test init / cleanup

        RepositoryClient client = null;

        [TestInitialize]
        public void TestInit()
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

            Console.WriteLine($"Connection successful - repository definition '{client.RepositoryDefinitionName}'");
            Console.WriteLine($"Client assembly version: {client.Version}\r\n");            
            Console.WriteLine("------------------------------------------------------------\r\n");
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            client?.Dispose();
        }

        #endregion

        /// <summary>
        /// List all documents in a folder.
        /// </summary>      
        [TestMethod]
        public void ListDocuments()
        {
            Console.WriteLine("Listing documents...\r\n");

            string rootFolder = "LibManSamples/Development";
            bool recursive = true;

            List<Document> docs = client.DocumentClient.ListDocuments(rootFolder, recursive);
            docs?.ForEach(d => Console.WriteLine($"Name: {d.Name} ({d.ClassName}) - Version: {d.Version}\r\nObject type: {d.ObjectType}\r\nFrozen: {d.IsFrozen}\r\nLocked: {d.IsLocked}\r\nLocation: {d.Location}\r\nVersion comment: {d.VersionComment}\r\n\r\n"));

            Assert.IsNotNull(docs);
        }

        /// <summary>
        /// Determines whether a document exists.
        /// </summary>      
        [TestMethod]
        public void DocumentExists()
        {
            Console.WriteLine("Checking whether document exists...\r\n");

            string folder = "LibManSamples/Development/Resources";
            string documentName = "settings-git.txt";

            bool exists = client.DocumentClient.DocumentExists(folder, documentName);
            Console.WriteLine($"Document '{documentName}' does{(exists ? string.Empty : " not")} exist in folder '{folder}'.");

            Assert.IsTrue(exists);

            Console.WriteLine("Check complete.");
        }

        /// <summary>
        /// Retrieves document information.
        /// </summary>      
        [TestMethod]
        public void GetDocumentInfo()
        {
            Console.WriteLine("Retrieving document info...\r\n");

            string folder = "LibManSamples/Development/Resources";
            string documentName = "settings-git.txt";

            Document doc = null;
            if (client.DocumentClient.DocumentExists(folder, documentName))
            {
                doc = client.DocumentClient.GetDocumentInfo(folder, documentName);
                if (doc != null)
                {
                    Console.WriteLine($"Name: {doc.Name} ({doc.ClassName}) - Version: {doc.Version}\r\nObject type: {doc.ObjectType}\r\nFrozen: {doc.IsFrozen}\r\nLocked: {doc.IsLocked}\r\nLocation: {doc.Location}\r\nExtraction name: {doc.ExtractionFileName}\r\nVersion comment: {doc.VersionComment}\r\n\r\n");
                }
            }
            else
            {
                Console.WriteLine($"Document '{documentName}' does not exist in folder '{folder}'");
            }
            Assert.IsNotNull(doc);
        }

        /// <summary>
        /// Creates a repository folder.
        /// </summary>
        [TestMethod]
        public void CreateDocumentFolder()
        {
            string parentFolder = "LibManSamples/Development";
            string newFolder = "ResourcesNew";

            Console.WriteLine($"Creating folder '{parentFolder + " / " + newFolder}'...\r\n");
            bool success = client.DocumentClient.CreateFolder(parentFolder, newFolder);
            
            Console.WriteLine($"The folder was {(!success ? "NOT " : string.Empty)}created successfully.");
            Assert.IsTrue(success);
        }

        /// <summary>
        /// Deletes a repository folder.
        /// </summary>
        [TestMethod]
        public void DeleteDocumentFolder()
        {
            string folder = "LibManSamples/Development/Resource";

            Console.WriteLine($"Deleting folder '{folder}'...\r\n");
            bool success = client.DocumentClient.DeleteFolder(folder);

            Console.WriteLine($"The folder was {(!success ? "NOT " : string.Empty)}deleted successfully.");
            Assert.IsTrue(success);
        }

        /// <summary>
        /// Adds a file to the repository. Overwrites the existing document (if any) and freezes it.
        /// </summary>
        [TestMethod]
        public void CheckInFile()
        {
            string folder = "LibManSamples/Development/Resources";
            string fileName = @"C:\Temp\old.txt";
            string fileInExistingBranch = @"C:\Temp\MyModel.pdm";

            // Register event handler
            client.DocumentClient.DocumentCheckedIn += DocumentCheckedIn;

            // Check in single file
            Console.WriteLine($"Checking-in file '{fileName}' in folder '{folder}'...");
            client.DocumentClient.CheckInDocument(folder, fileName, out _);

            // Check in single file that was already part of a repository branch            
            Console.WriteLine($"Checking-in file '{fileInExistingBranch}' in folder '{folder}'...");
            client.DocumentClient.CheckInDocument(folder, fileInExistingBranch, out _);

            Console.WriteLine("Check-in complete.");
        }

        /// <summary>
        /// Adds files to the repository. Overwrites the existing documents (if any) and freezes them.
        /// </summary>
        [TestMethod]
        public void CheckInFiles()
        {
            string repoFolder = "LibManSamples/Development/Resources";
            string sourceFolder = @"C:\Temp\test";

            // Register event handler
            client.DocumentClient.DocumentCheckedIn += DocumentCheckedIn;
                        
            Console.WriteLine($"Checking files in from folder '{sourceFolder}' into repo folder '{repoFolder}'...");
            client.DocumentClient.CheckInDocuments(repoFolder, sourceFolder);

            Console.WriteLine("Check-in complete.");
        }

        /// <summary>
        /// Checks out a document with its default name and an alternative name.
        /// </summary>      
        [TestMethod]
        public void CheckOutDocument()
        {
            string folder = "LibManSamples/Development/PDM";
            string documentName = "MyModel";
            string alternativeFileName = "MyOtherModel.pdm";

            string targetFolder = @"C:\Temp";

            // Register event handler
            client.DocumentClient.DocumentCheckedOut += DocumentCheckedOut;

            // Check out document with default name

            Console.WriteLine($"Checking out document '{documentName}' to '{targetFolder}'...");
            client.DocumentClient.CheckOutDocument(folder, documentName, targetFolder);

            // Check out same document with alternative name

            Console.WriteLine($"Checking out document '{documentName}' as '{alternativeFileName}' to '{targetFolder}'...");
            client.DocumentClient.CheckOutDocument(folder, documentName, targetFolder, alternativeFileName);

            Console.WriteLine("Check-out complete.");
        }

        /// <summary>
        /// Checks out a specific version of a document. 
        /// </summary>      
        [TestMethod]
        public void CheckOutDocumentOtherVersion()
        {
            string folder = "LibManSamples/Development/PDM";
            string documentName = "MyModel";
            string targetFolder = @"C:\Temp";

            int documentVersion = 2;
            string documentVersionFileName = $"MyModel-v{documentVersion}.pdm";

            // Register event handler
            client.DocumentClient.DocumentCheckedOut += DocumentCheckedOut;

            // Check out document with default name but different version

            Console.WriteLine($"Checking out version '{documentVersion}' of document '{documentName}' to '{targetFolder}'...");
            client.DocumentClient.CheckOutDocument(folder, documentName, targetFolder, documentVersion);

            // Check out same document with alternative name and version           

            Console.WriteLine($"Checking out version '{documentVersion}' of document '{documentName}' as '{documentVersionFileName}' to '{targetFolder}'...");
            client.DocumentClient.CheckOutDocument(folder, documentName, targetFolder, documentVersionFileName, documentVersion);

            Console.WriteLine("Check-out complete.");
        }

        /// <summary>
        /// Checks out all documents in a folder.
        /// </summary>      
        [TestMethod]
        public void CheckOutDocuments()
        {
            string folder = "LibManSamples/Development";
            string targetFolder = @"C:\Temp\CheckOutTest\SingleRepoFolder";

            // Register event handler            
            client.DocumentClient.DocumentCheckedOut += DocumentCheckedOut;

            Console.WriteLine($"Checking out all document in folder '{folder}' into '{targetFolder}' (no sub-folders)...");
            client.DocumentClient.CheckOutDocuments(folder, targetFolder, false, false);
        }

        /// <summary>
        /// Checks out all documents in a folder (and any sub folder of that folder) into a single folder on disc.
        /// </summary>      
        [TestMethod]
        public void CheckOutDocumentsRecursively()
        {
            string folder = "LibManSamples/Development";
            string targetFolder = @"C:\Temp\CheckOutTest\SingleTargetFolder";

            // Register event handler
            client.DocumentClient.DocumentCheckedOut += DocumentCheckedOut;

            Console.WriteLine($"Checking out all document in folder '{folder}' and its sub-folders (if any) into '{targetFolder}'...");
            client.DocumentClient.CheckOutDocuments(folder, targetFolder, true, false);
        }

        /// <summary>
        /// Checks out all documents in a folder (and any sub folder of that folder) while preserving the repository folder structure on disc.
        /// </summary>      
        [TestMethod]
        public void CheckOutDocumentsRecursivelyMimicingRepoStructure()
        {
            string folder = "LibManSamples/Development";
            string targetFolder = @"C:\Temp\CheckOutTest\MimicRepoStructure";

            // Register event handler
            client.DocumentClient.DocumentCheckedOut += DocumentCheckedOut;

            Console.WriteLine($"Checking out all document in folder '{folder}' and its sub-folders (if any) into '{targetFolder}' (mimicing repo folder structure)...");
            client.DocumentClient.CheckOutDocuments(folder, targetFolder, true, true);
        }

        /// <summary>
        /// Freezes a document.
        /// </summary>      
        [TestMethod]
        public void FreezeDocument()
        {
            string folder = "LibManSamples/Development/Resources";
            string documentName = "settings-git.txt";
            string freezeComment = "New version freeze.";

            Console.WriteLine($"Freezing document '{documentName}' in folder '{folder}'...");

            bool success = client.DocumentClient.FreezeDocument(folder, documentName, freezeComment);
            Console.WriteLine($"The document was {(!success ? "NOT " : string.Empty)}frozen successfully.");
        }

        /// <summary>
        /// Unfreezes a document.
        /// </summary>      
        [TestMethod]
        public void UnfreezeDocument()
        {
            string folder = "LibManSamples/Development/Resources";
            string documentName = "settings-git.txt";

            Console.WriteLine($"Unfreezing document '{documentName}' in folder '{folder}'...");

            bool success = client.DocumentClient.UnfreezeDocument(folder, documentName);
            Console.WriteLine($"The document was {(!success ? "NOT " : string.Empty)}successfully unfrozen.");
        }

        /// <summary>
        /// Locks a document.
        /// </summary>      
        [TestMethod]
        public void LockDocument()
        {
            string folder = "LibManSamples/Development/Resources";
            string documentName = "settings-git.txt";
            string lockComment = "Locking for maintenance.";

            Console.WriteLine($"Locking document '{documentName}' in folder '{folder}'...");

            bool success = client.DocumentClient.LockDocument(folder, documentName, lockComment);
            Console.WriteLine($"The document was {(!success ? "NOT " : string.Empty)}locked successfully.");
        }

        /// <summary>
        /// Unlocks a document.
        /// </summary>      
        [TestMethod]
        public void UnlockDocument()
        {
            string folder = "LibManSamples/Development/Resources";
            string documentName = "settings-git.txt";

            Console.WriteLine($"Unlocking document '{documentName}' in folder '{folder}'...");

            bool success = client.DocumentClient.UnlockDocument(folder, documentName);
            Console.WriteLine($"The document was {(!success ? "NOT " : string.Empty)}unlocked successfully.");
        }

        /// <summary>
        /// Deletes a document.
        /// </summary>      
        [TestMethod]
        public void DeleteDocument()
        {
            string folder = "LibManSamples/Development/Resources";
            string documentName = "old.txt";

            Console.WriteLine($"Deleting document '{documentName}' in folder '{folder}'...");

            bool success = client.DocumentClient.DeleteDocument(folder, documentName);
            Console.WriteLine($"The document was {(!success ? "NOT " : string.Empty)}deleted successfully.");
        }

        /// <summary>
        /// Deletes a document version.
        /// </summary>      
        [TestMethod]
        public void DeleteDocumentVersion()
        {
            string folder = "LibManSamples/Development/Resources";
            string documentName = "old.txt";

            Console.WriteLine($"Deleting current version of document '{documentName}' in folder '{folder}'...");

            bool success = client.DocumentClient.DeleteDocumentVersion(folder, documentName);
            Console.WriteLine($"The document version was {(!success ? "NOT " : string.Empty)}deleted successfully.");
        }

        /// <summary>
        /// Retrieves the permission of a document for a specific user or group.
        /// </summary>      
        [TestMethod]
        public void GetDocumentPermissions()
        {
            string folder = "LibManSamples/Development";
            string documentName = "Microsoft SQL Server 2014";
            string userOrGroupName = "HR";

            Console.WriteLine("Checking document permission...");

            PermissionTypeEnum permission = client.DocumentClient.GetPermission(folder, documentName, userOrGroupName);
            Console.WriteLine($"The permission of user or group '{userOrGroupName}' on document '{documentName}' is: '{permission}'");
        }

        /// <summary>
        /// Grants permissions to a document for a specific user or group.
        /// </summary>      
        [TestMethod]
        public void SetDocumentPermission()
        {
            string folder = "LibManSamples/Development";
            string documentName = "Microsoft SQL Server 2014";

            // Grant the HR group Read permission to the specified document 
            Permission permission = new Permission()
            {
                CopyToChildren = false,
                PermissionType = PermissionTypeEnum.Read,
                UserOrGroupName = "HR"
            };

            Console.WriteLine("Setting document permission...");

            bool success = client.DocumentClient.SetPermission(folder, documentName, permission);
            Console.WriteLine($"The permission for user or group '{permission.UserOrGroupName}' on document '{documentName}' was {(!success ? "NOT " : string.Empty)}set successfully to '{permission.PermissionType}'.");
        }

        /// <summary>
        /// Deletes permissions from a document for a specific user or group.
        /// </summary>      
        [TestMethod]
        public void DeleteDocumentPermission()
        {
            string folder = "LibManSamples/Development";
            string documentName = "Microsoft SQL Server 2014";

            // Remove the HR group permission from the specified document 
            Permission permission = new Permission()
            {
                CopyToChildren = false,
                UserOrGroupName = "HR"
            };

            Console.WriteLine("Removing document permission...");

            bool success = client.DocumentClient.DeletePermission(folder, documentName, permission);
            Console.WriteLine($"The permission for user or group '{permission.UserOrGroupName}' on document '{documentName}' was {(!success ? "NOT " : string.Empty)}removed successfully.");
        }

        private static void DocumentCheckedIn(object sender, CheckInEventArgs e)
        {
            Console.WriteLine($"Checked in document '{e.DocumentName}' version '{e.DocumentVersion}' from file '{e.CheckInFileName}' into folder '{e.DocumentFolder}'.");
        }

        private static void DocumentCheckedOut(object sender, CheckOutEventArgs e)
        {
            Console.WriteLine($"Checked out document '{e.DocumentName}' version '{e.DocumentVersion}' to file '{e.CheckOutFileName}'.");
        }
    }
}
