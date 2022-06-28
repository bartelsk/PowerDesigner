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
        private PdCommon.Application _app;
        private PdRMG.RepositoryConnection _pdRepoCon;

        public static RepositoryConnection Instance { get { return _connection.Value; } }

        private RepositoryConnection()
        {
            // Called only once (upon creation)            
            _app = new PdCommon.Application();
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
                    // Dispose managed state (managed objects)
                }

                // Free unmanaged resources (unmanaged objects) and override finalizer
                if (_pdRepoCon != null)
                {
                    _pdRepoCon.Close();
                    _pdRepoCon = null;
                }

                if (_app != null)
                {
                    _app = null;
                }                
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
        public async Task ConnectAsync()
        {
            await Task.Run(() =>
            {
                if (_app != null)
                {
                    _pdRepoCon = (PdRMG.RepositoryConnection)_app.RepositoryConnection;
                    if (!_pdRepoCon.Open("", _settings.User, _settings.Password))
                    {
                        throw new RepositoryException("Could not connect with the supplied credentials.");
                    }
                }
            });            
        }

        public bool IsConnected
        {
            get
            {
                if (_pdRepoCon != null)
                {
                    return _pdRepoCon.Connected;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
