using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDRepository.Documents
{
   public class DocumentClient : Repository, IDocumentClient
   {
      public DocumentClient(RepositorySettings settings) : base(settings)
      { }
   }
}
