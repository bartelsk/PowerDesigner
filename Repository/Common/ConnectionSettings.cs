// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

namespace PDRepository.Common
{
    /// <summary>
    /// This class represents the settings used to connect to the repository via a proxy.
    /// </summary>
    public class ConnectionSettings
    {
        /// <summary>
        /// The login name of the repository user.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// The password of the repository user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The name of the repository definition you want to use to connect to the repository.
        /// If omitted, the current (last used) definition will be used.
        /// </summary>
        public string RepositoryDefinition { get; set; }
    }
}
