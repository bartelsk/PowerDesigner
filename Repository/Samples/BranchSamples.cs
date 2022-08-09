// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using System;
using System.Collections.Generic;

namespace PDRepository.Samples
{
    static class BranchSamples
    {
        public static void ListBranches(RepositoryClient client)
        {
            Console.WriteLine("Listing branches...\r\n");

            string rootFolder = "Wholesale&Rural";

            List<Branch> branches = client.BranchClient.ListBranches(rootFolder);
            branches.ForEach(b => Console.WriteLine($"Branch: { b.Name } - Relative path: { b.RelativePath }"));
        }

        public static void BranchExists(RepositoryClient client)
        {
            Console.WriteLine("Testing branch existence...\r\n");

            string rootFolder = "Wholesale&Rural";
            string branchName = "Development";

            bool exists = client.BranchClient.BranchExists(rootFolder, branchName);
            Console.WriteLine($"Branch '{ branchName }' " + ((exists) ? "exists." : "does not exist."));
        }
    }
}
