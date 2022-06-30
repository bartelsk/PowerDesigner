using PDRepository.LibraryModels;
using System.Collections.Generic;

namespace PDRepository.Branches
{
    /// <summary>
    /// This class contains methods to work with PowerDesigner repository branches.
    /// </summary>
    public class BranchClient : Repository, IBranchClient
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BranchClient"/> class.
        /// </summary>
        /// <param name="settings">The current <see cref="RepositorySettings"/>.</param>
        public BranchClient(RepositorySettings settings) : base(settings)
        {
            Connect();
        }

        /// <summary>
        /// Returns a list of <see cref="Branch"/> objects, relative to the specified path.
        /// </summary>
        /// <param name="rootFolderPath">The repository folder from which to start the search.</param>
        /// <returns>A List with <see cref="Branch"/> objects.</returns>       
        public List<Branch> ListBranches(string rootFolderPath)
        {   
            if (!IsConnected) ThrowNoRepositoryConnectionException();  
            return GetBranchFolders(rootFolderPath);
        }
    }
}
