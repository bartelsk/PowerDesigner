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
        /// <param name="settings">The current repository <see cref="ConnectionSettings"/>.</param>
        public UserClient(ConnectionSettings settings) : base(settings)
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
        /// Determines whether a user exists.
        /// </summary>
        /// <param name="loginName">The login name of the user.</param>
        /// <returns>True if the user exists, False if not.</returns>
        public bool UserExists(string loginName)
        {
            if (string.IsNullOrEmpty(loginName)) ThrowArgumentNullException(loginName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return RepositoryUserExists(loginName);
        }

        /// <summary>
        /// Creates a user and assigns the specified rights.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="fullName">The real name of the user.</param>
        /// <param name="emailAddress">The email address of the user (optional).</param>
        /// <param name="temporaryPassword">Contains the temporary password of the newly created user.</param>
        /// <param name="rights">A <see cref="UserOrGroupRightsEnum"/> type.</param>
        public void CreateUser(string loginName, string fullName, string emailAddress, out string temporaryPassword, UserOrGroupRightsEnum rights)
        {
            CreateUser(loginName, fullName, emailAddress, out temporaryPassword, rights, null);
        }

        /// <summary>
        /// Creates a user and assigns the specified rights.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="fullName">The real name of the user.</param>
        /// <param name="emailAddress">The email address of the user (optional).</param>
        /// <param name="temporaryPassword">Contains the temporary password of the newly created user.</param>
        /// <param name="rights">A <see cref="UserOrGroupRightsEnum"/> type.</param>
        /// <param name="groupName">The name of the group to which to add the user.</param>
        public void CreateUser(string loginName, string fullName, string emailAddress, out string temporaryPassword, UserOrGroupRightsEnum rights, string groupName)
        {
            if (string.IsNullOrEmpty(loginName)) ThrowArgumentNullException(loginName);
            if (string.IsNullOrEmpty(fullName)) ThrowArgumentNullException(fullName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            CreateRepositoryUser(loginName, fullName, emailAddress, out temporaryPassword, rights, groupName);
        }

        public void AddUserToGroup(string loginName, string groupName)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveUserFromGroup(string loginName, string groupName)
        {
            throw new System.NotImplementedException();
        }

        public void SetUserRights(string loginName, UserOrGroupRightsEnum rights)
        {
            throw new System.NotImplementedException();
        }

        public string GetUserRights(string loginName)
        {
            throw new System.NotImplementedException();
        }

        public string GetUserGroups(string loginName)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="loginName">The login name of the user to delete.</param>
        public void DeleteUser(string loginName)
        {
            if (string.IsNullOrEmpty(loginName)) ThrowArgumentNullException(loginName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            DeleteRepositoryUser(loginName);
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
        /// Determines whether a group exists.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>True if the group exists, False if not.</returns>
        public bool GroupExists(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) ThrowArgumentNullException(groupName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return RepositoryGroupExists(groupName);
        }

        /// <summary>
        /// Creates a group and assigns the specified rights.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        /// <param name="rights">A <see cref="UserOrGroupRightsEnum"/> type.</param>
        public void CreateGroup(string name, UserOrGroupRightsEnum rights)
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

        public void SetGroupRights(string groupName, UserOrGroupRightsEnum rights)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Deletes a group.
        /// </summary>
        /// <param name="groupName">The name of the group to delete.</param>
        public void DeleteGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) ThrowArgumentNullException(groupName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            DeleteRepositoryGroup(groupName);
        }
    }
}
