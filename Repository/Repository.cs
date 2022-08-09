// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using PDRepository.Exceptions;
using PdRMG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PDRepository
{
    /// <summary>
    /// Abstract class that provides methods to interact with the PowerDesigner repository.
    /// </summary>
    public abstract class Repository : IDisposable
    {
        private bool disposedValue;
        internal readonly RepositoryConnection _con;
        public event EventHandler<CheckOutEventArgs> RepoDocumentCheckedOut;

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
            throw new NoRepositoryConnectionException("No repository connection.");
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

        #region Folders

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

        #endregion

        #region Documents
        
        /// <summary>
        /// Retrieves information on a document in the specified repository folder.
        /// </summary>
        /// <param name="folderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <returns>A <see cref="Document"/> type.</returns>
        public Document GetFolderDocumentInfo(string folderPath, string documentName)
        {
            Document document = null;
            StoredObject item = GetFolderDocument(folderPath, documentName);            
            if (item != null)
            {
                document = ParseStoredObjectInfo(item);
            }            
            return document;
        }

        /// <summary>
        /// Returns a list of <see cref="Document"/> objects in the specified path.        
        /// </summary>
        /// <param name="folderPath">The repository folder from which to retrieve the documents.</param>
        /// <param name="recursive">True to also list documents in any sub-folder of the <paramref name="folderPath"/>.</param>
        /// <returns>A List with <see cref="Document"/> objects.</returns> 
        public List<Document> GetFolderDocumentsInfo(string folderPath, bool recursive)
        {
            List<Document> documents = new List<Document>();
            ListFolderDocuments(folderPath, recursive, ref documents);
            return documents;
        }
        
        /// <summary>
        /// Checks out the document in the specified repository folder and saves it to disc. Overwrites the local document (if any).
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document to check out.</param>
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the document.</param>
        public void CheckOutFolderDocument(string repoFolderPath, string documentName, string targetFolder)
        {
            CheckOutFolderDocument(repoFolderPath, documentName, targetFolder, string.Empty);
        }

        /// <summary>
        /// Checks out the document in the specified repository folder and saves it to disc. Overwrites the local document (if any).
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document to check out.</param>
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the document.</param>
        /// <param name="targetFileName">The file name for the document.</param>
        public void CheckOutFolderDocument(string repoFolderPath, string documentName, string targetFolder, string targetFileName)
        {
            StoredObject item = GetFolderDocument(repoFolderPath, documentName);
            if (item != null)
            {
                RepositoryDocumentBase doc = (RepositoryDocumentBase)item;
                string fileName = (string.IsNullOrEmpty(targetFileName)) ? GetDocumentFileName(targetFolder, doc) : Path.Combine(targetFolder, targetFileName);
                _ = doc.CheckOutToFile(fileName, (int)SRmgMergeMode.SRmgMergeOverwrite, false, out _, out _);

                // Trigger checked out event
                OnDocumentCheckedOut(new CheckOutEventArgs() { CheckOutFileName = fileName, DocumentName = doc.Name });
            }
        }

        /// <summary>
        /// Checks out a specific version of the document in the specified repository folder and saves it to disc. Overwrites the local document (if any).
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document to check out.</param>
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the document.</param>
        /// <param name="version">The document version. The version must belong to the same branch as the current object.</param>
        public void CheckOutFolderDocument(string repoFolderPath, string documentName, string targetFolder, int version)
        {
            CheckOutFolderDocument(repoFolderPath, documentName, targetFolder, string.Empty, version);
        }

        /// <summary>
        /// Checks out a specific version of the document in the specified repository folder and saves it to disc. Overwrites the local document (if any).
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document to check out.</param>
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the document.</param>
        /// <param name="targetFileName">The file name for the document.</param>
        /// <param name="version">The document version. The version must belong to the same branch as the current object.</param>
        public void CheckOutFolderDocument(string repoFolderPath, string documentName, string targetFolder, string targetFileName, int version)
        {
            StoredObject item = GetFolderDocument(repoFolderPath, documentName);
            if (item != null)
            {
                RepositoryDocumentBase doc = (RepositoryDocumentBase)item;
                string fileName = (string.IsNullOrEmpty(targetFileName)) ? GetDocumentFileName(targetFolder, doc) : Path.Combine(targetFolder, targetFileName);
                _ = doc.CheckOutOldVersionToFile(version.ToString(), fileName, (int)SRmgMergeMode.SRmgMergeOverwrite, false, out _, out _);

                // Trigger checked out event
                OnDocumentCheckedOut(new CheckOutEventArgs() { CheckOutFileName = fileName, DocumentName = doc.Name });
            }
        }

        /// <summary>
        /// Checks out the documents in the specified repository folder and saves them in the target folder. 
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the documents.</param>
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the documents.</param>
        /// <param name="recursive">True to also check out the documents in any sub-folder of the <paramref name="repoFolderPath"/>.</param>     
        /// <param name="preserveFolderStructure">True to mimic the repository folder structure on the local disc when checking out. Applies to recursive check-outs only.</param>
        public void CheckOutFolderDocuments(string repoFolderPath, string targetFolder, bool recursive, bool preserveFolderStructure)
        {
            List<StoredObject> folderDocs = GetFolderDocuments(repoFolderPath);            
            foreach (StoredObject folderDoc in folderDocs)
            {
                switch (folderDoc.ClassKind)
                {
                    case (int)PdRMG_Classes.cls_RepositoryFolder:                        
                        if (recursive)
                        {
                            RepositoryFolder folder = (RepositoryFolder)folderDoc;
                            string localTargetFolder = preserveFolderStructure ? Path.Combine(targetFolder, folder.Name) : targetFolder;
                            CheckOutFolderDocuments(folder.Location.Substring(1) + "/" + folder.Name, localTargetFolder, recursive, preserveFolderStructure);
                        }
                        break;
                    default:                        
                        // Check out file
                        RepositoryDocumentBase doc = (RepositoryDocumentBase)folderDoc;
                        string fileName = GetDocumentFileName(targetFolder, doc);
                        _ = doc.CheckOutToFile(fileName, (int)SRmgMergeMode.SRmgMergeOverwrite, false, out _, out _);

                        // Trigger checked out event
                        OnDocumentCheckedOut(new CheckOutEventArgs() { CheckOutFileName = fileName, DocumentName = doc.Name });
                        break;
                }
            }            
        }        

        /// <summary>
        /// Freezes a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to freeze.</param>
        /// <param name="comment">Freeze comment.</param>
        /// <returns>True if successful, False if not.</returns>
        public bool FreezeFolderDocument(string repoFolderPath, string documentName, string comment)
        {
            bool result = false;
            StoredObject item = GetFolderDocument(repoFolderPath, documentName);
            if (item != null)
            {
                RepositoryDocumentBase doc = (RepositoryDocumentBase)item;
                result = doc.Freeze(comment);
            }
            return result;
        }

        /// <summary>
        /// Unfreezes a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to unfreeze.</param>        
        /// <returns>True if successful, False if not.</returns>
        public bool UnfreezeFolderDocument(string repoFolderPath, string documentName)
        {
            bool result = false;
            StoredObject item = GetFolderDocument(repoFolderPath, documentName);
            if (item != null)
            {
                RepositoryDocumentBase doc = (RepositoryDocumentBase)item;
                result = doc.Unfreeze();
            }
            return result;
        }

        /// <summary>
        /// Locks a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to lock.</param>
        /// <param name="comment">Lock comment.</param>
        /// <returns>True if successful, False if not.</returns>
        public bool LockFolderDocument(string repoFolderPath, string documentName, string comment)
        {
            bool result = false;
            StoredObject item = GetFolderDocument(repoFolderPath, documentName);
            if (item != null)
            {
                RepositoryDocumentBase doc = (RepositoryDocumentBase)item;
                result = doc.Lock(comment);
            }
            return result;
        }

        /// <summary>
        /// Unlocks a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to unlock.</param>        
        /// <returns>True if successful, False if not.</returns>
        public bool UnlockFolderDocument(string repoFolderPath, string documentName)
        {
            bool result = false;
            StoredObject item = GetFolderDocument(repoFolderPath, documentName);
            if (item != null)
            {
                RepositoryDocumentBase doc = (RepositoryDocumentBase)item;
                result = doc.Unlock();
            }
            return result;
        }

        #endregion

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

        /// <summary>
        /// Returns a list of <see cref="Document"/> objects in the specified path.        
        /// </summary>
        /// <param name="folderPath">The repository folder from which to retrieve the documents.</param>
        /// <param name="recursive">True to also list documents in any sub-folder of the <paramref name="folderPath"/>.</param>
        /// <param name="documents">A ref to the List of found <see cref="Document"/> types.</param>
        /// <returns>A List with <see cref="Document"/> objects.</returns> 
        private List<Document> ListFolderDocuments(string folderPath, bool recursive, ref List<Document> documents)
        {
            List<StoredObject> folderItems = GetFolderDocuments(folderPath);
            foreach (StoredObject item in folderItems)
            {
                switch (item.ClassKind)
                {
                    case (int)PdRMG_Classes.cls_RepositoryFolder:
                        if (recursive)
                        {
                            RepositoryFolder folder = (RepositoryFolder)item;
                            ListFolderDocuments(folder.Location.Substring(1) + "/" + folder.Name, recursive, ref documents);
                        }
                        break;
                    default:
                        Document document = ParseStoredObjectInfo(item);
                        if (document != null)
                        {
                            documents.Add(document);
                        }
                        break;
                }

            }
            return documents;
        }

        /// <summary>
        /// Retrieves the specified folder document.
        /// </summary>
        /// <param name="folderPath">The repository folder from which to retrieve the documents.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <returns>A <see cref="StoredObject"/> type.</returns>
        private StoredObject GetFolderDocument(string folderPath, string documentName)
        {
            StoredObject storedObject = null;
            RepositoryFolder repositoryFolder = GetRepositoryFolder(folderPath);
            if (repositoryFolder != null)
            {
                storedObject = (StoredObject)repositoryFolder.FindChildByPath(documentName, (int)PdRMG_Classes.cls_StoredObject);
            }
            return storedObject;
        }

        /// <summary>
        /// Retrieves a list of folder documents.
        /// </summary>
        /// <param name="folderPath">The repository folder from which to retrieve the documents.</param>
        /// <returns>A List of <see cref="StoredObject"/> types.</returns>
        private List<StoredObject> GetFolderDocuments(string folderPath)
        {
            List<StoredObject> storedObjects = new List<StoredObject>();
            RepositoryFolder repositoryFolder = GetRepositoryFolder(folderPath);
            if (repositoryFolder != null)
            {
                storedObjects = repositoryFolder.ChildObjects.Cast<StoredObject>().ToList<StoredObject>();                            
            }
            return storedObjects;
        }

        /// <summary>
        /// Returns the document file name.
        /// </summary>
        /// <param name="targetFolder">A file folder on disc.</param>
        /// <param name="storedObject">A <see cref="StoredObject"/> type.</param>
        /// <returns>A fully-qualified file name.</returns>
        private static string GetDocumentFileName(string targetFolder, StoredObject storedObject)
        {
            Document info = ParseStoredObjectInfo(storedObject);
            return Path.Combine(targetFolder, info.ExtractionFileName);            
        }

        /// <summary>
        /// Reads information from the passed stored object and returns it as a <see cref="Document"/> type.
        /// </summary>
        /// <param name="item">A <see cref="StoredObject"/> type.</param>
        /// <returns>A <see cref="Document"/> type.</returns>
        private static Document ParseStoredObjectInfo(StoredObject item)
        {
            Document document = null;
            switch (item.ClassKind)
            {
                case (int)PdRMG_Classes.cls_RepositoryDocument:
                    RepositoryDocument doc = (RepositoryDocument)item;
                    document = new Document()
                    {
                        ClassName = doc.ClassName,
                        ExtractionFileName = doc.Name,
                        Location = doc.Location,
                        IsFrozen = Convert.ToBoolean(doc.Frozen),
                        IsLocked = doc.ObjectStatus != 0,
                        ObjectType = doc.ObjectType,
                        Name = doc.Name,
                        Version = doc.Version,
                        VersionComment = doc.VersionComment.Trim()
                    };
                    break;
                case (int)PdRMG_Classes.cls_RepositoryModel:
                    RepositoryModel mdl = (RepositoryModel)item;
                    document = new Document()
                    {
                        ClassName = mdl.ClassName,
                        ExtractionFileName = ParseModelFileName(mdl),
                        Location = mdl.Location,
                        IsFrozen = Convert.ToBoolean(mdl.Frozen),
                        IsLocked = mdl.ObjectStatus != 0,
                        ObjectType = mdl.ObjectType,
                        Name = mdl.Name,
                        Version = mdl.Version,
                        VersionComment = mdl.VersionComment.Trim()
                    };
                    break;
            }
            return document;
        }

        /// <summary>
        /// Returns the file name of the passed repository model.
        /// </summary>
        /// <param name="model">A <see cref="RepositoryModel"/> type.</param>
        /// <returns>The file name of the model.</returns>
        private static string ParseModelFileName(RepositoryModel model)
        {
            string fileName = string.Empty;
            switch (model.ClassName.ToLower())
            {
                case "dbms":
                    fileName = model.Name + ".xdb";
                    break;
                case "extension":
                    fileName = model.Name + ".xem";
                    break;
                case "free model":
                    fileName = model.Name + ".fem";
                    break;
                case "glossary model":
                    fileName = model.Name + ".glm";
                    break;
                case "logical data model":
                    fileName = model.Name + ".ldm";
                    break;
                case "physical data model":
                    fileName = model.Name + ".pdm";
                    break;
                case "report language":
                    fileName = model.Name + ".xrl";
                    break;
                case "requirements model":
                    fileName = model.Name + ".rqm";
                    break;
                case "xml model":
                    fileName = model.Name + ".xsm";
                    break;
            }
            return fileName;
        }

        #endregion

        #region Events

        protected virtual void OnDocumentCheckedOut(CheckOutEventArgs args)
        {
            RepoDocumentCheckedOut?.Invoke(this, args);
        }

        #endregion
    }
}
