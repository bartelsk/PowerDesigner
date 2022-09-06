// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using System.Collections.Generic;

namespace PDRepository.Users
{
    /// <summary>
    /// This class contains methods to work with PowerDesigner repository users.
    /// </summary>
    public class UserClient : Repository, IUserClient
    {
        /// <summary>
        /// Creates a new instance of the <see cref="UserClient"/> class.
        /// </summary>
        /// <param name="settings">The current <see cref="RepositorySettings"/>.</param>
        public UserClient(RepositorySettings settings) : base(settings)
        {
            Connect();
        }

        /// <summary>
        /// Lists the available users.
        /// </summary>
        /// <returns>A List with <see cref="User"/> types.</returns>
        public List<User> ListUsers()
        {
            return GetRepositoryUsers();
        }

        /// <summary>
        /// Lists the available groups.
        /// </summary>
        /// <returns>A List with <see cref="Group"/> types.</returns>
        public List<Group> ListGroups()
        {
            return GetRepositoryGroups();
        }

        /// <summary>
        /// Creates a group and assigns the specified rights.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        /// <param name="rights">A <see cref="UserRightsEnum"/> type.</param>
        public void CreateGroup(string name, UserRightsEnum rights)
        {
            if (string.IsNullOrEmpty(name)) ThrowArgumentNullException(name);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            CreateRepositoryGroup(name, rights);
        }

        /// <summary>
        /// Returns the group rights as a semi-colon separated string.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>A string with group rights.</returns>
        public string GetGroupRights(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) ThrowArgumentNullException(groupName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return GetRepositoryGroupRights(groupName);
        }
    }
}
