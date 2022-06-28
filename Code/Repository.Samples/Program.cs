using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDRepository.Samples
{
    internal class Program
    {
        static async void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Creating client");

                RepositorySettings settings = new RepositorySettings()
                {
                    Password = "1234",
                    User = "john"
                };

                using (var client = RepositoryClient.CreateClient(settings))
                {
                    //client.connect
                    await BranchTests(client);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }

        private static async Task BranchTests(RepositoryClient client)
        {
            Console.WriteLine("Starting branch test");

            string repoPath = "hello";
            List<string> branches = await client.BranchClient.ListBranches(repoPath);
            branches.ForEach(b => Console.WriteLine($"\r\nName: { b }"));

        }
    }
}
