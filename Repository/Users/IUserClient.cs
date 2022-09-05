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
        List<User> ListUsers();
    }
}
