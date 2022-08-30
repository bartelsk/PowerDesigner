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

        protected static void ThrowArgumentNullException(string argumentName)
        {
            throw new ArgumentNullException(argumentName, $"Parameter '{ argumentName }' cannot be null or empty.");
        }

        protected static void ThrowFolderNotFoundException(string documentFolder)
        {
            throw new RepositoryException($"The folder '{ documentFolder }' does not exist.");
        }

        protected static void ThrowDocumentNotFoundException(string documentName, string documentFolder)
        {
            throw new RepositoryException($"The document '{ documentName }' does not exist in folder '{ documentFolder }'.");
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
        /// Returns a <see cref="RepositoryBranchFolder"/> instance.
        /// </summary>
        /// <param name="branchFolderPath">The location of the branch folder in the repository.</param>
        /// <returns>A <see cref="RepositoryBranchFolder"/> type.</returns>
        public RepositoryBranchFolder GetRepositoryBranchFolder(string branchFolderPath)
        {
            RepositoryBranchFolder folder = null;
            BaseObject baseFolder = _con.Connection.FindChildByPath(branchFolderPath, (int)PdRMG_Classes.cls_RepositoryBranchFolder);
            if (baseFolder != null)
            {
                folder = (RepositoryBranchFolder)baseFolder;
            }
            return folder;
        }       

        #endregion

        #region Branches        

        /// <summary>
        /// Creates a new branch of an existing repository branch. 
        /// The source branch's document hierarchy will be duplicated in the new branch.
        /// </summary>
        /// <param name="sourceBranchFolder">The location of the source branch folder in the repository.</param>
        /// <param name="newBranchName">The name for the new branch.</param>
        /// <param name="branchPermission">Permission settings for the new branch. If omitted, the permissions of the currently connected account will be applied.</param>
        /// <remarks>The branch creation can fail for several reasons:
        /// - The currently connected account does not have Write permissions on the folder
        /// - The currently connected account does not have the Manage Branches privilege
        /// - The source branch folder already belongs to a branch (sub-branches are not supported).
        /// Be aware that PowerDesigner does not throw an exception in any of these cases.
        /// </remarks>
        public void CreateNewBranch(string sourceBranchFolder, string newBranchName, Permission branchPermission = null)
        {
            RepositoryBranchFolder sourceBranch = GetRepositoryBranchFolder(sourceBranchFolder);
            if (sourceBranch != null)
            {
                // Check if branch exists already
                if (GetRepositoryBranchFolder(sourceBranch.Location.Substring(1) + "/" + newBranchName) != null)
                    throw new RepositoryException($"A branch named '{ newBranchName }' already exists.");

                // Get parent folder and create new branch in that folder
                RepositoryFolder targetRepoFolder = (RepositoryFolder)sourceBranch.Parent;
                BaseObject newBranch = targetRepoFolder.CreateBranch(newBranchName, sourceBranch.DisplayName);

                // Set branch permission
                if (newBranch != null && branchPermission != null)
                {                    
                    RepositoryBranchFolder newBranchFolder = (RepositoryBranchFolder)newBranch;                    
                    newBranchFolder.SetPermission(ParseUserOrGroup(branchPermission.UserOrGroupName), ((int)branchPermission.PermissionType), branchPermission.CopyToChildren);  
                }
            }
        }

        /// <summary>
        /// Returns a list of <see cref="Branch"/> objects, relative to the specified root folder.
        /// </summary>
        /// <param name="rootFolder">The repository folder from which to start the search.</param>
        /// <param name="userOrGroupNameFilter">A user login or group name used to filter branches based on access permission (optional).</param>
        /// <returns>A List with <see cref="Branch"/> objects.</returns>
        public List<Branch> GetBranchFolders(string rootFolder, string userOrGroupNameFilter = null)
        {
            List<Branch> branches = new List<Branch>();
            RepositoryFolder repositoryFolder = GetRepositoryFolder(rootFolder);
            if (repositoryFolder != null)
            {
                ListBranches(repositoryFolder, ref branches, string.Empty, string.IsNullOrEmpty(userOrGroupNameFilter) ? null : ParseUserOrGroup(userOrGroupNameFilter));
            }
            return branches;
        }

        /// <summary>
        /// Retrieves the permission on a repository branch for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The location of the branch folder in the repository.</param>
        /// <param name="branchName">The name of the branch.</param>
        /// <param name="userOrGroupName">The user login or group name for which to check its permission.</param>
        /// <returns>A <see cref="PermissionTypeEnum"/> type.</returns>
        public PermissionTypeEnum GetBranchPermission(string repoFolderPath, string branchName, string userOrGroupName)
        {
            RepositoryBranchFolder branchFolder = GetRepositoryBranchFolder(repoFolderPath + "/" + branchName);            

            int permission = branchFolder.GetPermission(ParseUserOrGroup(userOrGroupName));
            return ParsePermission(permission.ToString());
        }

        /// <summary>
        /// Grants permissions to a repository branch for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The location of the branch folder in the repository.</param>
        /// <param name="branchName">The name of the branch.</param>
        /// <param name="permission">The <see cref="Permission"/> that is to be granted to the branch.</param>
        /// <returns>True if successful, False if not.</returns>
        public bool SetBranchPermission(string repoFolderPath, string branchName, Permission permission)
        {
            RepositoryBranchFolder branchFolder = GetRepositoryBranchFolder(repoFolderPath + "/" + branchName);
            return branchFolder.SetPermission(ParseUserOrGroup(permission.UserOrGroupName), (int)permission.PermissionType, permission.CopyToChildren);
        }

        /// <summary>
        /// Deletes all permissions from a repository branch for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The location of the branch folder in the repository.</param>
        /// <param name="branchName">The name of the branch.</param>
        /// <param name="permission">A <see cref="Permission"/> type that specifies the user login or group name and whether to remove the permissions from all child objects as well (if any).</param>
        /// <returns>True if successful, False if not.</returns>
        public bool DeleteBranchPermission(string repoFolderPath, string branchName, Permission permission)
        {
            RepositoryBranchFolder branchFolder = GetRepositoryBranchFolder(repoFolderPath + "/" + branchName);
            return branchFolder.DeletePermission(ParseUserOrGroup(permission.UserOrGroupName), permission.CopyToChildren);
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
            StoredObject item = GetFolderDocument(folderPath, documentName);                        
            return ParseStoredObjectInfo(item);                                    
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

        public void CheckInFolderDocument(string repoFolderPath, string fileName)
        {
            StoredObject item = GetFolder(repoFolderPath);
            RepositoryFolder folder = (RepositoryFolder)item;
            BaseObject obj = folder.CheckInDocument(fileName, (int)SRmgMergeMode.SRmgMergeOverwrite, out string actions, out string conflicts, string.Empty, out BaseObject changeList);

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
            RepositoryDocumentBase doc = (RepositoryDocumentBase)item;

            string fileName = (string.IsNullOrEmpty(targetFileName)) ? GetDocumentFileName(targetFolder, doc) : Path.Combine(targetFolder, targetFileName);
            _ = doc.CheckOutToFile(fileName, (int)SRmgMergeMode.SRmgMergeOverwrite, false, out _, out _);

            // Trigger checked out event
            OnDocumentCheckedOut(new CheckOutEventArgs() { CheckOutFileName = fileName, DocumentName = doc.Name });            
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
            RepositoryDocumentBase doc = (RepositoryDocumentBase)item;

            string fileName = (string.IsNullOrEmpty(targetFileName)) ? GetDocumentFileName(targetFolder, doc) : Path.Combine(targetFolder, targetFileName);
            _ = doc.CheckOutOldVersionToFile(version.ToString(), fileName, (int)SRmgMergeMode.SRmgMergeOverwrite, false, out _, out _);

            // Trigger checked out event
            OnDocumentCheckedOut(new CheckOutEventArgs() { CheckOutFileName = fileName, DocumentName = doc.Name });            
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
        /// <returns>True if successful, False if not (the document may already be frozen).</returns>
        public bool FreezeFolderDocument(string repoFolderPath, string documentName, string comment)
        {            
            StoredObject item = GetFolderDocument(repoFolderPath, documentName);
            RepositoryDocumentBase doc = (RepositoryDocumentBase)item;
            return doc.Freeze(comment);            
        }

        /// <summary>
        /// Unfreezes a repository document, making it updateable.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to unfreeze.</param>        
        /// <returns>True if successful, False if not (the document may already be updateable).</returns>
        public bool UnfreezeFolderDocument(string repoFolderPath, string documentName)
        {            
            StoredObject item = GetFolderDocument(repoFolderPath, documentName);            
            RepositoryDocumentBase doc = (RepositoryDocumentBase)item;
            return doc.Unfreeze();           
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
            StoredObject item = GetFolderDocument(repoFolderPath, documentName);
            RepositoryDocumentBase doc = (RepositoryDocumentBase)item;
            return doc.Lock(comment);            
        }

        /// <summary>
        /// Unlocks a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to unlock.</param>        
        /// <returns>True if successful, False if not.</returns>
        public bool UnlockFolderDocument(string repoFolderPath, string documentName)
        {
            StoredObject item = GetFolderDocument(repoFolderPath, documentName);            
            RepositoryDocumentBase doc = (RepositoryDocumentBase)item;
            return doc.Unlock();            
        }

        /// <summary>
        /// Retrieves the permission on a repository document for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <param name="userOrGroupName">The user login or group name for which to check its permission.</param>
        /// <returns>A <see cref="PermissionTypeEnum"/> type.</returns>
        public PermissionTypeEnum GetDocumentPermission(string repoFolderPath, string documentName, string userOrGroupName)
        {            
            StoredObject item = GetFolderDocument(repoFolderPath, documentName);
            RepositoryDocumentBase doc = (RepositoryDocumentBase)item;

            int permission = doc.GetPermission(ParseUserOrGroup(userOrGroupName));
            return ParsePermission(permission.ToString());            
        }

        /// <summary>
        /// Grants permissions to a repository document for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <param name="permission">The <see cref="Permission"/> that is to be granted to the folder document.</param>
        /// <returns>True if successful, False if not.</returns>
        public bool SetDocumentPermission(string repoFolderPath, string documentName, Permission permission)
        {
            StoredObject item = GetFolderDocument(repoFolderPath, documentName);
            RepositoryDocumentBase doc = (RepositoryDocumentBase)item;

            return doc.SetPermission(ParseUserOrGroup(permission.UserOrGroupName), (int)permission.PermissionType, permission.CopyToChildren);
        }

        /// <summary>
        /// Deletes all permissions from a repository document for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <param name="permission">A <see cref="Permission"/> type that specifies the user login or group name and whether to remove the permissions from all child objects as well (if any).</param>
        /// <returns>True if successful, False if not.</returns>
        public bool DeleteDocumentPermission(string repoFolderPath, string documentName, Permission permission)
        {
            StoredObject item = GetFolderDocument(repoFolderPath, documentName);
            RepositoryDocumentBase doc = (RepositoryDocumentBase)item;

            return doc.DeletePermission(ParseUserOrGroup(permission.UserOrGroupName), permission.CopyToChildren);
        }

        #endregion

        #endregion

        #region Private methods

        /// <summary>
        /// Generatic method for retrieving a repository (branch) folder.
        /// </summary>
        /// <param name="folderPath">A repository folder path.</param>        
        /// <returns>A <see cref="StoredObject"/> type that represents the folder.</returns>
        private StoredObject GetFolder(string folderPath)
        {
            StoredObject storedObject;
            storedObject = GetRepositoryFolder(folderPath);
            if (storedObject == null)
            {
                storedObject = GetRepositoryBranchFolder(folderPath);
                if (storedObject == null) ThrowFolderNotFoundException(folderPath);
            }
            return storedObject;
        }

        /// <summary>
        /// Retrieves the specified folder document.
        /// </summary>
        /// <param name="folderPath">The folder from which to retrieve the documents.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <returns>A <see cref="StoredObject"/> type.</returns>
        private StoredObject GetFolderDocument(string folderPath, string documentName)
        {
            StoredObject document = (StoredObject)GetFolder(folderPath).FindChildByPath(documentName, (int)PdRMG_Classes.cls_StoredObject);
            if (document == null) ThrowDocumentNotFoundException(documentName, folderPath);
            return document;
        }

        /// <summary>
        /// Retrieves a list of folder documents.
        /// </summary>
        /// <param name="folderPath">The repository folder from which to retrieve the documents.</param>
        /// <returns>A List of <see cref="StoredObject"/> types.</returns>
        private List<StoredObject> GetFolderDocuments(string folderPath)
        {
            StoredObject repositoryFolder = GetFolder(folderPath);
            return repositoryFolder.ChildObjects.Cast<StoredObject>().ToList<StoredObject>();
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
        /// Recursively retrieves branch folders from the repository starting at the specified root folder.
        /// </summary>
        /// <param name="rootFolder">The repository folder from which to start the search.</param>
        /// <param name="branches">A List type that will contain the encountered branch folders.</param>
        /// <param name="location">Used to track the current folder location in the recursion process.</param>
        /// <param name="user">Used to filter branches based on the permission of the specified user or group.</param>
        private static void ListBranches(StoredObject rootFolder, ref List<Branch> branches, string location, BaseObject user = null)
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
                                ListBranches(child, ref branches, (string.IsNullOrEmpty(location) ? rootFolder.Name + "/" + folder.Name : location + "/" + rootFolder.Name + "/" + folder.Name), user);
                            }
                            break;
                        case (int)PdRMG_Classes.cls_RepositoryBranchFolder:
                            RepositoryBranchFolder branchFolder = (RepositoryBranchFolder)item;
                            if (user != null)
                            {
                                // Filter branches for specified user
                                PermissionTypeEnum branchPermission = ParsePermission(branchFolder.GetPermission(user).ToString());
                                if (branchPermission != PermissionTypeEnum.NotSet)
                                {
                                    branches.Add(new Branch()
                                    {
                                        Name = branchFolder.DisplayName,
                                        Permission = branchPermission,
                                        RelativePath = (string.IsNullOrEmpty(location) ? rootFolder.Name : location + "/" + rootFolder.Name)
                                    });
                                }
                            }
                            else
                            {
                                branches.Add(new Branch()
                                {
                                    Name = branchFolder.DisplayName,
                                    RelativePath = (string.IsNullOrEmpty(location) ? rootFolder.Name : location + "/" + rootFolder.Name)
                                });
                            }
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
        /// Tries to parse the specified user or group name.
        /// </summary>
        /// <param name="userOrGroupName">A user login name or group name.</param>
        /// <returns>A <see cref="BaseObject"/> which represents the user or group.</returns>
        private BaseObject ParseUserOrGroup(string userOrGroupName)
        {
            BaseObject repoUser = _con.Connection.GetUser(userOrGroupName);
            if (repoUser == null)
            {
                repoUser = _con.Connection.GetGroup(userOrGroupName);
                if (repoUser == null)
                {
                    throw new UnknownUserOrGroupException($"The user or group with name '{ userOrGroupName }' does not exist in the repository.");
                }
            }
            return repoUser;
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

        /// <summary>
        /// Tries to parse the specified permission into a valid PermissionType enum.
        /// </summary>
        /// <param name="permission">The permission to parse.</param>
        /// <returns>A <see cref="PermissionTypeEnum"/> enum.</returns>
        private static PermissionTypeEnum ParsePermission(string permission)
        {
            if (!Enum.TryParse<PermissionTypeEnum>(permission, out PermissionTypeEnum result))
                throw new InvalidPermissionException("Invalid permission");
            return result;
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
