// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PDRepository.Samples
{
    static class UserSamples
    {
        /// <summary>
        /// Retrieve all users but only display the first 25 users.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void ListUsers(RepositoryClient client)
        {
            Console.WriteLine("Retrieving users...\r\n");

            List<User> users = client.UserClient.ListUsers();

            Console.WriteLine("Listing first 25 users...\r\n");

            users.Take(25).ToList().ForEach(u => Console.WriteLine($"Name: { u.FullName } - Status: { u.Status }"));
        }

        /// <summary>
        /// Determines whether a user exists.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void UserExists(RepositoryClient client)
        {
            Console.WriteLine("Checking whether user exists...\r\n");

            string userName = "AUser";

            bool exists = client.UserClient.UserExists(userName);
            Console.WriteLine($"User '{ userName }' does{ (exists ? string.Empty : " not") } exist.");

            Console.WriteLine("Check complete.");
        }

        /// <summary>
        /// Retrieve all groups but only display the first 10 groups.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void ListGroups(RepositoryClient client)
        {
            Console.WriteLine("Retrieving groups...\r\n");

            List<Group> groups = client.UserClient.ListGroups();

            Console.WriteLine("Listing first 10 groups...\r\n");
            groups.Take(10).ToList().ForEach(g => Console.WriteLine($"Name: { g.Name } - Description: { g.Description } - Rights: { g.Rights } "));
        }

        /// <summary>
        /// Determines whether a group exists.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void GroupExists(RepositoryClient client)
        {
            Console.WriteLine("Checking whether group exists...\r\n");

            string groupName = "AGroup";

            bool exists = client.UserClient.GroupExists(groupName);
            Console.WriteLine($"Group '{ groupName }' does{ (exists ? string.Empty : " not") } exist.");

            Console.WriteLine("Check complete.");
        }

        /// <summary>
        /// Creates a new group.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void CreateGroup(RepositoryClient client)
        {
            string groupName = "MyNewGroup";
            UserRightsEnum groupRights = UserRightsEnum.Connect | UserRightsEnum.FreezeVersions | UserRightsEnum.ManageBranches;
            
            Console.WriteLine($"Creating group '{ groupName }'...\r\n");
            client.UserClient.CreateGroup(groupName, groupRights);

            Console.WriteLine($"Group '{ groupName }' created.\r\n");
        }

        /// <summary>
        /// Retrieves group rights.
        /// </summary>
        /// <param name="client">An instance of the <see cref="RepositoryClient"/>.</param>
        public static void GetGroupRights(RepositoryClient client)
        {
            string groupName = "MyNewGroup";            

            Console.WriteLine($"Retrieving rights for group '{ groupName }'...\r\n");
            string groupRights = client.UserClient.GetGroupRights(groupName);

            Console.WriteLine($"Group '{ groupName }' has the following rights: { groupRights } \r\n");
        }

        public static void DeleteGroup(RepositoryClient client)
        {
            string groupName = "MyNewGroup";

            Console.WriteLine($"Deleting group '{ groupName }'...\r\n");
            client.UserClient.DeleteGroup(groupName);

            Console.WriteLine($"Group '{ groupName }' has been deleted.\r\n");
        }
    }
}
