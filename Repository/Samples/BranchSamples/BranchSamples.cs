// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDRepository;
using PDRepository.Common;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace BranchSamples
{
    [TestClass]
    public class BranchSamples
    {
        #region Test init / cleanup

        RepositoryClient client = null;

        [TestInitialize]
        public void TestInit()
        {
            Console.WriteLine("PowerDesigner Repository Client");
            Console.WriteLine("===============================\r\n");

            // Get repository connection settings
            ConnectionSettings connectionSettings = new ConnectionSettings()
            {
                Password = ConfigurationManager.AppSettings["PDRepoPassword"],
                RepositoryDefinition = ConfigurationManager.AppSettings["PDRepoDefinition"],
                User = ConfigurationManager.AppSettings["PDRepoUser"]
            };

            // Start PowerDesigner and connect to the repository
            Console.WriteLine("Connecting...");
            client = RepositoryClient.CreateClient(connectionSettings);

            Console.WriteLine($"Connection successful - repository definition '{client.RepositoryDefinitionName}'");
            Console.WriteLine($"Client assembly version: {client.Version}\r\n");
            Console.WriteLine("------------------------------------------------------------\r\n");
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            client?.Dispose();
        }

        #endregion

        /// <summary>
        /// List all branches
        /// </summary>      
        [TestMethod]
        public void ListBranches()
        {
            Console.WriteLine("Listing branches...\r\n");

            string rootFolder = "LibManSamples";

            List<Branch> branches = client.BranchClient.ListBranches(rootFolder);
            branches?.ForEach(b => Console.WriteLine($"Branch: {b.Name} - Relative path: {b.RelativePath}"));
        }

        /// <summary>
        /// List all branches based on access permissions.
        /// </summary>
        [TestMethod]
        public void ListBranchesForUser()
        {
            string rootFolder = "LibManSamples";
            string userLogIn = "BartelsK";

            Console.WriteLine($"Listing branches for user '{userLogIn}'...\r\n");

            List<Branch> branches = client.BranchClient.ListBranches(rootFolder, userLogIn);
            branches?.ForEach(b => Console.WriteLine($"Branch: {b.Name} - Relative path: {b.RelativePath} - Permission: {b.Permission}"));
        }

        /// <summary>
        /// Checks whether a branch exists.
        /// </summary>
        [TestMethod]
        public void BranchExists()
        {
            Console.WriteLine("Testing branch existence...\r\n");

            string repoFolder = "LibManSamples";
            string branchName = "Development";

            bool exists = client.BranchClient.BranchExists(repoFolder, branchName);
            Console.WriteLine($"Branch '{branchName}' " + ((exists) ? "exists." : "does not exist."));
        }

        /// <summary>
        /// Creates a new branch of an existing branch.
        /// </summary>      
        [TestMethod]
        public void CreateBranch()
        {
            string sourceBranchFolder = "LibManSamples/Development";
            string newBranchName = "MyNewDevelopmentBranch";

            Console.WriteLine($"Creating branch '{newBranchName}' of source branch '{sourceBranchFolder}'...\r\n");

            client.BranchClient.CreateBranch(sourceBranchFolder, newBranchName);
        }

        /// <summary>
        /// Creates a new branch of an existing branch and applies specific branch permissions.
        /// </summary>      
        [TestMethod]
        public void CreateBranchWithSpecificPermissions()
        {
            string sourceBranchFolder = "LibManSamples/Development";
            string newBranchName = "AnotherDevelopmentBranch";

            Permission permission = new Permission()
            {
                CopyToChildren = true,
                PermissionType = PermissionTypeEnum.Read,
                UserOrGroupName = "BartelsK"
            };

            Console.WriteLine($"Creating branch '{newBranchName}' of source branch '{sourceBranchFolder}' with permissions for user or group '{permission.UserOrGroupName}'...\r\n");

            client.BranchClient.CreateBranch(sourceBranchFolder, newBranchName, permission);
        }

        /// <summary>
        /// Retrieves the permission on a branch for a specific user or group.
        /// </summary>
        [TestMethod]
        public void GetBranchPermissions()
        {
            string folder = "LibManSamples";
            string branchName = "Development";
            string userOrGroupName = "HR";

            Console.WriteLine("Checking branch permission...");

            PermissionTypeEnum permission = client.BranchClient.GetPermission(folder, branchName, userOrGroupName);
            Console.WriteLine($"The permission of user or group '{userOrGroupName}' on branch '{branchName}' is: '{permission}'");
        }

        /// <summary>
        /// Grants permissions to a branch for a specific user or group.
        /// </summary>
        [TestMethod]
        public void SetBranchPermission()
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
            Console.WriteLine($"The permission for user or group '{permission.UserOrGroupName}' on branch '{branchName}' was {(!success ? "NOT " : string.Empty)}set successfully to '{permission.PermissionType}'.");
        }

        /// <summary>
        /// Deletes permissions from a branch for a specific user or group.
        /// </summary>
        [TestMethod]
        public void DeleteBranchPermission()
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
            Console.WriteLine($"The permission for user or group '{permission.UserOrGroupName}' on branch '{branchName}' was {(!success ? "NOT " : string.Empty)}removed successfully.");
        }
    }
}
