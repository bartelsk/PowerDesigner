using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDRepository.Users
{
   public class UserClient : Repository, IUserClient
   {
      public UserClient(RepositorySettings settings) : base(settings)
      { }
   }
}
