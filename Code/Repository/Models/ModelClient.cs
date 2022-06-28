using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDRepository.Models
{
    /// <summary>
    /// This class contains methods to work with PowerDesigner repository models.
    /// </summary>
    public class ModelClient : Repository, IModelClient
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ModelClient"/> class.
        /// </summary>
        /// <param name="settings">The current <see cref="RepositorySettings"/>.</param>
        public ModelClient(RepositorySettings settings) : base(settings)
        {
            Connect();
        }
    }
}
