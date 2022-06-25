using PDRepository.Branches;
using PDRepository.Documents;
using PDRepository.Models;
using PDRepository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDRepository
{
#pragma warning disable CS1591

   public abstract class Repository : IDisposable
   {
      private bool disposedValue;
      
      protected readonly List<string> _refToCOMRepositoryObject;      

      internal readonly RepositoryConnection _con;

      #region Constructor / Destructor

      protected Repository(RepositorySettings settings)
      {
         _con = RepositoryConnection.Instance;
         _con.Settings = settings;

         //_refToCOMRepositoryObject = new List<string>();
        // repositorySettings = settings;
      }
      
      ~Repository()
      {         
         Dispose(disposing: false);
      }

      protected virtual void Dispose(bool disposing)
      {
         if (!disposedValue)
         {
            if (disposing)
            {               
               _con.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
         }
      }     

      /// <summary>
      /// Disposes the Repository class.
      /// </summary>
      public void Dispose()
      {         
         Dispose(disposing: true);
         GC.SuppressFinalize(this);
      }

      #endregion

      #region Exceptions

      protected static RepositoryException CreateRepositoryException(string message)
      {
         throw new RepositoryException(message);
      }

      protected static RepositoryException CreateRepositoryException(string message, Exception innerException)
      { 
         throw new RepositoryException(message, innerException);
      }

      #endregion

      #region Repository actions

      protected void Connect()
      {
         _con.Connect();         
      }

      protected bool IsConnected 
      {
         get 
         {
            return _con.IsConnected;
         }          
      }

      protected List<string> GetBranches(string path)
      {
         // using the _con, write repo code to get branches

         List<string> result = new List<string>
         {
            "One"
         };
         return result;         
      }

      #endregion
   }

#pragma warning restore CS1591
}
