﻿// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
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
        /// Determines whether a user exists.
        /// </summary>
        /// <param name="loginName">The login name of the user.</param>
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
        /// Adds a repository user to a repository group.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="groupName">The name of the group to which to add the user.</param>
        void AddUserToGroup(string loginName, string groupName);

        /// <summary>
        /// Removes a repository user from a repository group.
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

        void SetUserRights(string loginName, UserOrGroupRightsEnum rights);

        string GetUserRights(string loginName);        

        /// <summary>
        /// Blocks a repository user.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param> 
        void BlockUser(string loginName);

        /// <summary>
        /// Unblocks a repository user.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>  
        void UnblockUser(string loginName);

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        void DeleteUser(string loginName);
        
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

        void SetGroupRights(string groupName, UserOrGroupRightsEnum rights);

        /// <summary>
        /// Deletes a group.
        /// </summary>
        /// <param name="groupName">The name of the group to delete.</param>
        void DeleteGroup(string groupName);
    }
}
