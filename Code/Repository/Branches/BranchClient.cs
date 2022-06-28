using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDRepository.Branches
{
    public class BranchClient : Repository, IBranchClient
    {
        public BranchClient(RepositorySettings settings) : base(settings)
        {
            Connect();
        }

        public List<string> ListBranches(string path)
        {
            var branches = new List<string>();
            if (IsConnected)
            {
                branches.Add("hi");
            }
            else
            {
                CreateRepositoryException("Not connected!");
            }
            return branches;
        }
    }
}
