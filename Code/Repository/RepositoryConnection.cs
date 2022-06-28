﻿using System;

namespace PDRepository
{
    /// <summary>
    /// Singleton class that starts PdShell16.exe and connects to the repository. 
    /// Maintains the current connection to the PowerDesigner repository.
    /// </summary>
    internal sealed class RepositoryConnection : IDisposable
    {
        private bool disposedValue;
        private static readonly Lazy<RepositoryConnection> _connection = new Lazy<RepositoryConnection>(() => new RepositoryConnection());
        private RepositorySettings _settings;
        private PdCommon.Application _app;
        private PdRMG.RepositoryConnection _pdRepoCon;

        public static RepositoryConnection Instance { get { return _connection.Value; } }

        #region Constructor / Destructor

        private RepositoryConnection()
        {
            // Called only once, starts PdShell16.exe         
            _app = new PdCommon.Application
            {
                InteractiveMode = PdCommon.InteractiveModeValue.im_Batch    // Suppress dialogs
            };
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

        #endregion

        /// <summary>
        /// Sets the current <see cref="RepositorySettings"/>.
        /// </summary>
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

        /// <summary>
        /// Creates a repository connection with the current <see cref="RepositorySettings"/>.
        /// </summary>
        public void Connect()
        {
            if (_app != null)
            {
                _pdRepoCon = (PdRMG.RepositoryConnection)_app.RepositoryConnection;
                if (!_pdRepoCon.Open("", _settings.User, _settings.Password))
                {
                    throw new RepositoryException("Invalid repository credentials.");
                }                
            }
        }

        /// <summary>
        /// Returns True if a successful repository connection has been made.
        /// </summary>
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
