// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using PDRepository.Exceptions;
using System;
using System.Collections.Generic;

namespace PDRepository.Samples
{
    static class BranchSamples
    {
        /// <summary>
        /// List all branches
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void ListBranches(RepositoryClient client)
        {
            Console.WriteLine("Listing branches...\r\n");

            string rootFolder = "LibManSamples";

            List<Branch> branches = client.BranchClient.ListBranches(rootFolder);
            branches.ForEach(b => Console.WriteLine($"Branch: { b.Name } - Relative path: { b.RelativePath }"));
        }

        /// <summary>
        /// List all branches based on access permissions.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void ListBranchesWithPermissions(RepositoryClient client)
        {
            string rootFolder = "LibManSamples";
            string userLogIn = "BartelsK";

            Console.WriteLine($"Listing branches for user '{ userLogIn }'...\r\n");

            List<Branch> branches = client.BranchClient.ListBranches(rootFolder, userLogIn);
            branches.ForEach(b => Console.WriteLine($"Branch: { b.Name } - Relative path: { b.RelativePath } - Permission: { b.Permission }"));
        }

        /// <summary>
        /// Checks whether a branch exists.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void BranchExists(RepositoryClient client)
        {
            Console.WriteLine("Testing branch existence...\r\n");

            string repoFolder = "LibManSamples";
            string branchName = "Development";

            bool exists = client.BranchClient.BranchExists(repoFolder, branchName);
            Console.WriteLine($"Branch '{ branchName }' " + ((exists) ? "exists." : "does not exist."));
        }

        /// <summary>
        /// Creates a new branch of an existing branch.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void CreateBranch(RepositoryClient client)
        {
            try
            {
                string sourceBranchFolder = "LibManSamples/Development";
                string newBranchName = "MyNewDevelopmentBranch";

                Console.WriteLine($"Creating branch '{ newBranchName }' of source branch '{ sourceBranchFolder }'...\r\n");

                client.BranchClient.CreateBranch(sourceBranchFolder, newBranchName);
            }
            catch (RepositoryException re)
            {
                Console.WriteLine(re.Message);                
            }
            catch (Exception)
            {
                throw;
            }            
        }

        /// <summary>
        /// Creates a new branch of an existing branch and applies specific branch permissions.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void CreateBranchWithPermissions(RepositoryClient client)
        {
            try
            {
                string sourceBranchFolder = "LibManSamples/Development";
                string newBranchName = "AnotherDevelopmentBranch";

                Permission permission = new Permission() 
                { 
                    CopyToChildren = true,
                    PermissionType = PermissionTypeEnum.Read,
                    UserOrGroupName = "BartelsK"
                };

                Console.WriteLine($"Creating branch '{ newBranchName }' of source branch '{ sourceBranchFolder }' with permissions for user or group '{ permission.UserOrGroupName }'...\r\n");

                client.BranchClient.CreateBranch(sourceBranchFolder, newBranchName, permission);
            }
            catch (RepositoryException re)
            {
                Console.WriteLine(re.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves the permission on a branch for a specific user or group.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void GetBranchPermissions(RepositoryClient client)
        {            
            string folder = "LibManSamples";
            string branchName = "Development";
            string userOrGroupName = "HR";

            Console.WriteLine("Checking branch permission...");

            PermissionTypeEnum permission = client.BranchClient.GetPermission(folder, branchName, userOrGroupName);
            Console.WriteLine($"The permission of user or group '{ userOrGroupName }' on branch '{ branchName }' is: '{ permission }'");            
        }

        /// <summary>
        /// Grants permissions to a branch for a specific user or group.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void SetBranchPermission(RepositoryClient client)
        {
            string folder = "LibManSamples";
            string branchName = "Development";
            
            // Grant the HR group Read permission to the specified branch 
            Permission permission = new Permission()
            {
                CopyToChildren = false,
                PermissionType = PermissionTypeEnum.Read,
                UserOrGroupName = "HR"
            };

            Console.WriteLine("Setting branch permission...");

            bool success = client.BranchClient.SetPermission(folder, branchName, permission);
            Console.WriteLine($"The permission for user or group '{ permission.UserOrGroupName }' on branch '{ branchName }' was { (!success ? "NOT " : string.Empty) }set successfully to '{ permission.PermissionType }'.");
        }

        /// <summary>
        /// Deletes permissions from a branch for a specific user or group.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void DeleteBranchPermission(RepositoryClient client)
        {
            string folder = "LibManSamples";
            string branchName = "Development";

            // Remove the HR group permission from the specified branch
            Permission permission = new Permission()
            {
                CopyToChildren = false,
                UserOrGroupName = "HR"
            };

            Console.WriteLine("Removing branch permission...");

            bool success = client.BranchClient.DeletePermission(folder, branchName, permission);
            Console.WriteLine($"The permission for user or group '{ permission.UserOrGroupName }' on branch '{ branchName }' was { (!success ? "NOT " : string.Empty) }removed successfully.");
        }

    }
}
