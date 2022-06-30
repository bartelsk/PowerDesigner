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

      public static void GetDocumentInfo(RepositoryClient client)
      {
         Console.WriteLine("Retrieving document info...");

         string folder = "Wholesale&Rural/RDW/PDM/Development/Resources/Configuration";
         string documentName = "hello.xml";

         Document doc = client.DocumentClient.GetDocumentInfo(folder, documentName);
         if (doc != null)
         {
            Console.WriteLine($"Name: {doc.Name} ({doc.ClassName}) - Version: {doc.Version}\r\nLocation: {doc.Location}\r\nVersion comment: {doc.VersionComment}\r\n\r\n");
         }
      }
    }
}
