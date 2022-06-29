using PDRepository.LibraryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<Branch> ListBranches(string path)
        {            
            var branches = new List<Branch>();
            if (IsConnected)
            {
                branches = GetBranchFolders(path);
            }
            else
            {
                CreateRepositoryException("No repository connection.");
            }
            return branches;
        }
    }
}
