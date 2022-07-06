﻿using PDRepository.LibraryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDRepository.Documents
{
    /// <summary>
    /// This class contains methods to work with PowerDesigner repository documents.
    /// </summary>
    public class DocumentClient : Repository, IDocumentClient
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DocumentClient"/> class.
        /// </summary>
        /// <param name="settings">The current <see cref="RepositorySettings"/>.</param>
        public DocumentClient(RepositorySettings settings) : base(settings)
        {
            Connect();
        }      

        /// <summary>
        /// Returns a list of <see cref="Document"/> objects in the specified repository folder.
        /// Does not recurse sub-folders.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the documents.</param>
        /// <returns>A List with <see cref="Document"/> objects.</returns> 
        public List<Document> ListDocuments(string repoFolderPath)
        {
            if (!IsConnected) ThrowNoRepositoryConnectionException();
            return GetFolderDocumentsInfo(repoFolderPath);
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
        public void CheckOutDocuments(string repoFolderPath, string targetFolder, bool recursive)
        {
            CheckOutFolderDocuments(repoFolderPath, targetFolder, recursive);
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
        /// <returns>True if successful, False if not.</returns>
        public bool FreezeDocument(string repoFolderPath, string documentName, string comment)
        {
            if (!IsConnected) ThrowNoRepositoryConnectionException();
            return FreezeFolderDocument(repoFolderPath, documentName, comment);
        }

        /// <summary>
        /// Unfreezes a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to unfreeze.</param>        
        /// <returns>True if successful, False if not.</returns>
        public bool UnfreezeDocument(string repoFolderPath, string documentName)
        {
            if (!IsConnected) ThrowNoRepositoryConnectionException();
            return UnfreezeFolderDocument(repoFolderPath, documentName);
        }       
    }
}
