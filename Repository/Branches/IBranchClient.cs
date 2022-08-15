// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using System;
using System.Collections.Generic;

namespace PDRepository.Branches
{
    /// <summary>
    /// Defines the interface for the <see cref="BranchClient"/> class.
    /// </summary>
    public interface IBranchClient : IDisposable
    {
        /// <summary>
        /// Returns a list of <see cref="Branch"/> objects, relative to the specified path.
        /// </summary>
        /// <param name="rootFolderPath">The repository folder from which to start the search.</param>
        /// <returns>A List with <see cref="Branch"/> objects.</returns>       
        List<Branch> ListBranches(string rootFolderPath);

        /// <summary>
        /// Returns a list of <see cref="Branch"/> objects, relative to the specified path.
        /// </summary>
        /// <param name="rootFolderPath">The repository folder from which to start the search.</param>
        /// <param name="userOrGroupNameFilter">A user login or group name used to filter branches based on access permission.</param>
        /// <returns>A List with <see cref="Branch"/> objects.</returns>       
        List<Branch> ListBranches(string rootFolderPath, string userOrGroupNameFilter);

        /// <summary>
        /// Determines whether the specified branch exists.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to start the search.</param>
        /// <param name="branchName">The name of the branch.</param>
        /// <returns>True if the branch exists, False if it does not.</returns>
        bool BranchExists(string repoFolderPath, string branchName);

        void CreateBranch(string sourceBranchFolder, string newBranchName);

        void CreateBranch(string sourceBranchFolder, string newBranchName, Permission branchPermission);
    }
}
