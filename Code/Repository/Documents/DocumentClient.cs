using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDRepository.Documents
{
    /// <summary>
    /// This class contains methods to work with PowerDesigner repository documents.
    /// </summary>
    public class DocumentClient : Repository, IDocumentClient
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DocumentClient"/> class.
        /// </summary>
        /// <param name="settings">The current <see cref="RepositorySettings"/>.</param>
        public DocumentClient(RepositorySettings settings) : base(settings)
        {
            Connect();
        }
    }
}
