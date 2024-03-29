﻿// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using PDRepository.Exceptions;
using PdRMG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PDRepository
{
    /// <summary>
    /// Abstract class that provides methods to interact with the PowerDesigner repository.
    /// </summary>
    public abstract class Repository : IDisposable
    {
        private bool disposedValue;
        internal readonly RepositoryConnection _con;
        public event EventHandler<CheckInEventArgs> RepoDocumentCheckedIn;
        public event EventHandler<CheckOutEventArgs> RepoDocumentCheckedOut;

        #region Constructor / Destructor

        protected Repository(ConnectionSettings settings)
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

        protected static void ThrowFileNotFoundException(string fileName)
        {
            throw new FileNotFoundException($"The file '{ fileName }' does not exist.");
        }

        protected static void ThrowDirectoryNotFoundException(string directoryName)
        {
            throw new DirectoryNotFoundException($"The directory '{ directoryName }' does not exist.");
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
        /// Creates a repository connection with the current repository <see cref="ConnectionSettings"/>.
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
        /// Refreshes the repository connection.
        /// </summary>
        public void Refresh()
        {
            _con.Refresh();
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

        /// <summary>
        /// Creates a repository folder.
        /// </summary>
        /// <param name="folderPath">The parent repository folder path.</param>
        /// <param name="folderName">The name of the new folder.</param>
        /// <returns>True if successful, False if not.</returns>
        public bool CreateRepositoryFolder(string folderPath, string folderName)
        {
            BaseObject newFolder = null;
            string newFolderPath = folderPath + "/" + folderName;
            
            if (FolderExists(newFolderPath))
                throw new RepositoryException($"Folder '{ newFolderPath }' already exists.");

            StoredObject folderObject = GetFolder(folderPath);
            switch (folderObject.ClassKind)
            {
                case (int)PdRMG_Classes.cls_RepositoryFolder:
                    RepositoryFolder folder = (RepositoryFolder)folderObject;
                    newFolder = folder.CreateFolder(folderName);
                    break;
                case (int)PdRMG_Classes.cls_RepositoryBranchFolder:
                    RepositoryBranchFolder branchFolder = (RepositoryBranchFolder)folderObject;
                    newFolder = branchFolder.CreateFolder(folderName);   
                    break;
            }
            return newFolder != null;
        }

        /// <summary>
        /// Deletes a repository folder. 
        /// This method cannot be used to delete a repository branch folder.
        /// </summary>
        /// <remarks>
        /// The deletion may fail if the folder is not empty.
        /// </remarks>
        /// <param name="folderPath">A repository folder path.</param>        
        /// <returns>True if successful, False if not.</returns>
        public bool DeleteRepositoryFolder(string folderPath)
        {
            RepositoryFolder folder = GetRepositoryFolder(folderPath);
            if (folder == null)
                ThrowFolderNotFoundException(folderPath);

            if (folder.ChildObjects.Count > 0)
            {
                return false;
            }
            else
            {
                return folder.DeleteEmptyFolder();
            }
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
        /// Determines the existence of a repository folder.
        /// </summary>
        /// <param name="folderPath">A repository folder path.</param>        
        /// <returns>True if the folder exists, False if not.</returns>
        public bool DocumentFolderExists(string folderPath)
        {
            return FolderExists(folderPath);
        }

        /// <summary>
        /// Determines whether the specified document exists in the specified repository folder.
        /// </summary>
        /// <param name="folderPath">The repository folder in which the document should exist.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <returns>True if the document exists in the repository folder, False if not.</returns>
        public bool FolderDocumentExists(string folderPath, string documentName)
        {
            bool documentExists = false;
            if (FolderExists(folderPath))
            {
                StoredObject document = (StoredObject)GetFolder(folderPath).FindChildByPath(documentName, (int)PdRMG_Classes.cls_StoredObject);
                documentExists = document != null;
            }
            return documentExists;
        }

        /// <summary>
        /// Retrieves information on a document in the specified repository folder.
        /// </summary>
        /// <param name="folderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <returns>A <see cref="Document"/> type.</returns>
        public Document GetFolderDocumentInfo(string folderPath, string documentName)
        {            
            StoredObject document = (StoredObject)GetFolder(folderPath).FindChildByPath(documentName, (int)PdRMG_Classes.cls_StoredObject);
            if (document == null) ThrowDocumentNotFoundException(documentName, folderPath);
            return ParseStoredObjectInfo(document);                                    
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
        /// Adds a file to the specified repository folder. Overwrites the existing document (if any) and freezes it.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder in which to add the file.</param>
        /// <param name="fileName">The fully-qualified name of the file.</param>
        /// <param name="documentVersion">Contains the current document version number if the check-in was successful.</param>
        public void CheckInFolderDocument(string repoFolderPath, string fileName, out string documentVersion)
        {
            StoredObject item = GetFolder(repoFolderPath);
            RepositoryFolder folder = (RepositoryFolder)item;
            BaseObject obj = folder.CheckInDocument(fileName, (int)SRmgMergeMode.SRmgMergeOverwrite, out _, out _, string.Empty, out _);

            if (obj == null)
                throw new RepositoryException($"Failed to check in file '{ fileName }'.");

            StoredObject newDocument = (StoredObject)obj;
            Document document = ParseStoredObjectInfo(newDocument);
            documentVersion = document.Version;

            // Trigger checked in event
            OnDocumentCheckedIn(new CheckInEventArgs() { CheckInFileName = fileName, DocumentFolder = document.Location, DocumentName = document.Name, DocumentVersion = document.Version });
        }

        /// <summary>
        /// Adds files to the specified repository folder. Overwrites the existing documents (if any) and freezes them.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder in which to add the files. Will get overridden if the model was part of a repository branch already.</param>
        /// <param name="sourceFolder">The folder on disc that contains the files you want to add.</param>        
        public void CheckInFolderDocuments(string repoFolderPath, string sourceFolder)
        {
            string[] folderFiles = Directory.GetFiles(sourceFolder, "*.*").Where(file => Path.GetExtension(file).ToLower() != ".pdb").ToArray(); 
            foreach (string file in folderFiles)
            {
                CheckInFolderDocument(repoFolderPath, file, out _);
            }
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
            OnDocumentCheckedOut(new CheckOutEventArgs() { CheckOutFileName = fileName, DocumentName = doc.Name, DocumentVersion = doc.Version });            
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
            OnDocumentCheckedOut(new CheckOutEventArgs() { CheckOutFileName = fileName, DocumentName = doc.Name, DocumentVersion = doc.Version });            
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
                        OnDocumentCheckedOut(new CheckOutEventArgs() { CheckOutFileName = fileName, DocumentName = doc.Name, DocumentVersion = doc.Version });
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
            RepositoryDocumentBase doc = GetFolderDocument(repoFolderPath, documentName);
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
            RepositoryDocumentBase doc = GetFolderDocument(repoFolderPath, documentName);            
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
            RepositoryDocumentBase doc = GetFolderDocument(repoFolderPath, documentName);
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
            RepositoryDocumentBase doc = GetFolderDocument(repoFolderPath, documentName);
            return doc.Unlock();            
        }

        /// <summary>
        /// Completely removes a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to remove completely.</param>        
        /// <returns>True if successful, False if not.</returns>
        public bool DeleteFolderDocument(string repoFolderPath, string documentName)
        {
            RepositoryDocumentBase doc = GetFolderDocument(repoFolderPath, documentName);
            return doc.DeleteAllVersions();
        }

        /// <summary>
        /// Removes the current version of a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document.</param>   
        /// <returns>True if successful, False if not.</returns>
        public bool DeleteFolderDocumentVersion(string repoFolderPath, string documentName)
        {
            RepositoryDocumentBase doc = GetFolderDocument(repoFolderPath, documentName);
            return doc.DeleteVersion();
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
            RepositoryDocumentBase doc = GetFolderDocument(repoFolderPath, documentName);   
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
            RepositoryDocumentBase doc = GetFolderDocument(repoFolderPath, documentName); 
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
            RepositoryDocumentBase doc = GetFolderDocument(repoFolderPath, documentName);
            return doc.DeletePermission(ParseUserOrGroup(permission.UserOrGroupName), permission.CopyToChildren);
        }

        #endregion

        #region Users / Groups

        /// <summary>
        /// Returns a repository user.
        /// </summary>
        /// <param name="loginName">The login name of the user.</param>
        /// <returns>A <see cref="User"/> type.</returns>
        public User GetRepositoryUser(string loginName)
        {             
            RepositoryUser repoUser = GetUser(loginName);
            if (repoUser == null)
                throw new RepositoryException($"A user with login name '{ loginName }' does not exist.");

            return ParseUserDetails(repoUser);
        }

        /// <summary>
        /// Lists the available users.
        /// </summary>
        /// <returns>A List with <see cref="User"/> types.</returns>
        public List<User> GetRepositoryUsers()
        {
            List<User> users = new List<User>();
            ObjectCol allUsers = _con.Connection.Users;
            foreach (RepositoryUser repoUser in allUsers)
            {   
                users.Add(GetRepositoryUser(repoUser.LoginName));                
            }
            return users;           
        }

        /// <summary>
        /// Determines whether a repository user exists.
        /// </summary>
        /// <param name="loginName">The login name of the user.</param>
        /// <returns>True if the user exists, False if not.</returns>
        public bool RepositoryUserExists(string loginName)
        {            
            return GetUser(loginName) != null;
        }

        /// <summary>
        /// Creates a repository user and assigns the specified rights.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="fullName">The real name of the user.</param>
        /// <param name="emailAddress">The email address of the user (optional).</param>
        /// <param name="temporaryPassword">Contains the temporary password of the newly created user.</param>
        /// <param name="rights">A <see cref="UserOrGroupRightsEnum"/> type.</param>
        /// <param name="groupName">The name of the group to which to add the user (optional).</param>
        public void CreateRepositoryUser(string loginName, string fullName, string emailAddress, out string temporaryPassword, UserOrGroupRightsEnum rights, string groupName)
        {
            if (RepositoryUserExists(loginName))
                throw new RepositoryException($"A user with login name '{ loginName }' already exists.");

            if (!string.IsNullOrEmpty(groupName) && !RepositoryGroupExists(groupName))
                throw new RepositoryException($"A group with name '{ groupName }' does not exist.");

            temporaryPassword = _con.Connection.GeneratePassword();
            BaseObject newUser = _con.Connection.CreateUser();
            if (newUser != null)
            {
                RepositoryUser user = (RepositoryUser)newUser;
                user.LoginName = loginName;
                user.FullName = fullName;
                user.EmailAddress = emailAddress;                
                user.SetPassword(temporaryPassword);
                user.Rights = (int)rights;

                if (!string.IsNullOrEmpty(groupName))
                {
                    RepositoryGroup repoGroup = GetGroup(groupName);
                    if (repoGroup != null)
                    {
                        repoGroup.AddMember(user.LoginName);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a repository user to a repository group.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="groupName">The name of the group to which to add the user.</param>
        public void AddUserToRepositoryGroup(string loginName, string groupName)
        {
            RepositoryUser repoUser = GetUser(loginName);
            if (repoUser == null)
                throw new RepositoryException($"A user with login name '{ loginName }' does not exist.");

            RepositoryGroup repoGroup = GetGroup(groupName);
            if (repoGroup == null)
                throw new RepositoryException($"A group with name '{ groupName }' does not exist.");

            repoGroup.AddMember(repoUser.LoginName);
        }

        /// <summary>
        /// Removes a repository user from a repository group.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="groupName">The name of the group from which to remove the user.</param>
        public void RemoveUserFromRepositoryGroup(string loginName, string groupName)
        {
            RepositoryUser repoUser = GetUser(loginName);
            if (repoUser == null)
                throw new RepositoryException($"A user with login name '{ loginName }' does not exist.");

            RepositoryGroup repoGroup = GetGroup(groupName);
            if (repoGroup == null)
                throw new RepositoryException($"A group with name '{ groupName }' does not exist.");

            repoGroup.RemoveMember(repoUser.LoginName);
        }

        /// <summary>
        /// Returns a list of <see cref="Group"/> objects of which the specified user is a member.       
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <returns>A List with <see cref="Group"/> objects.</returns> 
        public List<Group> GetRepositoryUserGroups(string loginName)
        {
            List<Group> userGroups = new List<Group>();

            RepositoryUser repoUser = GetUser(loginName);
            if (repoUser == null)
                throw new RepositoryException($"A user with login name '{ loginName }' does not exist.");

            foreach (RepositoryGroup repoGroup in repoUser.Groups)
            {
                userGroups.Add(new Group() 
                { 
                    Description = repoGroup.ShortDescription,
                    Name = repoGroup.GroupName,
                    Rights = ParseRights(repoGroup.Rights)                
                });                
            }
            return userGroups;            
        }

        /// <summary>
        /// Returns a list of users that are members of a particular group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>A list with <see cref="User"/> objects.</returns>
        public List<User> GetRepositoryGroupMembers(string groupName)
        {
            List<User> members = new List<User>();
            RepositoryGroup repoGroup = GetGroup(groupName);
            if (repoGroup == null)
                throw new RepositoryException($"A group with name '{ groupName }' does not exist.");

            var groupUsers = repoGroup.GroupUsers;
            foreach (RepositoryUser repoUser in groupUsers)
            {
                members.Add(ParseUserDetails(repoUser));
            }
            return members;
        }

        /// <summary>
        /// Checks whether the repository user has the specified right.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="expectedRight">A <see cref="UserOrGroupRightsEnum"/> type. Allows a single value (i.e. right) only.</param>
        /// <returns>True if the repository has the specified right, False if not.</returns>
        public bool DoesUserHaveRight(string loginName, UserOrGroupRightsEnum expectedRight)
        {
            string userRights = GetRepositoryUserRights(loginName);
            return userRights.Contains(ParseRights((int)expectedRight));            
        }

        /// <summary>
        /// Returns the repository user rights as a semi-colon separated string.
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <returns>A string with group rights.</returns>
        public string GetRepositoryUserRights(string loginName)
        {
            RepositoryUser repoUser = GetUser(loginName);
            if (repoUser == null)
                throw new RepositoryException($"A user with login name '{ loginName }' does not exist.");

            int rights = repoUser.Rights;

            // Add inherited rights 
            foreach (RepositoryGroup repoGroup in repoUser.Groups)
            {
                rights |= repoGroup.Rights;                
            }   
            return ParseRights(rights);
        }

        /// <summary>
        /// Assigns the specified rights to a user.
        /// Please note this method does not affect inherited group rights (if any).
        /// </summary>
        /// <param name="loginName">The name with which the user connects to the repository.</param>
        /// <param name="rights">A <see cref="UserOrGroupRightsEnum"/> type.</param>
        /// <param name="replaceExisting">When true, replaces the existing user rights with the specified ones. When false, the specified rights will be added to the existing user rights.</param>
        public void SetRepositoryUserRights(string loginName, UserOrGroupRightsEnum rights, bool replaceExisting)
        {
            RepositoryUser repoUser = GetUser(loginName);
            if (repoUser == null)
                throw new RepositoryException($"A user with login name '{ loginName }' does not exist.");
                        
            if (replaceExisting)
            {
                repoUser.Rights = (int)rights;
            }
            else
            {
                repoUser.Rights |= (int)rights;
            }            
        }

        /// <summary>
        /// Blocks a repository user.
        /// </summary>
        /// <param name="loginName">The login name of the user.</param>  
        public void BlockRepositoryUser(string loginName)
        {
            RepositoryUser repoUser = GetUser(loginName);
            if (repoUser == null)
                throw new RepositoryException($"A user with login name '{ loginName }' does not exist.");

            repoUser.Blocked = true;
        }

        /// <summary>
        /// Unblocks a repository user.
        /// </summary>
        /// <param name="loginName">The login name of the user.</param>        
        public void UnblockRepositoryUser(string loginName)
        {
            RepositoryUser repoUser = GetUser(loginName);
            if (repoUser == null)
                throw new RepositoryException($"A user with login name '{ loginName }' does not exist.");

            repoUser.Blocked = false;
        }

        /// <summary>
        /// Deletes a repository user.
        /// </summary>
        /// <param name="loginName">The login name of the user to delete.</param>
        public void DeleteRepositoryUser(string loginName)
        {
            if (!RepositoryUserExists(loginName))
                throw new RepositoryException($"A user with login name '{ loginName }' does not exist.");

            _con.Connection.DeleteUser(loginName);
        }

        /// <summary>
        /// Resets the specified repository user's password.         
        /// </summary>
        /// <param name="loginName">The login name of the user for which to reset the password.</param>
        /// <returns>Returns a new (temporary) password that complies with the current password policy.</returns>
        public string ResetRepositoryUserPassword(string loginName)
        {            
            if (!RepositoryUserExists(loginName))
                throw new RepositoryException($"A user with login name '{ loginName }' does not exist.");

            // Check whether the currently connected user account is allowed to reset the password.
            if (!DoesUserHaveRight(_con.ConnectedUser, UserOrGroupRightsEnum.ManageUsers))
                throw new RepositoryException($"The user with login name '{ loginName }' does not have the 'Manage Users & Permissions' right necessary to perform this action.");
                        
            RepositoryUser repoUser = GetUser(loginName);
            string newPassword = _con.Connection.GeneratePassword();
            repoUser.SetPassword(newPassword);
            
            return newPassword;
        }

        /// <summary>
        /// Returns a repository group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>A <see cref="Group"/> type.</returns>
        public Group GetRepositoryGroup(string groupName)
        {
            RepositoryGroup repoGroup = GetGroup(groupName);
            if (repoGroup == null)
                throw new RepositoryException($"A group with name '{ repoGroup }' does not exist.");

            return ParseRepoGroup(repoGroup);
        }

        /// <summary>
        /// Lists the available repository groups.
        /// </summary>
        /// <returns>A List with <see cref="Group"/> types.</returns>
        public List<Group> GetRepositoryGroups()
        {
            List<Group> groups = new List<Group>();
            ObjectCol allGroups = _con.Connection.Groups;
            foreach (RepositoryGroup group in allGroups)
            {
                groups.Add(ParseRepoGroup(group));                
            }
            return groups;
        }

        /// <summary>
        /// Determines whether a repository group exists.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>True if the group exists, False if not.</returns>
        public bool RepositoryGroupExists(string groupName)
        {            
            return GetGroup(groupName) != null;
        }

        /// <summary>
        /// Returns the repository group rights as a semi-colon separated string.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>A string with group rights.</returns>
        public string GetRepositoryGroupRights(string groupName)
        {   
            return GetRepositoryGroup(groupName).Rights;
        }

        /// <summary>
        /// Assigns the specified rights to a repository group.
        /// Please note this method does not alter the rights of the individual users in the group (if any).
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="rights">A <see cref="UserOrGroupRightsEnum"/> type.</param>
        /// <param name="replaceExisting">When true, replaces the existing group rights with the specified ones. When false, the specified rights will be added to the existing group rights.</param>
        public void SetRepositoryGroupRights(string groupName, UserOrGroupRightsEnum rights, bool replaceExisting)
        {
            RepositoryGroup repoGroup = GetGroup(groupName);
            if (repoGroup == null)
                throw new RepositoryException($"A group with name '{ repoGroup }' does not exist.");

            if (replaceExisting)
            {
                repoGroup.Rights = (int)rights;
            }
            else
            {
                repoGroup.Rights |= (int)rights;
            }                
        }

        /// <summary>
        /// Creates a repository group and assigns the specified rights.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        /// <param name="rights">A <see cref="UserOrGroupRightsEnum"/> type.</param>
        public void CreateRepositoryGroup(string name, UserOrGroupRightsEnum rights)
        {
            if (RepositoryGroupExists(name))
                throw new RepositoryException($"A group with name '{ name }' already exists.");
            
            BaseObject newGroup = _con.Connection.CreateGroup();
            if (newGroup != null)
            {
                RepositoryGroup group = (RepositoryGroup)newGroup;
                group.GroupName = name;
                group.GroupCode = name;
                group.Rights = (int)rights;
            }            
        }

        /// <summary>
        /// Deletes a repository group.
        /// </summary>
        /// <param name="groupName">The name of the group to delete.</param>
        public void DeleteRepositoryGroup(string groupName)
        {
            if (!RepositoryGroupExists(groupName))
                throw new RepositoryException($"A group with name '{ groupName }' does not exist.");

            _con.Connection.DeleteGroup(groupName);
        }

        #endregion

        #endregion

        #region Private methods

        /// <summary>
        /// Generic method for determining the existence of a repository (branch) folder.
        /// </summary>
        /// <param name="folderPath">A repository folder path.</param>        
        /// <returns>True if the folder exists, False if not.</returns>
        private bool FolderExists(string folderPath)
        {
            StoredObject storedObject;
            storedObject = GetRepositoryFolder(folderPath);
            if (storedObject == null)
            {
                storedObject = GetRepositoryBranchFolder(folderPath);                
            }
            return storedObject != null;
        }

        /// <summary>
        /// Generic method for retrieving a repository (branch) folder.
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
        private RepositoryDocumentBase GetFolderDocument(string folderPath, string documentName)
        {
            StoredObject document = (StoredObject)GetFolder(folderPath).FindChildByPath(documentName, (int)PdRMG_Classes.cls_StoredObject);
            if (document == null) ThrowDocumentNotFoundException(documentName, folderPath);
            return (RepositoryDocumentBase)document;
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
                    case (int)PdRMG_Classes.cls_RepositoryBranchFolder:
                        if (recursive)
                        {
                            RepositoryBranchFolder branchFolder = (RepositoryBranchFolder)item;
                            ListFolderDocuments(branchFolder.Location.Substring(1) + "/" + branchFolder.Name, recursive, ref documents);
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
        /// Returns a repository user.
        /// </summary>
        /// <param name="loginName">The login name of the user.</param>
        /// <returns>A <see cref="RepositoryUser"/> type.</returns>
        private RepositoryUser GetUser(string loginName)
        {
            RepositoryUser user = null;
            BaseObject repoUser = _con.Connection.GetUser(loginName);
            if (repoUser != null)
            {
                user = (RepositoryUser)repoUser;
            }
            return user;
        }

        /// <summary>
        /// Returns a repository group.
        /// </summary>
        /// <param name="groupName">The group name.</param>
        /// <returns>A <see cref="RepositoryGroup"/> type.</returns>
        private RepositoryGroup GetGroup(string groupName)
        {
            RepositoryGroup group = null;
            BaseObject repoGroup = _con.Connection.GetGroup(groupName);
            if (repoGroup != null)
            {
                group = (RepositoryGroup)repoGroup;
            }
            return group;
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
        /// Gets user details, access rights and group memberships.
        /// </summary>
        /// <param name="repoUser">A <see cref="RepositoryUser"/> type.</param>
        /// <returns>A <see cref="User"/> type.</returns>
        private User ParseUserDetails(RepositoryUser repoUser)
        {
            User user = ParseRepoUser(repoUser);
            user.Rights = GetRepositoryUserRights(user.LoginName);

            // Get user group membership
            List<Group> userGroups = GetRepositoryUserGroups(user.LoginName);
            if (userGroups.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                userGroups.ForEach(g => sb.Append($";{g.Name}"));
                user.GroupMembership = sb.ToString().Substring(1);
            }
            return user;
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
        /// Tries to parse a <see cref="RepositoryUser"/> type into a <see cref="User"/> type.
        /// </summary>
        /// <param name="repoUser">A <see cref="RepositoryUser"/> type.</param>
        /// <returns>A <see cref="User"/> type.</returns>
        private static User ParseRepoUser(RepositoryUser repoUser)
        {
            User user = null;
            if (repoUser != null)
            {
                user = new User()
                {
                    Blocked = repoUser.Blocked,
                    Comment = repoUser.Comment,
                    Disabled = repoUser.Disabled,
                    FullName = repoUser.FullName,
                    LastLoginDate = repoUser.LastLoginDate,
                    LastModifiedDate = repoUser.ModificationDateInRepository,
                    LoginName = repoUser.LoginName,                    
                    Status = ParseUserStatus(repoUser.Status)
                };
            }
            return user;
        }

        /// <summary>
        /// Tries to parse a <see cref="RepositoryGroup"/> type into a <see cref="Group"/> type.
        /// </summary>
        /// <param name="repoGroup">A <see cref="RepositoryGroup"/> type.</param>
        /// <returns>A <see cref="Group"/> type.</returns>
        private static Group ParseRepoGroup(RepositoryGroup repoGroup)
        {
            Group group = null;
            if (repoGroup != null)
            {                
                group = new Group()
                { 
                    Description = repoGroup.ShortDescription,
                    Name = repoGroup.Name,
                    Rights = ParseRights(repoGroup.Rights)
                };
            }
            return group;
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

        /// <summary>
        /// Tries to parse the specified user or group rights into a semi-colon separated string of rights.
        /// </summary>
        /// <param name="rights">The user or group rights to parse.</param>
        /// <returns>A string with the parsed result.</returns>
        private static string ParseRights(int rights)
        {
            if (rights < 0) throw new InvalidRightsException("Invalid rights");
            if (rights > 0)
            {
                BitArray b = new BitArray(new int[] { rights });
                int[] bits = b.Cast<bool>().Select(bit => bit ? 1 : 0).ToArray();

                StringBuilder sb = new StringBuilder();
                if (bits[0] == 1) { sb.Append(";Connect"); }
                if (bits[1] == 1) { sb.Append(";Freeze Versions"); }
                if (bits[2] == 1) { sb.Append(";Lock Documents"); }
                if (bits[3] == 1) { sb.Append(";Manage Branches"); }
                if (bits[4] == 1) { sb.Append(";Manage Configurations"); }
                if (bits[5] == 1) { sb.Append(";Manage All Documents"); }
                if (bits[6] == 1) { sb.Append(";Manage Users & Permissions"); }
                if (bits[7] == 1) { sb.Append(";Manage Repository"); }
                if (bits[8] == 1) { sb.Append(";Edit on Web"); }
                if (bits[9] == 1) { sb.Append(";Edit Extensions on Web"); }
                return sb.ToString().Substring(1);
            }
            else
            {
                return "None";
            }
        }

        /// <summary>
        /// Tries to parse the specified user status into a va lid UserStatus enum.
        /// </summary>
        /// <param name="status">The user status to parse.</param>
        /// <returns>A <see cref="UserStatusEnum"/> enum.</returns>
        private static UserStatusEnum ParseUserStatus(string status)
        {
            switch (status.ToLowerInvariant())
            {
                case "a":
                    return UserStatusEnum.Active;
                case "i":
                    return UserStatusEnum.Inactive;
                case "b":
                    return UserStatusEnum.Blocked;
                default:
                    throw new RepositoryException("Invalid user status");
            }
        }

        #endregion

        #region Events

        protected virtual void OnDocumentCheckedIn(CheckInEventArgs args)
        {
            RepoDocumentCheckedIn?.Invoke(this, args);
        }

        protected virtual void OnDocumentCheckedOut(CheckOutEventArgs args)
        {
            RepoDocumentCheckedOut?.Invoke(this, args);
        }

        #endregion
    }
}
