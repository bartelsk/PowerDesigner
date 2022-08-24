// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using System;
using System.Collections.Generic;

namespace PDRepository.Documents
{
    /// <summary>
    /// This class contains methods to work with PowerDesigner repository documents.
    /// </summary>
    public class DocumentClient : Repository, IDocumentClient
    {
        /// <summary>
        /// Signals a document has been checked out.
        /// </summary>
        public event EventHandler<CheckOutEventArgs> DocumentCheckedOut;

        /// <summary>
        /// Creates a new instance of the <see cref="DocumentClient"/> class.
        /// </summary>
        /// <param name="settings">The current <see cref="RepositorySettings"/>.</param>
        public DocumentClient(RepositorySettings settings) : base(settings)
        {
            Connect();
        }

        /// <summary>
        /// Determines whether a document exists in the specified repository folder.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder in which the document should be located.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <returns>True if the document exists in the specified repository folder, False if it does not.</returns>
        public bool DocumentExists(string repoFolderPath, string documentName)
        {
            if (!IsConnected) ThrowNoRepositoryConnectionException();
            return (GetFolderDocumentInfo(repoFolderPath, documentName) != null);
        }

        /// <summary>
        /// Returns a list of <see cref="Document"/> objects in the specified repository folder.        
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the documents.</param>
        /// <param name="recursive">True to also list documents in any sub-folder of the <paramref name="repoFolderPath"/>.</param>
        /// <returns>A List with <see cref="Document"/> objects.</returns>      
        public List<Document> ListDocuments(string repoFolderPath, bool recursive)
        {
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
            if (!IsConnected) ThrowNoRepositoryConnectionException();
            return GetFolderDocumentInfo(repoFolderPath, documentName);
        }

        /// <summary>
        /// Checks out the document in the specified repository folder and saves it to disc. Overwrites the local document (if any).
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document to check out.</param>        
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the document.</param>
        public void CheckOutDocument(string repoFolderPath, string documentName, string targetFolder)
        {
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
            if (!IsConnected) ThrowNoRepositoryConnectionException();
            CheckOutFolderDocument(repoFolderPath, documentName, targetFolder, targetFileName);
        }

        /// <summary>
        /// Checks out a specific version of the document in the specified repository folder and saves it to disc. Overwrites the local document (if any).
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document to check out.</param>        
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the document.</param>
        /// <param name="version">The document version. The version must belong to the same branch as the current object.</param>
        public void CheckOutDocument(string repoFolderPath, string documentName, string targetFolder, int version)
        {
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
        /// <param name="version">The document version. The version must belong to the same branch as the current object.</param>
        public void CheckOutDocument(string repoFolderPath, string documentName, string targetFolder, string targetFileName, int version)
        {
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
            if (!IsConnected) ThrowNoRepositoryConnectionException();
            return UnfreezeFolderDocument(repoFolderPath, documentName);
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
            if (!IsConnected) ThrowNoRepositoryConnectionException();
            return DeleteDocumentPermission(repoFolderPath, documentName, permission);
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
