using PDRepository.LibraryModels;
using System;
using System.Collections.Generic;

namespace PDRepository.Samples
{
    static class BranchSamples
    {
        public static void ListBranches(RepositoryClient client)
        {
            Console.WriteLine("Listing branches...");

            string rootFolder = "Wholesale&Rural";

            List<Branch> branches = client.BranchClient.ListBranches(rootFolder);
            branches.ForEach(b => Console.WriteLine($"Branch: { b.Name } - Relative path: { b.RelativePath }"));
        }
    }
}
