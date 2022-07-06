﻿using PDRepository.LibraryModels;
using System;
using System.Collections.Generic;

namespace PDRepository.Samples
{
    static class DocumentSamples
    {
        public static void ListDocuments(RepositoryClient client)
        {
            Console.WriteLine("Listing documents...\r\n");

            string rootFolder = "Wholesale&Rural/RDW/PDM/Development/Resources";

            List<Document> docs = client.DocumentClient.ListDocuments(rootFolder);
            docs.ForEach(d => Console.WriteLine($"Name: { d.Name } ({ d.ClassName }) - Version: { d.Version }\r\nObject type: { d.ObjectType }\r\nFrozen: { d.IsFrozen }\r\nLocked: { d.IsLocked }\r\nLocation: { d.Location }\r\nVersion comment: { d.VersionComment }\r\n\r\n"));
        }

        public static void GetDocumentInfo(RepositoryClient client)
        {
            Console.WriteLine("Retrieving document info...\r\n");

            string folder = "Wholesale&Rural/RDW/PDM/Development/Resources/Extended Model Definition";
            string documentName = "DataVault Meta Data Profile";

            Document doc = client.DocumentClient.GetDocumentInfo(folder, documentName);
            if (doc != null)
            {
                Console.WriteLine($"Name: {doc.Name} ({doc.ClassName}) - Version: {doc.Version}\r\nObject type: { doc.ObjectType }\r\nFrozen: { doc.IsFrozen }\r\nLocked: { doc.IsLocked }\r\nLocation: {doc.Location}\r\nExtraction name: { doc.ExtractionFileName }\r\nVersion comment: {doc.VersionComment}\r\n\r\n");
            }
        }

        public static void CheckOutDocument(RepositoryClient client)
        {
            string folder = "Wholesale&Rural/RDW/PDM/Development/Resources/Extended Model Definition";
            string documentName = "DataVault Meta Data Profile";

            //string folder = "Wholesale&Rural/RDW/PDM/Development/CRDW/CRDW_SA";
            //string documentName = "CRDW_SA_StaticData";

            string targetFolder = @"C:\Temp";

            // Check out document with default name

            Console.WriteLine($"Checking out document '{ documentName }' to '{ targetFolder }'...");                        
            
            client.DocumentClient.CheckOutDocument(folder, documentName, targetFolder);

            // Check out same document with alternative name

            string newFileName = "MyProfile.xem";
            Console.WriteLine($"Checking out document '{ documentName }' as '{ newFileName }' to '{ targetFolder }'...");

            client.DocumentClient.CheckOutDocument(folder, documentName, targetFolder, newFileName);
            
            Console.WriteLine("Check-out complete.");
        }

        public static void CheckOutDocumentOtherVersion(RepositoryClient client)
        {
            string folder = "Wholesale&Rural/RDW/PDM/Development/Resources/Extended Model Definition";            
            string documentName = "DataVault Meta Data Profile";
            int documentVersion = 1;
            string targetFolder = @"C:\Temp";

            // Check out document with default name but different version

            Console.WriteLine($"Checking out version '{ documentVersion }' of document '{ documentName }' to '{ targetFolder }'...");
            
            client.DocumentClient.CheckOutDocument(folder, documentName, targetFolder, documentVersion);

            // Check out same document with alternative name and version

            string newFileName = $"MyProfile-v{ documentVersion }.xem";

            Console.WriteLine($"Checking out version '{ documentVersion }' of document '{ documentName }' as '{ newFileName }' to '{ targetFolder }'...");

            client.DocumentClient.CheckOutDocument(folder, documentName, targetFolder, newFileName, documentVersion);

            Console.WriteLine("Check-out complete.");
        }

        public static void CheckOutDocuments(RepositoryClient client)
        {
            string folder = "Wholesale&Rural/RDW/PDM/Development/CRDW/CRDW_SA";
            //string folder = "Wholesale&Rural/RDW/PDM/Development/Resources";
            string targetFolder = @"C:\Temp";
            client.DocumentClient.CheckOutDocuments(folder, targetFolder, false);
        }

    }
}
