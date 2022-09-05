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

        public List<User> ListUsers()
        {
            return GetRepositoryUsers();
        }
    }
}
