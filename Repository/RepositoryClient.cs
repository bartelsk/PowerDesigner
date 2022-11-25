// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Branches;
using PDRepository.Common;
using PDRepository.Documents;
using PDRepository.Users;
using System;
using System.Reflection;

namespace PDRepository
{
    /// <summary>
    /// The main entry point for all repository methods.
    /// </summary>
    public class RepositoryClient : IDisposable
    {
        protected readonly ConnectionSettings _currentConnectionSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryClient"/> class.
        /// </summary>
        /// <param name="settings">A RepositorySettings instance.</param>
        protected RepositoryClient(ConnectionSettings settings)
        {
            _currentConnectionSettings = settings;
            this.BranchClient = new BranchClient(settings);
            this.DocumentClient = new DocumentClient(settings);
            this.UserClient = new UserClient(settings);
        }

        /// <summary>
        /// Destroys the current instance of the <see cref="RepositoryClient"/> class.
        /// </summary>
        ~RepositoryClient()
        {
            Dispose();
        }

        /// <summary>
        /// Creates a PowerDesigner client and connects to the repository with the specified repository <see cref="ConnectionSettings"/>.
        /// Please note: this can take a few seconds depending on the speed and health of the repository.
        /// </summary>
        /// <param name="settings">A RepositorySettings instance.</param>      
        public static RepositoryClient CreateClient(ConnectionSettings settings)
        {            
            return new RepositoryClient(settings);
        }

        /// <summary>
        /// Entry point to Branches
        /// </summary>
        public IBranchClient BranchClient { get; }

        /// <summary>
        /// Entry point to Documents
        /// </summary>
        public IDocumentClient DocumentClient { get; }

        /// <summary>
        /// Entry point to Users and Groups
        /// </summary>
        public IUserClient UserClient { get; }

        /// <summary>
        /// Returns the name of the repository definition used at connection time.
        /// </summary>
        public string RepositoryDefinitionName
        {
            get
            {
                return _currentConnectionSettings.RepositoryDefinition;
            }
        }

        /// <summary>
        /// Returns the version information associated with this assembly.
        /// </summary>
        public string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
            }
        }

        /// <summary>
        /// Disposes the <see cref="RepositoryClient"/> class.
        /// </summary>
        public void Dispose()
        {
            BranchClient?.Dispose();
            DocumentClient?.Dispose();
            UserClient?.Dispose();
        }               
    }
}
