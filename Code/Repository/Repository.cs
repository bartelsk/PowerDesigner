﻿using PDRepository.LibraryModels;
using PdRMG;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDRepository
{
#pragma warning disable CS1591

    /// <summary>
    /// Abstract class that provides methods to interact with the PowerDesigner repository.
    /// </summary>
    public abstract class Repository : IDisposable
    {
        private bool disposedValue;
        internal readonly RepositoryConnection _con;

        #region Constructor / Destructor

        protected Repository(RepositorySettings settings)
        {
            _con = RepositoryConnection.Instance;
            _con.Settings = settings;
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

        protected static void ThrowNoRepositoryConnectionException()
        {
            CreateRepositoryException("No repository connection.");
        }

        protected static RepositoryException CreateRepositoryException(string message)
        {
            throw new RepositoryException(message);
        }

        protected static RepositoryException CreateRepositoryException(string message, Exception innerException)
        {
            throw new RepositoryException(message, innerException);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Creates a repository connection with the current <see cref="RepositorySettings"/>.
        /// </summary>
        public void Connect()
        {
            try
            {
                if (_con == null || !_con.IsConnected)
                {
                    _con.Connect();
                }
            }
            catch (Exception)
            {
                _con.Dispose();
                // Rethrow to notify clients
                throw;
            }            
        }

        /// <summary>
        /// Returns True if a successful repository connection has been made.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _con.IsConnected;
            }
        }

        /// <summary>
        /// Returns a <see cref="RepositoryFolder"/> instance.
        /// </summary>
        /// <param name="folderPath">The location of the folder in the repository.</param>
        /// <returns>A <see cref="RepositoryFolder"/> type.</returns>
        public RepositoryFolder GetRepositoryFolder(string folderPath)
        {
            RepositoryFolder folder = null;
            BaseObject baseFolder = _con.Connection.FindChildByPath(folderPath, (int)PdRMG_Classes.cls_RepositoryFolder);
            if (baseFolder != null)
            {
                folder = (RepositoryFolder)baseFolder;                
            }
            return folder;
        }

        /// <summary>
        /// Returns a list of <see cref="Branch"/> objects, relative to the specified root folder.
        /// </summary>
        /// <param name="rootFolder">The repository folder from which to start the search.</param>
        /// <returns>A List with <see cref="Branch"/> objects.</returns>
        public List<Branch> GetBranchFolders(string rootFolder)
        {
            List<Branch> branches = new List<Branch>();
            RepositoryFolder repositoryFolder = GetRepositoryFolder(rootFolder);
            if (repositoryFolder != null)
            {
                ListBranches(repositoryFolder, ref branches, string.Empty);
            }
            return branches;            
        }

        /// <summary>
        /// Returns a list of <see cref="Document"/> objects in the specified path.
        /// Does not recurse sub-folders.
        /// </summary>
        /// <param name="folderPath">The repository folder from which to retrieve the documents.</param>
        /// <returns>A List with <see cref="Document"/> objects.</returns> 
        public List<Document> GetFolderDocuments(string folderPath)
        {
            List<Document> documents = new List<Document>();
            RepositoryFolder repositoryFolder = GetRepositoryFolder(folderPath);
            if (repositoryFolder != null)
            {
                foreach (var item in repositoryFolder.ChildObjects.Cast<StoredObject>())
                {
                    RepositoryDocument repoDocument = (RepositoryDocument)item;
                    documents.Add(new Document() 
                    { 
                        ClassName = repoDocument.ClassName,
                        Location = repoDocument.Location,
                        Name = repoDocument.Name,
                        Version = repoDocument.Version,
                        VersionComment = repoDocument.VersionComment
                    });
                }
            }
            return documents;
        }

        public Document GetGetDocumentInfo(string folderPath, string documentName)
        {
            return null;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Recursively retrieves branch folders from the repository starting at the specified root folder.
        /// </summary>
        /// <param name="rootFolder">The repository folder from which to start the search.</param>
        /// <param name="branches">A List type that will contain the encountered branch folders.</param>
        /// <param name="location">Used to track the current folder location in the recursion process.</param>
        private static void ListBranches(StoredObject rootFolder, ref List<Branch> branches, string location)
        {
            if (rootFolder.ClassKind != (int)PdRMG_Classes.cls_RepositoryBranchFolder)
            {
                foreach (var item in rootFolder.ChildObjects.Cast<StoredObject>())
                {
                    switch (item.ClassKind)
                    {
                        case (int)PdRMG_Classes.cls_RepositoryFolder:
                            RepositoryFolder folder = (RepositoryFolder)item;                            

                            // Continue search through child folders
                            foreach (var child in folder.ChildObjects.Cast<StoredObject>())
                            {
                                ListBranches(child, ref branches, (string.IsNullOrEmpty(location) ? rootFolder.Name + "/" + folder.Name : location + "/" + rootFolder.Name + "/" + folder.Name));
                            }
                            break;
                        case (int)PdRMG_Classes.cls_RepositoryBranchFolder:
                            RepositoryBranchFolder branchFolder = (RepositoryBranchFolder)item;
                            Branch branch = new Branch()
                            {                                
                                RelativePath = (string.IsNullOrEmpty(location) ? rootFolder.Name : location + "/" + rootFolder.Name),
                                Name = branchFolder.DisplayName
                            };
                            branches.Add(branch);                            
                            break;
                    }
                }
            }
            else
            {
                RepositoryBranchFolder branchFolder = (RepositoryBranchFolder)rootFolder;
                Branch branch = new Branch()
                {                    
                    RelativePath = location,
                    Name = branchFolder.DisplayName
                };
                branches.Add(branch);                
            }
        }

        #endregion       
    }

#pragma warning restore CS1591
}
