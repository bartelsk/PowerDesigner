// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

namespace PDRepository.Common
{
    /// <summary>
    /// Permission types of a repository object.
    /// </summary>
    public enum PermissionTypeEnum 
    { 
        /// <summary>
        /// Permission has not been set.
        /// </summary>
        NotSet = -1,

        /// <summary>
        /// View the document or folder in the repository browser and in search results. 
        /// Without this permission, the folder or document is hidden from the user.
        /// </summary>
        Listable = 0, 
        
        /// <summary>
        /// Read-only access to the document or folder.
        /// </summary>
        Read = 10, 
        
        /// <summary>
        /// Permission to check-in a document.
        /// </summary>
        Submit = 20, 
        
        /// <summary>
        /// Permission to write to the document or folder.
        /// </summary>
        Write = 30, 
        
        /// <summary>
        /// Full access on all repository objects.
        /// </summary>
        Full = 40 
    }

    /// <summary>
    /// Permission types of a repository user or group.
    /// </summary>
    public enum UserRightsEnum 
    { 
        /// <summary>
        /// No rights.
        /// </summary>
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
    public enum UserStatusEnum 
    {  
        /// <summary>
        /// The user account is active.
        /// </summary>
        Active, 

        /// <summary>
        /// The user account is inactive.
        /// </summary>
        Inactive, 

        /// <summary>
        /// The user account is blocked.
        /// </summary>
        Blocked 
    }
}
