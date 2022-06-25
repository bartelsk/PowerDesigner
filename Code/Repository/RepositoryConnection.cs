using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDRepository
{
   internal sealed class RepositoryConnection : IDisposable
   {
      private bool disposedValue;
      private static readonly Lazy<RepositoryConnection> _connection = new Lazy<RepositoryConnection>(() => new RepositoryConnection());
      private RepositorySettings _settings;
      //private readonly string _me;

      public static RepositoryConnection Instance { get { return _connection.Value; } }

      private RepositoryConnection()
      {
         // Called only once (upon creation)
         //_me = DateTime.Now.ToShortTimeString();
      }

      ~RepositoryConnection()
      {
         Dispose(disposing: false);
      }

      private void Dispose(bool disposing)
      {
         if (!disposedValue)
         {
            if (disposing)
            {
               // TODO: dispose managed state (managed objects)
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

      public RepositorySettings Settings
      {
         set
         {
            if (_settings == null)
            {
               _settings = value;
            }
         }
      }
      public void Connect()
      {
         // Handles creation of pd.application and repository connection

      }

      public bool IsConnected
      {
         get
         {
            return true; //return _refToCOMRepositoryObject.Count > 0;
         }
      }

   }
}
