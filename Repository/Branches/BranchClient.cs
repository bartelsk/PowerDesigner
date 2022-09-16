// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using System;
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
        /// <param name="settings">The current repository <see cref="ConnectionSettings"/>.</param>
        public BranchClient(ConnectionSettings settings) : base(settings)
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
            return ListBranches(rootFolderPath, string.Empty);            
        }

        /// <summary>
        /// Returns a list of <see cref="Branch"/> objects, relative to the specified path.
        /// </summary>
        /// <param name="rootFolderPath">The repository folder from which to start the search.</param>
        /// <param name="userOrGroupNameFilter">A user login or group name used to filter branches based on access permission.</param>
        /// <returns>A List with <see cref="Branch"/> objects.</returns>       
        public List<Branch> ListBranches(string rootFolderPath, string userOrGroupNameFilter)
        {
            if (string.IsNullOrEmpty(rootFolderPath)) ThrowArgumentNullException(rootFolderPath);
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
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(branchName)) ThrowArgumentNullException(branchName);              

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
        /// - The currently connected account does not have Write permissions on the folder
        /// - The currently connected account does not have the Manage Branches privilege
        /// - The source branch folder already belongs to a branch (sub-branches are not supported).
        /// Be aware that PowerDesigner does not throw an exception in any of these cases.
        /// </remarks>
        public void CreateBranch(string sourceBranchFolder, string newBranchName)
        {
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
        /// - The currently connected account does not have Write permissions on the folder
        /// - The currently connected account does not have the Manage Branches privilege 
        /// - The source branch folder already belongs to a branch (sub-branches are not supported).
        /// Be aware that PowerDesigner does not throw an exception in any of these cases.
        /// </remarks>
        public void CreateBranch(string sourceBranchFolder, string newBranchName, Permission branchPermission)
        {
            if (string.IsNullOrEmpty(sourceBranchFolder)) ThrowArgumentNullException(sourceBranchFolder);
            if (string.IsNullOrEmpty(newBranchName)) ThrowArgumentNullException(newBranchName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            CreateNewBranch(sourceBranchFolder, newBranchName, branchPermission);
        }

        /// <summary>
        /// Retrieves the permission on a repository branch for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The location of the branch folder in the repository.</param>
        /// <param name="branchName">The name of the branch.</param>
        /// <param name="userOrGroupName">The user login or group name for which to check its permission.</param>
        /// <returns>A <see cref="PermissionTypeEnum"/> type.</returns>
        public PermissionTypeEnum GetPermission(string repoFolderPath, string branchName, string userOrGroupName)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(branchName)) ThrowArgumentNullException(branchName);
            if (string.IsNullOrEmpty(userOrGroupName)) ThrowArgumentNullException(userOrGroupName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return GetBranchPermission(repoFolderPath, branchName, userOrGroupName);
        }

        /// <summary>
        /// Grants permissions to a repository branch for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The location of the branch folder in the repository.</param>
        /// <param name="branchName">The name of the branch.</param>
        /// <param name="permission">The <see cref="Permission"/> that is to be granted to the branch.</param>
        /// <returns>True if successful, False if not.</returns>
        public bool SetPermission(string repoFolderPath, string branchName, Permission permission)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(branchName)) ThrowArgumentNullException(branchName);
            if (permission == null) ThrowArgumentNullException("permission");
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return SetBranchPermission(repoFolderPath, branchName, permission);
        }

        /// <summary>
        /// Deletes all permissions from a repository branch for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The location of the branch folder in the repository.</param>
        /// <param name="branchName">The name of the branch.</param>
        /// <param name="permission">A <see cref="Permission"/> type that specifies the user login or group name and whether to remove the permissions from all child objects as well (if any).</param>
        /// <returns>True if successful, False if not.</returns>
        public bool DeletePermission(string repoFolderPath, string branchName, Permission permission)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(branchName)) ThrowArgumentNullException(branchName);
            if (permission == null) ThrowArgumentNullException("permission");
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return DeleteBranchPermission(repoFolderPath, branchName, permission);
        }
    }
}
