using PDRepository.LibraryModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                                    
                // Run branch test
                BranchTests(client);
                
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

        private static void BranchTests(RepositoryClient client)
        {
            Console.WriteLine("Starting branch test");

            string rootFolder = "Wholesale&Rural";
            List<Branch> branches = client.BranchClient.ListBranches(rootFolder);
            branches.ForEach(b => Console.WriteLine($"Branch: { b.Name } - Relative path: { b.RelativePath }"));

        }
    }
}
