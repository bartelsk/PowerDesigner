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
    }
}
