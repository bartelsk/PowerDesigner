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

                Console.WriteLine("Connecting...");

                RepositorySettings settings = new RepositorySettings()
                {
                    Password = ConfigurationManager.AppSettings["PDRepoPassword"],
                    User = ConfigurationManager.AppSettings["PDRepoUser"]
                };

                // Start PD and connect to repo
                client = RepositoryClient.CreateClient(settings);

                // Run branch samples
                //BranchSamples.ListBranches(client);
                //BranchSamples.BranchExists(client);

                // Run document samples
                //DocumentSamples.ListDocuments(client);
               // DocumentSamples.GetDocumentInfo(client);
                DocumentSamples.CheckOutDocument(client);

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
