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
        /// Returns user information.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <returns>A <see cref="User"/> type with user information.</returns>
        User GetUserInfo(string loginName);

        /// <summary>
        /// Lists the available users.
        /// </summary>
        /// <returns>A List with <see cref="User"/> types.</returns>
        List<User> ListUsers();

        /// <summary>
        /// Determines whether a user exists.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <returns>True if the user exists, False if not.</returns>
        bool UserExists(string loginName);

        /// <summary>
        /// Creates a user and assigns the specified rights.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="fullName">The real name of the user.</param>
        /// <param name="emailAddress">The email address of the user (optional).</param>
        /// <param name="temporaryPassword">Contains the temporary password of the newly created user.</param>
        /// <param name="rights">A <see cref="UserOrGroupRightsEnum"/> type.</param>
        void CreateUser(string loginName, string fullName, string emailAddress, out string temporaryPassword, UserOrGroupRightsEnum rights);

        /// <summary>
        /// Creates a user and assigns the specified rights.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="fullName">The real name of the user.</param>
        /// <param name="emailAddress">The email address of the user (optional).</param>
        /// <param name="temporaryPassword">Contains the temporary password of the newly created user.</param>
        /// <param name="rights">A <see cref="UserOrGroupRightsEnum"/> type.</param>
        /// <param name="groupName">The name of the group to which to add the user.</param>
        void CreateUser(string loginName, string fullName, string emailAddress, out string temporaryPassword, UserOrGroupRightsEnum rights, string groupName);

        /// <summary>
        /// Adds a user to a group.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="groupName">The name of the group to which to add the user.</param>
        void AddUserToGroup(string loginName, string groupName);

        /// <summary>
        /// Removes a user from a group.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="groupName">The name of the group from which to remove the user.</param>
        void RemoveUserFromGroup(string loginName, string groupName);

        /// <summary>
        /// Returns a list of <see cref="Group"/> objects of which the specified user is a member.       
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <returns>A List with <see cref="Group"/> objects.</returns>
        List<Group> GetUserGroups(string loginName);

        /// <summary>
        /// Assigns the specified rights to a user.
        /// Please note this method does not affect inherited group rights (if any).
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="rights">A <see cref="UserOrGroupRightsEnum"/> type.</param>
        /// <param name="replaceExisting">When true, replaces the existing user rights with the specified ones. When false, the specified rights will be added to the existing user rights.</param>
        void SetUserRights(string loginName, UserOrGroupRightsEnum rights, bool replaceExisting);

        /// <summary>
        /// Returns the user rights as a semi-colon separated string.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <returns>A string with user rights.</returns>
        string GetUserRights(string loginName);        

        /// <summary>
        /// Blocks a user.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param> 
        void BlockUser(string loginName);

        /// <summary>
        /// Unblocks a user.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>  
        void UnblockUser(string loginName);

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        void DeleteUser(string loginName);

        string ResetPassword(string loginName);

        /// <summary>
        /// Returns group information.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>A <see cref="Group"/> type with group information.</returns>
        Group GetGroupInfo(string groupName);

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
        /// <param name="rights">A <see cref="UserOrGroupRightsEnum"/> type.</param>
        void CreateGroup(string name, UserOrGroupRightsEnum rights);

        /// <summary>
        /// Returns the group rights as a semi-colon separated string.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>A string with group rights.</returns>
        string GetGroupRights(string groupName);

        /// <summary>
        /// Assigns the specified rights to a group.
        /// Please note this method does not alter the rights of the individual users in the group (if any).
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="rights">A <see cref="UserOrGroupRightsEnum"/> type.</param>
        /// <param name="replaceExisting">When true, replaces the existing group rights with the specified ones. When false, the specified rights will be added to the existing group rights.</param>
        void SetGroupRights(string groupName, UserOrGroupRightsEnum rights, bool replaceExisting);

        /// <summary>
        /// Deletes a group.
        /// </summary>
        /// <param name="groupName">The name of the group to delete.</param>
        void DeleteGroup(string groupName);
    }
}
