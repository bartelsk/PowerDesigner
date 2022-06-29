using PDRepository.LibraryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDRepository.Branches
{
    /// <summary>
    /// Defines the interface for the <see cref="BranchClient"/> class.
    /// </summary>
    public interface IBranchClient : IDisposable
    {
        /// <summary>
        /// Returns a list of <see cref="Branch"/> objects, relative to the specified path.
        /// </summary>
        /// <param name="path">The repository folder from which to start the search.</param>
        /// <returns>A List with <see cref="Branch"/> objects.</returns>       
        List<Branch> ListBranches(string path);
    }
}
