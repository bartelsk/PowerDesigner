// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System;

namespace PDRepository.Common
{
    /// <summary>
    /// Represents a repository user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Determines whether the user account is blocked.
        /// </summary>
        public bool Blocked { get; set; }

        /// <summary>
        /// The user comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Determines whether the user account is disabled.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// The full name of the user.
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        /// The last successful login date.
        /// </summary>
        public DateTime LastLoginDate { get; set; }

        /// <summary>
        /// The login name of the user.
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// Specifies user rights.
        /// </summary>
        public string Rights { get; set; }

        /// <summary>
        /// The user account status.
        /// </summary>
        public UserStatusEnum Status { get; set; }

        /// <summary>
        /// Specifies the groups of which the user is a member. 
        /// </summary>
        public string GroupMembership { get; set; }
    }
}
