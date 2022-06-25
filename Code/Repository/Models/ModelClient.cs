using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDRepository.Models
{
   public class ModelClient : Repository, IModelClient
   {
      public ModelClient(RepositorySettings settings) : base(settings)
      { }
   }
}
