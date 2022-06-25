using System;

namespace PDRepository
{
#pragma warning disable CS1591

   public class RepositoryException : Exception
   {
      public RepositoryException(string message) : base(message)
      { }

      public RepositoryException(string message, Exception innerException) : base(message, innerException)
      { }
   }

#pragma warning restore CS1591
}
