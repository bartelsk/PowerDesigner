// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using System.Collections.Generic;

namespace PDRepository.Users
{
    /// <summary>
    /// This class contains methods to work with PowerDesigner repository users and groups.
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
        /// Returns user information.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <returns>A <see cref="User"/> type with user information.</returns>
        public User GetUserInfo(string loginName)
        {
            if (string.IsNullOrEmpty(loginName)) ThrowArgumentNullException(loginName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return GetRepositoryUser(loginName);
        }

        /// <summary>
        /// Lists the available users.
        /// </summary>
        /// <returns>A List with <see cref="User"/> types.</returns>
        public List<User> ListUsers()
        {
            if (!IsConnected) ThrowNoRepositoryConnectionException();
            return GetRepositoryUsers();
        }

        /// <summary>
        /// Determines whether a user exists.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
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

        /// <summary>
        /// Adds a user to a group.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="groupName">The name of the group to which to add the user.</param>
        public void AddUserToGroup(string loginName, string groupName)
        {
            if (string.IsNullOrEmpty(loginName)) ThrowArgumentNullException(loginName);
            if (string.IsNullOrEmpty(groupName)) ThrowArgumentNullException(groupName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            AddUserToRepositoryGroup(loginName, groupName);
        }

        /// <summary>
        /// Removes a user from a group.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="groupName">The name of the group from which to remove the user.</param>
        public void RemoveUserFromGroup(string loginName, string groupName)
        {
            if (string.IsNullOrEmpty(loginName)) ThrowArgumentNullException(loginName);
            if (string.IsNullOrEmpty(groupName)) ThrowArgumentNullException(groupName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            RemoveUserFromRepositoryGroup(loginName, groupName);
        }

        /// <summary>
        /// Returns a list of <see cref="Group"/> objects of which the specified user is a member.       
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <returns>A List with <see cref="Group"/> objects.</returns>
        public List<Group> GetUserGroups(string loginName)
        {
            if (string.IsNullOrEmpty(loginName)) ThrowArgumentNullException(loginName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return GetRepositoryUserGroups(loginName);
        }

        /// <summary>
        /// Assigns the specified rights to a user.
        /// Please note this method does not affect inherited group rights (if any).
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="rights">A <see cref="UserOrGroupRightsEnum"/> type.</param>
        /// <param name="replaceExisting">When true, replaces the existing user rights with the specified ones. When false, the specified rights will be added to the existing user rights.</param>
        public void SetUserRights(string loginName, UserOrGroupRightsEnum rights, bool replaceExisting)
        {
            if (string.IsNullOrEmpty(loginName)) ThrowArgumentNullException(loginName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            SetRepositoryUserRights(loginName, rights, replaceExisting);
            Refresh();            
        }

        /// <summary>
        /// Returns the user rights as a semi-colon separated string.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <returns>A string with user rights.</returns>
        public string GetUserRights(string loginName)
        {
            if (string.IsNullOrEmpty(loginName)) ThrowArgumentNullException(loginName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return GetRepositoryUserRights(loginName);            
        }

        /// <summary>
        /// Blocks a user.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        public void BlockUser(string loginName)
        {
            if (string.IsNullOrEmpty(loginName)) ThrowArgumentNullException(loginName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            BlockRepositoryUser(loginName);
        }

        /// <summary>
        /// Unblocks a user.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        public void UnblockUser(string loginName)
        {
            if (string.IsNullOrEmpty(loginName)) ThrowArgumentNullException(loginName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            UnblockRepositoryUser(loginName);
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        public void DeleteUser(string loginName)
        {
            if (string.IsNullOrEmpty(loginName)) ThrowArgumentNullException(loginName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            DeleteRepositoryUser(loginName);
        }

        public string ResetPassword(string loginName)
        {
            if (string.IsNullOrEmpty(loginName)) ThrowArgumentNullException(loginName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return ResetRepositoryUserPassword(loginName);
        }

        /// <summary>
        /// Returns group information.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>A <see cref="Group"/> type with group information.</returns>
        public Group GetGroupInfo(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) ThrowArgumentNullException(groupName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return GetRepositoryGroup(groupName);
        }

        /// <summary>
        /// Lists the available groups.
        /// </summary>
        /// <returns>A List with <see cref="Group"/> types.</returns>
        public List<Group> ListGroups()
        {
            if (!IsConnected) ThrowNoRepositoryConnectionException();

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

        /// <summary>
        /// Assigns the specified rights to a group.
        /// Please note this method does not alter the rights of the individual users in the group (if any).
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="rights">A <see cref="UserOrGroupRightsEnum"/> type.</param>
        /// <param name="replaceExisting">When true, replaces the existing group rights with the specified ones. When false, the specified rights will be added to the existing group rights.</param>
        public void SetGroupRights(string groupName, UserOrGroupRightsEnum rights, bool replaceExisting)
        {
            if (string.IsNullOrEmpty(groupName)) ThrowArgumentNullException(groupName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            SetRepositoryGroupRights(groupName, rights, replaceExisting);
            Refresh();
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
