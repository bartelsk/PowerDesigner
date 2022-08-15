// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using System.Collections.Generic;

namespace PDRepository.Branches
{
    /// <summary>
    /// This class contains methods to work with PowerDesigner repository branches.
    /// </summary>
    public class BranchClient : Repository, IBranchClient
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BranchClient"/> class.
        /// </summary>
        /// <param name="settings">The current <see cref="RepositorySettings"/>.</param>
        public BranchClient(RepositorySettings settings) : base(settings)
        {
            Connect();
        }       

        /// <summary>
        /// Returns a list of <see cref="Branch"/> objects, relative to the specified path.
        /// </summary>
        /// <param name="rootFolderPath">The repository folder from which to start the search.</param>
        /// <returns>A List with <see cref="Branch"/> objects.</returns>       
        public List<Branch> ListBranches(string rootFolderPath)
        {   
            if (!IsConnected) ThrowNoRepositoryConnectionException();  
            return GetBranchFolders(rootFolderPath);
        }

        /// <summary>
        /// Returns a list of <see cref="Branch"/> objects, relative to the specified path.
        /// </summary>
        /// <param name="rootFolderPath">The repository folder from which to start the search.</param>
        /// <param name="userOrGroupNameFilter">A user login or group name used to filter branches based on access permission.</param>
        /// <returns>A List with <see cref="Branch"/> objects.</returns>       
        public List<Branch> ListBranches(string rootFolderPath, string userOrGroupNameFilter)
        {
            if (!IsConnected) ThrowNoRepositoryConnectionException();
            return GetBranchFolders(rootFolderPath, userOrGroupNameFilter);
        }

        /// <summary>
        /// Determines whether the specified branch exists.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to start the search.</param>
        /// <param name="branchName">The name of the branch.</param>
        /// <returns>True if the branch exists, False if it does not.</returns>
        public bool BranchExists(string repoFolderPath, string branchName)
        {
            List<Branch> branches = ListBranches(repoFolderPath);            
            return branches.Exists(b => b.Name.ToLower() == branchName.ToLower());
        }

        /// <summary>
        /// Creates a new branch of an existing repository branch. 
        /// The source branch's document hierarchy will be duplicated in the new branch.
        /// The permissions of the currently connected account will be applied to the new branch.
        /// </summary>
        /// <param name="sourceBranchFolder">The location of the source branch folder in the repository.</param>
        /// <param name="newBranchName">The name for the new branch.</param>
        /// <remarks>The branch creation can fail for several reasons:
        /// - The currently connected account does not have Write permissions on the folder. 
        /// - The currently connected account does not have Manage Branches privilege. 
        /// - The source branch folder already belongs to a branch (sub-branches are not supported).
        /// Be aware that PowerDesigner does not throw an exception in any of these cases.
        /// </remarks>
        public void CreateBranch(string sourceBranchFolder, string newBranchName)
        {
            // TODO: also check for empty parameters
            CreateBranch(sourceBranchFolder, newBranchName, null);
        }

        /// <summary>
        /// Creates a new branch of an existing repository branch. 
        /// The source branch's document hierarchy will be duplicated in the new branch.
        /// </summary>
        /// <param name="sourceBranchFolder">The location of the source branch folder in the repository.</param>
        /// <param name="newBranchName">The name for the new branch.</param>
        /// <param name="branchPermission">Permission settings for the new branch. If omitted, the permissions of the currently connected account will be applied.</param>
        /// <remarks>The branch creation can fail for several reasons:
        /// - The currently connected account does not have Write permissions on the folder. 
        /// - The currently connected account does not have Manage Branches privilege. 
        /// - The source branch folder already belongs to a branch (sub-branches are not supported).
        /// Be aware that PowerDesigner does not throw an exception in any of these cases.
        /// </remarks>
        public void CreateBranch(string sourceBranchFolder, string newBranchName, Permission branchPermission)
        {
            if (!IsConnected) ThrowNoRepositoryConnectionException();
            CreateNewBranch(sourceBranchFolder, newBranchName, branchPermission);
        }
    }
}
