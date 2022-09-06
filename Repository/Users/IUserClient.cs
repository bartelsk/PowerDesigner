// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using System;
using System.Collections.Generic;

namespace PDRepository.Users
{
    /// <summary>
    /// Defines the interface for the <see cref="UserClient"/> class.
    /// </summary>
    public interface IUserClient : IDisposable
    {
        /// <summary>
        /// Lists the available users.
        /// </summary>
        /// <returns>A List with <see cref="User"/> types.</returns>
        List<User> ListUsers();

        /// <summary>
        /// Lists the available groups.
        /// </summary>
        /// <returns>A List with <see cref="Group"/> types.</returns>
        List<Group> ListGroups();

        /// <summary>
        /// Determines whether a group exists.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>True if the group exists, False if not.</returns>
        bool GroupExists(string groupName);

        /// <summary>
        /// Creates a group and assigns the specified rights.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        /// <param name="rights">A <see cref="UserRightsEnum"/> type.</param>
        void CreateGroup(string name, UserRightsEnum rights);

        /// <summary>
        /// Returns the group rights as a semi-colon separated string.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>A string with group rights.</returns>
        string GetGroupRights(string groupName);

        /// <summary>
        /// Deletes a repository group.
        /// </summary>
        /// <param name="groupName">The name of the group to delete.</param>
        void DeleteGroup(string groupName);
    }
}
