using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDRepository.Branches
{
   public interface IBranchClient : IDisposable
   {
        Task<List<string>> ListBranches(string path);
   }
}
