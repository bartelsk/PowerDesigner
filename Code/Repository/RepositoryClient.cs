using PDRepository.Branches;
using PDRepository.Common;
using PDRepository.Documents;
using PDRepository.Users;
using System;
using System.Diagnostics;
using System.Reflection;

namespace PDRepository
{
    /// <summary>
    /// The main entry point for all repository methods.
    /// </summary>
    public class RepositoryClient : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryClient"/> class.
        /// </summary>
        /// <param name="settings">A RepositorySettings instance.</param>
        protected RepositoryClient(RepositorySettings settings)
        {
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
        /// Creates a PowerDesigner client and connects to the repository with the specified <see cref="RepositorySettings"/>.
        /// Please note: this can take a few seconds depending on the speed and health of the repository.
        /// </summary>
        /// <param name="settings">A RepositorySettings instance.</param>      
        public static RepositoryClient CreateClient(RepositorySettings settings)
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
        /// Entry point to Users
        /// </summary>
        public IUserClient UserClient { get; }

        /// <summary>
        /// Disposes the <see cref="RepositoryClient"/> class.
        /// </summary>
        public void Dispose()
        {
            BranchClient?.Dispose();
            DocumentClient?.Dispose();            
            UserClient?.Dispose();
        }

        /// <summary>
        /// Returns the version information associated with this assembly.
        /// </summary>
        public string Version
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fileVersion.FileVersion;
            }
        }
    }
}
