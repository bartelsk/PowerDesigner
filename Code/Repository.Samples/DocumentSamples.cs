using PDRepository.LibraryModels;
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
                Console.WriteLine($"Name: {doc.Name} ({doc.ClassName}) - Version: {doc.Version}\r\nObject type: { doc.ObjectType }\r\nFrozen: { doc.IsFrozen }\r\nLocked: { doc.IsLocked }\r\nLocation: {doc.Location}\r\nVersion comment: {doc.VersionComment}\r\n\r\n");
            }
        }

        public static void CheckOutDocument(RepositoryClient client)
        {
            string folder = "Wholesale&Rural/RDW/PDM/Development/Resources/Extended Model Definition";
            string documentName = "DataVault Meta Data Profile";
            string targetFile = @"C:\Temp\DV.xem";
            string targetFileVersion1 = @"C:\Temp\DV-v1.xem";

            Console.WriteLine($"Checking out document '{ documentName }' to '{ targetFile }'...");

            // Get current document version
            Document info = client.DocumentClient.GetDocumentInfo(folder, documentName);
            if (info != null)
            {
                Console.WriteLine($"Current version: { info.Version }");
            }
            
            // Check out current version of the document
            client.DocumentClient.CheckOutDocument(folder, documentName, targetFile);

            Console.WriteLine($"Checking out version 1 of document '{ documentName }' to '{ targetFileVersion1 }'...");

            // Check out version 1 of the document
            client.DocumentClient.CheckOutDocument(folder, documentName, targetFileVersion1, 1);

            Console.WriteLine("Check-out complete.");
        }

        public static void CheckOutModel(RepositoryClient client)
        {
            string folder = "Wholesale&Rural/RDW/PDM/Development/CRDW/CRDW_SA";
            string documentName = "CRDW_SA_StaticData";
            string targetFile = @"C:\Temp\CRDW_SA_StaticData.pdm";            

            Console.WriteLine($"Checking out model '{ documentName }' to '{ targetFile }'...");
                        
            // Check out current version of the document
            client.DocumentClient.CheckOutDocument(folder, documentName, targetFile);
            
            Console.WriteLine("Check-out complete.");
        }

        public static void CheckOutDocuments(RepositoryClient client)
        {
            string folder = "Wholesale&Rural/RDW/PDM/Development/CRDW/CRDW_SA";
            //string folder = "Wholesale&Rural/RDW/PDM/Development/Resources";
            string targetFile = @"C:\Temp";
            client.DocumentClient.CheckOutDocuments(folder, targetFile, false);
        }

    }
}
