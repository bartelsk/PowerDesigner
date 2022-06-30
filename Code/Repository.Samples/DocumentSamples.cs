using PDRepository.LibraryModels;
using System;
using System.Collections.Generic;

namespace PDRepository.Samples
{
    static class DocumentSamples
    {
        public static void ListDocuments(RepositoryClient client)
        {
            Console.WriteLine("Listing documents...");

            string rootFolder = "Wholesale&Rural/RDW/PDM/Development/Resources/Configuration";

            List<Document> docs = client.DocumentClient.ListDocuments(rootFolder);            
            docs.ForEach(d => Console.WriteLine($"Name: { d.Name } ({ d.ClassName }) - Version: { d.Version }\r\nLocation: { d.Location }\r\nVersion comment: { d.VersionComment }\r\n\r\n"));
        }
    }
}
