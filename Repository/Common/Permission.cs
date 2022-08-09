// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

namespace PDRepository.Common
{
    /// <summary>
    /// Represents permission settings for a user or group of a particular repository object.
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// The name of the user or group.
        /// </summary>
        public string UserOrGroupName { get; set; }

        /// <summary>
        /// The type of permission.
        /// </summary>
        public PermissionTypeEnum PermissionType { get; set; }

        /// <summary>
        /// Determines whether to copy the permission to all child objects (if any).
        /// </summary>
        public bool CopyToChildren { get; set; }
    }
}
