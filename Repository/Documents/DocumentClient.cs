﻿// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using System;
using System.IO;
using System.Collections.Generic;

namespace PDRepository.Documents
{
    /// <summary>
    /// This class contains methods to work with PowerDesigner repository documents.
    /// </summary>
    public class DocumentClient : Repository, IDocumentClient
    {
        /// <summary>
        /// Signals a document has been checked in.
        /// </summary>
        public event EventHandler<CheckInEventArgs> DocumentCheckedIn;

        /// <summary>
        /// Signals a document has been checked out.
        /// </summary>
        public event EventHandler<CheckOutEventArgs> DocumentCheckedOut;

        /// <summary>
        /// Creates a new instance of the <see cref="DocumentClient"/> class.
        /// </summary>
        /// <param name="settings">The current repository <see cref="ConnectionSettings"/>.</param>
        public DocumentClient(ConnectionSettings settings) : base(settings)
        {
            Connect();
        }

        /// <summary>
        /// Determines the existence of a repository folder.
        /// </summary>
        /// <param name="repoFolderPath">A repository folder path.</param>        
        /// <returns>True if the folder exists, False if not.</returns>
        public bool FolderExists(string repoFolderPath)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);            
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return DocumentFolderExists(repoFolderPath);
        }

        /// <summary>
        /// Creates a repository folder.
        /// </summary>
        /// <param name="repoFolderPath">The parent repository folder path.</param>
        /// <param name="folderName">The name of the new folder.</param>
        /// <returns>True if successful, False if not.</returns>     
        public bool CreateFolder(string repoFolderPath, string folderName)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(folderName)) ThrowArgumentNullException(folderName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return CreateRepositoryFolder(repoFolderPath, folderName);
        }

        /// <summary>
        /// Deletes a repository folder. 
        /// This method cannot be used to delete a repository branch folder.
        /// </summary>
        /// <remarks>
        /// The deletion may fail if the folder is not empty.
        /// </remarks>
        /// <param name="repoFolderPath">The repository folder that should be deleted.</param>        
        /// <returns>True if successful, False if not.</returns>
        public bool DeleteFolder(string repoFolderPath)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return DeleteRepositoryFolder(repoFolderPath);
        }

        /// <summary>
        /// Determines whether a document exists in the specified repository folder.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder in which the document should be located.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <returns>True if the document exists in the specified repository folder, False if it does not.</returns>
        public bool DocumentExists(string repoFolderPath, string documentName)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return FolderDocumentExists(repoFolderPath, documentName);
        }

        /// <summary>
        /// Returns a list of <see cref="Document"/> objects in the specified repository folder.        
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the documents.</param>
        /// <param name="recursive">True to also list documents in any sub-folder of the <paramref name="repoFolderPath"/>.</param>
        /// <returns>A List with <see cref="Document"/> objects.</returns>      
        public List<Document> ListDocuments(string repoFolderPath, bool recursive)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return GetFolderDocumentsInfo(repoFolderPath, recursive);
        }

        /// <summary>
        /// Retrieves information on a document in the specified repository folder.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <returns>A <see cref="Document"/> type.</returns>
        public Document GetDocumentInfo(string repoFolderPath, string documentName)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return GetFolderDocumentInfo(repoFolderPath, documentName);
        }

        /// <summary>
        /// Adds a file to the specified repository folder. Overwrites the existing document (if any) and freezes it.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder in which to add the files. Will get overridden if the model was part of a repository branch already.</param>
        /// <param name="fileName">The fully-qualified name of the file.</param>
        /// <param name="documentVersion">Contains the current document version number if the check-in was successful.</param>
        public void CheckInDocument(string repoFolderPath, string fileName, out string documentVersion)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(fileName)) ThrowArgumentNullException(fileName);
            if (!File.Exists(fileName)) ThrowFileNotFoundException(fileName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            CheckInFolderDocument(repoFolderPath, fileName, out documentVersion);
        }

        /// <summary>
        /// Adds files to the specified repository folder. Overwrites the existing documents (if any) and freezes them.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder in which to add the files. Will get overridden if the model was part of a repository branch already.</param>
        /// <param name="sourceFolder">The folder on disc that contains the files you want to add.</param>        
        public void CheckInDocuments(string repoFolderPath, string sourceFolder)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (!Directory.Exists(sourceFolder)) ThrowDirectoryNotFoundException(sourceFolder);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            CheckInFolderDocuments(repoFolderPath, sourceFolder);
        }

        /// <summary>
        /// Checks out the document in the specified repository folder and saves it to disc. Overwrites the local document (if any).
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document to check out.</param>        
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the document.</param>
        public void CheckOutDocument(string repoFolderPath, string documentName, string targetFolder)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (string.IsNullOrEmpty(targetFolder)) ThrowArgumentNullException(targetFolder);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            CheckOutFolderDocument(repoFolderPath, documentName, targetFolder);
        }

        /// <summary>
        /// Checks out the document in the specified repository folder and saves it to disc. Overwrites the local document (if any).
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document to check out.</param>
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the document.</param>
        /// <param name="targetFileName">The file name for the document.</param>
        public void CheckOutDocument(string repoFolderPath, string documentName, string targetFolder, string targetFileName)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (string.IsNullOrEmpty(targetFolder)) ThrowArgumentNullException(targetFolder);
            if (string.IsNullOrEmpty(targetFileName)) ThrowArgumentNullException(targetFileName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            CheckOutFolderDocument(repoFolderPath, documentName, targetFolder, targetFileName);
        }

        /// <summary>
        /// Checks out a specific version of the document in the specified repository folder and saves it to disc. Overwrites the local document (if any).
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document to check out.</param>        
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the document.</param>
        /// <param name="version">The document version. The latest version of the document will be checked out if the specified document version does not exist. The version must also belong to the same branch as the current object.</param>
        public void CheckOutDocument(string repoFolderPath, string documentName, string targetFolder, int version)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (string.IsNullOrEmpty(targetFolder)) ThrowArgumentNullException(targetFolder);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            CheckOutFolderDocument(repoFolderPath, documentName, targetFolder, version);
        }

        /// <summary>
        /// Checks out a specific version of the document in the specified repository folder and saves it to disc. Overwrites the local document (if any).
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document to check out.</param>
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the document.</param>
        /// <param name="targetFileName">The file name for the document.</param>
        /// <param name="version">The document version. The latest version of the document will be checked out if the specified document version does not exist. The version must also belong to the same branch as the current object.</param>
        public void CheckOutDocument(string repoFolderPath, string documentName, string targetFolder, string targetFileName, int version)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (string.IsNullOrEmpty(targetFolder)) ThrowArgumentNullException(targetFolder);
            if (string.IsNullOrEmpty(targetFileName)) ThrowArgumentNullException(targetFileName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();
            
            CheckOutFolderDocument(repoFolderPath, documentName, targetFolder, targetFileName, version);
        }

        /// <summary>
        /// Checks out the documents in the specified repository folder and saves them in the target folder. 
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the documents.</param>
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the documents.</param>
        /// <param name="recursive">True to also check out the documents in any sub-folder of the <paramref name="repoFolderPath"/>.</param>
        /// <param name="preserveFolderStructure">True to mimic the repository folder structure on the local disc when checking out. Applies to recursive check-outs only.</param>
        public void CheckOutDocuments(string repoFolderPath, string targetFolder, bool recursive, bool preserveFolderStructure)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);            
            if (string.IsNullOrEmpty(targetFolder)) ThrowArgumentNullException(targetFolder);            
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            CheckOutFolderDocuments(repoFolderPath, targetFolder, recursive, preserveFolderStructure);
        }

        /// <summary>
        /// Locks a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to lock.</param>
        /// <param name="comment">Lock comment.</param>
        /// <returns>True if successful, False if not.</returns>
        public bool LockDocument(string repoFolderPath, string documentName, string comment)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return LockFolderDocument(repoFolderPath, documentName, comment);
        }

        /// <summary>
        /// Unlocks a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to unlock.</param>        
        /// <returns>True if successful, False if not.</returns>
        public bool UnlockDocument(string repoFolderPath, string documentName)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return UnlockFolderDocument(repoFolderPath, documentName);
        }

        /// <summary>
        /// Freezes a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to freeze.</param>
        /// <param name="comment">Freeze comment.</param>
        /// <returns>True if successful, False if not (the document may already be frozen).</returns>
        public bool FreezeDocument(string repoFolderPath, string documentName, string comment)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return FreezeFolderDocument(repoFolderPath, documentName, comment);
        }

        /// <summary>
        /// Unfreezes a repository document, making it updateable.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to unfreeze.</param>        
        /// <returns>True if successful, False if not (the document may already be updateable).</returns>
        public bool UnfreezeDocument(string repoFolderPath, string documentName)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return UnfreezeFolderDocument(repoFolderPath, documentName);
        }

        /// <summary>
        /// Completely removes a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to remove completely.</param>        
        /// <returns>True if successful, False if not.</returns>
        public bool DeleteDocument(string repoFolderPath, string documentName)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return DeleteFolderDocument(repoFolderPath, documentName);
        }

        /// <summary>
        /// Removes the current version of a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document.</param>   
        /// <returns>True if successful, False if not.</returns>
        public bool DeleteDocumentVersion(string repoFolderPath, string documentName)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return DeleteFolderDocumentVersion(repoFolderPath, documentName);
        }

        /// <summary>
        /// Retrieves the permission on a repository document for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <param name="userOrGroupName">The user login or group name for which to check its permission.</param>
        /// <returns>A <see cref="PermissionTypeEnum"/> type.</returns>
        public PermissionTypeEnum GetPermission(string repoFolderPath, string documentName, string userOrGroupName)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (string.IsNullOrEmpty(userOrGroupName)) ThrowArgumentNullException(userOrGroupName);
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return GetDocumentPermission(repoFolderPath, documentName, userOrGroupName);
        }

        /// <summary>
        /// Grants permissions to a repository document for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <param name="permission">The <see cref="Permission"/> that is to be granted to the folder document.</param>
        /// <returns>True if successful, False if not.</returns>
        public bool SetPermission(string repoFolderPath, string documentName, Permission permission)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (permission == null) ThrowArgumentNullException("permission");
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return SetDocumentPermission(repoFolderPath, documentName, permission);
        }

        /// <summary>
        /// Deletes all permissions from a repository document for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <param name="permission">A <see cref="Permission"/> type that specifies the user login or group name and whether to remove the permissions from all child objects as well (if any).</param>
        /// <returns>True if successful, False if not.</returns>
        public bool DeletePermission(string repoFolderPath, string documentName, Permission permission)
        {
            if (string.IsNullOrEmpty(repoFolderPath)) ThrowArgumentNullException(repoFolderPath);
            if (string.IsNullOrEmpty(documentName)) ThrowArgumentNullException(documentName);
            if (permission == null) ThrowArgumentNullException("permission");
            if (!IsConnected) ThrowNoRepositoryConnectionException();

            return DeleteDocumentPermission(repoFolderPath, documentName, permission);
        }

        protected override void OnDocumentCheckedIn(CheckInEventArgs args)
        {
            DocumentCheckedIn?.Invoke(this, args);
        }

        /// <summary>
        /// Signals a document is checked out.
        /// </summary>
        /// <param name="args">The file name of the document.</param>
        protected override void OnDocumentCheckedOut(CheckOutEventArgs args)
        {
            DocumentCheckedOut?.Invoke(this, args);
        }
    }
}
