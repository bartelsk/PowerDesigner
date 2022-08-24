// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using System;
using System.Collections.Generic;

namespace PDRepository.Samples
{
    static class DocumentSamples
    {
        /// <summary>
        /// List all documents in a folder.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void ListDocuments(RepositoryClient client)
        {
            Console.WriteLine("Listing documents...\r\n");

            string rootFolder = "LibManSamples/Development";
            bool recursive = true; 

            List<Document> docs = client.DocumentClient.ListDocuments(rootFolder, recursive);
            docs.ForEach(d => Console.WriteLine($"Name: { d.Name } ({ d.ClassName }) - Version: { d.Version }\r\nObject type: { d.ObjectType }\r\nFrozen: { d.IsFrozen }\r\nLocked: { d.IsLocked }\r\nLocation: { d.Location }\r\nVersion comment: { d.VersionComment }\r\n\r\n"));
        }

        /// <summary>
        /// Determines whether a document exists.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void DocumentExists(RepositoryClient client)
        {
            Console.WriteLine("Checking whether document exists...\r\n");

            string folder = "LibManSamples/Development/Resources";
            string documentName = "settings-git.txt";

            bool exists = client.DocumentClient.DocumentExists(folder, documentName);
            Console.WriteLine($"Document '{ documentName }' does{ (exists ? string.Empty : " not") } exist in folder '{ folder }'.");

            Console.WriteLine("Check complete.");
        }

        /// <summary>
        /// Retrieves document information.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void GetDocumentInfo(RepositoryClient client)
        {
            Console.WriteLine("Retrieving document info...\r\n");

            string folder = "LibManSamples/Development/Resources";
            string documentName = "settings-git.txt";

            Document doc = client.DocumentClient.GetDocumentInfo(folder, documentName);
            if (doc != null)
            {
                Console.WriteLine($"Name: {doc.Name} ({doc.ClassName}) - Version: {doc.Version}\r\nObject type: { doc.ObjectType }\r\nFrozen: { doc.IsFrozen }\r\nLocked: { doc.IsLocked }\r\nLocation: {doc.Location}\r\nExtraction name: { doc.ExtractionFileName }\r\nVersion comment: {doc.VersionComment}\r\n\r\n");
            }
        }

        /// <summary>
        /// Checks out a document with its default name and an alternative name.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void CheckOutDocument(RepositoryClient client)
        {
            string folder = "Wholesale&Rural/RDW/PDM/Development/CRDW/CRDW_SA";
            string documentName = "CRDW_SA_StaticData";
            string alternativeFileName = "CRDW_SA_StaticData_alt.pdm";

            string targetFolder = @"C:\Temp";

            // Register event handler
            client.DocumentClient.DocumentCheckedOut += DocumentCheckedOut;

            // Check out document with default name

            Console.WriteLine($"Checking out document '{ documentName }' to '{ targetFolder }'...");            
            client.DocumentClient.CheckOutDocument(folder, documentName, targetFolder);
            
            // Check out same document with alternative name

            Console.WriteLine($"Checking out document '{ documentName }' as '{ alternativeFileName }' to '{ targetFolder }'...");            
            client.DocumentClient.CheckOutDocument(folder, documentName, targetFolder, alternativeFileName);
            
            Console.WriteLine("Check-out complete.");
        }

        /// <summary>
        /// Checks out a specific version of a document. 
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void CheckOutDocumentOtherVersion(RepositoryClient client)
        {
            string folder = "Wholesale&Rural/RDW/PDM/Development/Resources/Extended Model Definition";            
            string documentName = "DataVault Meta Data Profile";
            int documentVersion = 1;
            string targetFolder = @"C:\Temp";

            // Register event handler
            client.DocumentClient.DocumentCheckedOut += DocumentCheckedOut;

            // Check out document with default name but different version

            Console.WriteLine($"Checking out version '{ documentVersion }' of document '{ documentName }' to '{ targetFolder }'...");            
            client.DocumentClient.CheckOutDocument(folder, documentName, targetFolder, documentVersion);

            // Check out same document with alternative name and version

            string newFileName = $"MyProfile-v{ documentVersion }.xem";

            Console.WriteLine($"Checking out version '{ documentVersion }' of document '{ documentName }' as '{ newFileName }' to '{ targetFolder }'...");
            client.DocumentClient.CheckOutDocument(folder, documentName, targetFolder, newFileName, documentVersion);

            Console.WriteLine("Check-out complete.");
        }

        /// <summary>
        /// Checks out all documents in a folder.
        /// </summary>
        /// <param name="client"></param>
        public static void CheckOutDocuments(RepositoryClient client)
        {            
            string folder = "Wholesale&Rural/RDW/PDM/Development/Resources";
            string targetFolder = @"C:\Temp\CheckOutTest\SingleRepoFolder";

            // Register event handler            
            client.DocumentClient.DocumentCheckedOut += DocumentCheckedOut;

            Console.WriteLine($"Checking out all document in folder '{ folder }' into '{ targetFolder }' (no sub-folders)...");            
            client.DocumentClient.CheckOutDocuments(folder, targetFolder, false, false);
        }        

        /// <summary>
        /// Checks out all documents in a folder (and any sub folder of that folder) into a single folder on disc.
        /// </summary>
        /// <param name="client"></param>
        public static void CheckOutDocumentsRecursively(RepositoryClient client)
        {            
            string folder = "Wholesale&Rural/RDW/PDM/Development/Resources";
            string targetFolder = @"C:\Temp\CheckOutTest\SingleTargetFolder";

            // Register event handler
            client.DocumentClient.DocumentCheckedOut += DocumentCheckedOut;

            Console.WriteLine($"Checking out all document in folder '{ folder }' and its sub-folders (if any) into '{ targetFolder }'...");            
            client.DocumentClient.CheckOutDocuments(folder, targetFolder, true, false);
        }

        /// <summary>
        /// Checks out all documents in a folder (and any sub folder of that folder) while preserving the repository folder structure on disc.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void CheckOutDocumentsRecursivelyMimicingRepoStructure(RepositoryClient client)
        {            
            string folder = "Wholesale&Rural/RDW/PDM/Development/Resources";
            string targetFolder = @"C:\Temp\CheckOutTest\MimicRepoStructure";

            // Register event handler
            client.DocumentClient.DocumentCheckedOut += DocumentCheckedOut;

            Console.WriteLine($"Checking out all document in folder '{ folder }' and its sub-folders (if any) into '{ targetFolder }' (mimicing repo folder structure)...");
            client.DocumentClient.CheckOutDocuments(folder, targetFolder, true, true);
        }

        /// <summary>
        /// Freezes a document.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void FreezeDocument(RepositoryClient client)
        {
            string folder = "LibManSamples/Development/Resources";
            string documentName = "settings-git.txt";
            string freezeComment = "New version freeze.";

            Console.WriteLine($"Freezing document '{ documentName }' in folder '{ folder }'...");

            bool success = client.DocumentClient.FreezeDocument(folder, documentName, freezeComment);
            Console.WriteLine($"The document was { (!success ? "NOT " : string.Empty) }frozen successfully.");
        }

        /// <summary>
        /// Unfreezes a document.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void UnfreezeDocument(RepositoryClient client)
        {
            string folder = "LibManSamples/Development/Resources";
            string documentName = "settings-git.txt";            

            Console.WriteLine($"Unfreezing document '{ documentName }' in folder '{ folder }'...");

            bool success = client.DocumentClient.UnfreezeDocument(folder, documentName);
            Console.WriteLine($"The document was { (!success ? "NOT " : string.Empty) }successfully unfrozen.");
        }      

        /// <summary>
        /// Locks a document.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void LockDocument(RepositoryClient client)
        {
            string folder = "LibManSamples/Development/Resources";
            string documentName = "settings-git.txt";
            string lockComment = "Locking for maintenance.";

            Console.WriteLine($"Locking document '{ documentName }' in folder '{ folder }'...");

            bool success = client.DocumentClient.LockDocument(folder, documentName, lockComment);
            Console.WriteLine($"The document was { (!success ? "NOT " : string.Empty) }locked successfully.");
        }

        /// <summary>
        /// Unlocks a document.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void UnlockDocument(RepositoryClient client)
        {
            string folder = "LibManSamples/Development/Resources";
            string documentName = "settings-git.txt";            

            Console.WriteLine($"Unlocking document '{ documentName }' in folder '{ folder }'...");

            bool success = client.DocumentClient.UnlockDocument(folder, documentName);
            Console.WriteLine($"The document was { (!success ? "NOT " : string.Empty) }unlocked successfully.");
        }

        /// <summary>
        /// Retrieves the permission of a document for a specific user or group.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void GetDocumentPermissions(RepositoryClient client)
        {
            string folder = "LibManSamples/Development";
            string documentName = "Microsoft SQL Server 2014";
            string userOrGroupName = "HR";

            Console.WriteLine("Checking document permission...");

            PermissionTypeEnum permission = client.DocumentClient.GetPermission(folder, documentName, userOrGroupName);
            Console.WriteLine($"The permission of user or group '{ userOrGroupName }' on document '{ documentName }' is: '{ permission }'");
        }

        private static void DocumentCheckedOut(object sender, CheckOutEventArgs e)
        {
            Console.WriteLine($"Checked out document '{ e.DocumentName }' to file '{ e.CheckOutFileName }'");
        }
    }
}
