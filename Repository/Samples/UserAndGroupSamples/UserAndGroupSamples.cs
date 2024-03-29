﻿// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDRepository;
using PDRepository.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace UserAndGroupSamples
{
    [TestClass]
    public class UserAndGroupSamples
    {
        #region Test init / cleanup

        RepositoryClient client = null;

        [TestInitialize]
        public void TestInit()
        {
            Console.WriteLine("PowerDesigner Repository Client");
            Console.WriteLine("===============================\r\n");

            // Get repository connection settings
            ConnectionSettings connectionSettings = new ConnectionSettings()
            {
                Password = ConfigurationManager.AppSettings["PDRepoPassword"],
                RepositoryDefinition = ConfigurationManager.AppSettings["PDRepoDefinition"],
                User = ConfigurationManager.AppSettings["PDRepoUser"]
            };

            // Start PowerDesigner and connect to the repository
            Console.WriteLine("Connecting...");
            client = RepositoryClient.CreateClient(connectionSettings);

            Console.WriteLine($"Connection successful - repository definition '{client.RepositoryDefinitionName}'");
            Console.WriteLine($"Client assembly version: {client.Version}\r\n");
            Console.WriteLine("------------------------------------------------------------\r\n");
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            client?.Dispose();
        }

        #endregion

        /// <summary>
        /// Retrieve all users but only display the first 25 users.
        /// </summary>
        [TestMethod]
        public void ListUsers()
        {
            Console.WriteLine("Retrieving users...\r\n");

            List<User> users = client.UserClient.ListUsers();
            
            Console.WriteLine("Listing first 25 users...\r\n");

            users?.Take(25).ToList().ForEach(u => Console.WriteLine($"Name: {u.FullName} - Status: {u.Status} - Rights: {u.Rights} "));            
        }

        /// <summary>
        /// Determines whether a user exists.
        /// </summary>
        [TestMethod]
        public void UserExists()
        {
            Console.WriteLine("Checking whether user exists...\r\n");

            string loginName = "UserA";

            bool exists = client.UserClient.UserExists(loginName);
            Console.WriteLine($"A user with login name '{loginName}' does{(exists ? string.Empty : " not")} exist.");

            Console.WriteLine("Check complete.");
        }

        /// <summary>
        /// Displays user information.
        /// </summary>
        [TestMethod]
        public void GetUserInformation()
        {
            Console.WriteLine("Getting user information...\r\n");

            string loginName = "UserA";

            if (client.UserClient.UserExists(loginName))
            {
                User user = client.UserClient.GetUserInfo(loginName);
                Console.WriteLine($"Information for user '{user.FullName}':\r\n");
                Console.WriteLine($"Status: {user.Status}");
                Console.WriteLine($"Blocked: {user.Blocked}");
                Console.WriteLine($"Comment: {user.Comment}");
                Console.WriteLine($"Disabled: {user.Disabled}");
                Console.WriteLine($"Last login date: {user.LastLoginDate}");
                Console.WriteLine($"Last modified date: {user.LastModifiedDate}");
                Console.WriteLine($"Rights: {user.Rights}");
                Console.WriteLine($"Group membership: {user.GroupMembership}");                
            }
        }

        /// <summary>
        /// Creates a user.
        /// </summary>
        [TestMethod]
        public void CreateUser()
        {
            string loginName = "UserA";
            string fullName = "My Full Name";
            string emailAddress = "UserA@company.com";
            UserOrGroupRightsEnum userRights = UserOrGroupRightsEnum.Connect | UserOrGroupRightsEnum.FreezeVersions;

            Console.WriteLine($"Creating user with login name '{loginName}'...\r\n");

            client.UserClient.CreateUser(loginName, fullName, emailAddress, out string temporaryPassword, userRights);

            Console.WriteLine($"User '{loginName}' created with temporary password '{@temporaryPassword}'.\r\n");
        }

        /// <summary>
        /// Creates a user and adds it to a group.
        /// </summary>
        [TestMethod]
        public void CreateUserAndAddToGroup()
        {
            string loginName = "UserA";
            string fullName = "My Full Name";
            string groupName = "AGroup";
            string emailAddress = "UserA@company.com";
            UserOrGroupRightsEnum userRights = UserOrGroupRightsEnum.Connect | UserOrGroupRightsEnum.EditPortalObjects;

            Console.WriteLine($"Creating user with login name '{loginName}' in group '{groupName}'...\r\n");

            client.UserClient.CreateUser(loginName, fullName, emailAddress, out string temporaryPassword, userRights, groupName);

            Console.WriteLine($"User '{loginName}' created with temporary password '{@temporaryPassword}' in group '{groupName}'.\r\n");
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        [TestMethod]
        public void DeleteUser()
        {
            string loginName = "UserA";

            Console.WriteLine($"Deleting user with login name '{loginName}'...\r\n");
            client.UserClient.DeleteUser(loginName);

            Console.WriteLine($"User '{loginName}' has been deleted.\r\n");
        }

        /// <summary>
        /// Resets the password of a user.
        /// </summary>
        [TestMethod]
        public void ResetUserPassword()
        {
            string loginName = "UserA";

            Console.WriteLine($"Resetting password of user with login name '{loginName}'...\r\n");
            string newTemporaryPassword = client.UserClient.ResetPassword(loginName);

            Console.WriteLine($"The password of user '{loginName}' has been reset. The new temporary password is '{@newTemporaryPassword}'.\r\n");
        }

        /// <summary>
        /// Retrieve all groups but only display the first 10 groups.
        /// </summary>
        [TestMethod]
        public void ListGroups()
        {
            Console.WriteLine("Retrieving groups...\r\n");

            List<Group> groups = client.UserClient.ListGroups();            
            
            Console.WriteLine("Listing first 10 groups...\r\n");
            groups?.Take(10).ToList().ForEach(g => Console.WriteLine($"Name: {g.Name} - Description: {g.Description} - Rights: {g.Rights}"));            
        }

        /// <summary>
        /// Retrieves the members of a particular group.
        /// </summary>
        [TestMethod]
        public void GetGroupMembers()
        {
            string groupName = "HR";

            Console.WriteLine($"Retrieving members of group '{groupName}'...\r\n");

            List<User> users = client.UserClient.GetGroupMembers(groupName);
                        
            Console.WriteLine("Listing users...\r\n");
            users?.ForEach(u => Console.WriteLine($"Login name: {u.LoginName} - Full name: {u.FullName} - Rights: {u.Rights}"));            
        }

        /// <summary>
        /// Determines whether a group exists.
        /// </summary>
        [TestMethod]
        public void GroupExists()
        {
            Console.WriteLine("Checking whether group exists...\r\n");

            string groupName = "AGroup";

            bool exists = client.UserClient.GroupExists(groupName);
            Console.WriteLine($"Group '{groupName}' does{(exists ? string.Empty : " not")} exist.");

            Console.WriteLine("Check complete.");
        }

        /// <summary>
        /// Displays group information.
        /// </summary>
        [TestMethod]
        public void GetGroupInformation()
        {
            Console.WriteLine("Getting group information...\r\n");

            string groupName = "AGroup";

            if (client.UserClient.GroupExists(groupName))
            {
                Group group = client.UserClient.GetGroupInfo(groupName);
                Console.WriteLine($"Group name: '{group.Name}', description: '{group.Description}', rights: {group.Rights}\r\n");
            }
        }

        /// <summary>
        /// Creates a group.
        /// </summary>
        [TestMethod]
        public void CreateGroup()
        {
            string groupName = "AGroup";
            UserOrGroupRightsEnum groupRights = UserOrGroupRightsEnum.Connect | UserOrGroupRightsEnum.FreezeVersions | UserOrGroupRightsEnum.ManageBranches;

            Console.WriteLine($"Creating group '{groupName}'...\r\n");
            client.UserClient.CreateGroup(groupName, groupRights);

            Console.WriteLine($"Group '{groupName}' created.\r\n");
        }

        /// <summary>
        /// Retrieves group rights.
        /// </summary>
        [TestMethod]
        public void GetGroupRights()
        {
            string groupName = "AGroup";

            Console.WriteLine($"Retrieving rights for group '{groupName}'...\r\n");
            string groupRights = client.UserClient.GetGroupRights(groupName);

            Console.WriteLine($"Group '{groupName}' has the following rights: {groupRights} \r\n");
        }

        /// <summary>
        /// Assigns additional group rights.
        /// </summary>
        [TestMethod]
        public void AddGroupRights()
        {
            string groupName = "AGroup";
            UserOrGroupRightsEnum rights = UserOrGroupRightsEnum.EditPortalObjects | UserOrGroupRightsEnum.EditPortalExtensions;

            Console.WriteLine($"Assigning additional group rights to group '{groupName}'...\r\n");
            client.UserClient.SetGroupRights(groupName, rights, false);

            Console.WriteLine($"Rights of group '{groupName}' have been updated. \r\n");
        }

        /// <summary>
        /// Clears the available group rights and assigns new ones.
        /// </summary>
        [TestMethod]
        public void ReplaceGroupRights()
        {
            string groupName = "AGroup";
            UserOrGroupRightsEnum rights = UserOrGroupRightsEnum.ManageUsers;

            Console.WriteLine($"Replacing group rights of group '{groupName}'...\r\n");
            client.UserClient.SetGroupRights(groupName, rights, true);

            Console.WriteLine($"Rights of group '{groupName}' have been updated. \r\n");
        }

        /// <summary>
        /// Deletes a group.
        /// </summary>
        [TestMethod]
        public void DeleteGroup()
        {
            string groupName = "AGroup";

            Console.WriteLine($"Deleting group '{groupName}'...\r\n");
            client.UserClient.DeleteGroup(groupName);

            Console.WriteLine($"Group '{groupName}' has been deleted.\r\n");
        }

        /// <summary>
        /// Retrieves the groups of which the user is a member.
        /// </summary>
        [TestMethod]
        public void GetUserGroups()
        {
            string loginName = "UserA";

            Console.WriteLine($"Listing groups of which user '{loginName}' is a member...\r\n");

            List<Group> groups = client.UserClient.GetUserGroups(loginName);            
            groups?.ForEach(g => Console.WriteLine($"Group name: {g.Name} - Description: {g.Description} - Rights: {g.Rights}"));            
        }

        /// <summary>
        /// Retrieves user rights.
        /// </summary>
        [TestMethod]
        public void GetUserRights()
        {
            string loginName = "UserA";

            Console.WriteLine($"Listing user rights of user '{loginName}'...\r\n");
            string userRights = client.UserClient.GetUserRights(loginName);

            Console.WriteLine($"User '{loginName}' has the following rights: {userRights} \r\n");
        }

        /// <summary>
        /// Assigns additional user rights.
        /// </summary>
        [TestMethod]
        public void AddUserRights()
        {
            string loginName = "UserA";
            UserOrGroupRightsEnum rights = UserOrGroupRightsEnum.EditPortalObjects | UserOrGroupRightsEnum.EditPortalExtensions;

            Console.WriteLine($"Assigning additional user rights to user '{loginName}'...\r\n");
            client.UserClient.SetUserRights(loginName, rights, false);

            Console.WriteLine($"Rights of user '{loginName}' have been updated. \r\n");
        }

        /// <summary>
        /// Clears the available user rights and assigns new ones.
        /// </summary>
        [TestMethod]
        public void ReplaceUserRights()
        {
            string loginName = "UserA";
            UserOrGroupRightsEnum rights = UserOrGroupRightsEnum.Connect;

            Console.WriteLine($"Replacing user rights of user '{loginName}'...\r\n");
            client.UserClient.SetUserRights(loginName, rights, true);

            Console.WriteLine($"Rights of user '{loginName}' have been updated. \r\n");
        }
    }
}
