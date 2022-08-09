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

        public void CreateBranch(string repoFolderPath, string branchName)
        {

        }

        public void CreateBranch(string repoFolderPath, string branchName, Permission branchPermission)
        {

        }
    }
}
