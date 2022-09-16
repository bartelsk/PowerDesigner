// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using PDRepository.Exceptions;
using System;

namespace PDRepository
{
    /// <summary>
    /// Internal class that starts PdShell16.exe and connects to the PowerDesigner repository.     
    /// </summary>
    internal sealed class RepositoryConnection : IDisposable
    {
        private bool disposedValue;
        private static readonly Lazy<RepositoryConnection> _connection = new Lazy<RepositoryConnection>(() => new RepositoryConnection());
        private ConnectionSettings _connectionSettings;
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
        /// Returns the active repository connection.
        /// </summary>
        public PdRMG.RepositoryConnection Connection
        {
            get
            {
                return _pdRepoCon;
            }
        }

        /// <summary>
        /// Sets the current repository <see cref="ConnectionSettings"/>.
        /// </summary>
        public ConnectionSettings Settings
        {
            set
            {
                if (_connectionSettings == null)
                {
                    _connectionSettings = value;
                }
            }
        }

        /// <summary>
        /// Creates a repository connection with the current <see cref="ConnectionSettings"/>.
        /// </summary>
        public void Connect()
        {
            if (_app != null)
            {
                _pdRepoCon = (PdRMG.RepositoryConnection)_app.RepositoryConnection;
                if (!_pdRepoCon.Open(_connectionSettings.RepositoryDefinition, _connectionSettings.User, _connectionSettings.Password))
                {
                    throw new InvalidCredentialsException("Could not connect to the repository: invalid credentials.");
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
