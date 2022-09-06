// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

namespace PDRepository.Common
{
    /// <summary>
    /// Permission types of a repository object.
    /// </summary>
    public enum PermissionTypeEnum { NotSet = -1, Listable = 0, Read = 10, Submit = 20, Write = 30, Full = 40 }

    /// <summary>
    /// Permission types of a repository user or group.
    /// </summary>
    public enum UserRightsEnum 
    { 
        None = 0,

        /// <summary>
        /// Connect to the repository.
        /// </summary>
        Connect = 1,

        /// <summary>
        /// Freeze and unfreeze document versions.
        /// </summary>
        FreezeVersions = 2,

        /// <summary>
        /// Lock documents to prevent other users from making changes to them.
        /// </summary>
        LockVersions = 4,

        /// <summary>
        /// Create repository branches.
        /// </summary>
        ManageBranches = 8,

        /// <summary>
        /// Create sets of repository documents.
        /// </summary>
        ManageConfigurations = 16,

        /// <summary>
        /// Perform any action on any document version. 
        /// Implicitly includes Full permission on all repository documents.
        /// </summary>
        ManageAllDocuments = 32,
        
        /// <summary>
        /// Create, modify, and delete repository users and groups, grant them rights, and add them to groups. 
        /// Users with this right can list all repository documents and set permissions on them without needing explicit Full permission.
        /// </summary>
        ManageUsers = 64,

        /// <summary>
        /// Create, upgrade, and delete the repository database.
        /// </summary>
        ManageRepository = 128,

        /// <summary>
        /// Create and edit diagrams in PowerDesigner Web.
        /// </summary>
        EditPortalObjects = 256,
        
        /// <summary>
        /// Create and edit custom properties in PowerDesigner Web.
        /// </summary>
        EditPortalExtensions = 512
    }

    /// <summary>
    /// The status of a repository user.
    /// </summary>
    public enum UserStatusEnum {  Active, Inactive, Blocked }
}
