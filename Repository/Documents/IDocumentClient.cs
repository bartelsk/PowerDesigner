// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using PDRepository.Common;
using System;
using System.Collections.Generic;

namespace PDRepository.Documents
{
    /// <summary>
    /// Defines the interface for the <see cref="DocumentClient"/> class.
    /// </summary>
    public interface IDocumentClient : IDisposable
    {
        /// <summary>
        /// Signals a document has been checked out.
        /// </summary>
        event EventHandler<CheckOutEventArgs> DocumentCheckedOut;

        /// <summary>
        /// Determines the existence of a repository folder.
        /// </summary>
        /// <param name="repoFolderPath">A repository folder path.</param>        
        /// <returns>True if the folder exists, False if not.</returns>
        bool FolderExists(string repoFolderPath);

        /// <summary>
        /// Creates a repository folder.
        /// </summary>
        /// <param name="repoFolderPath">The parent repository folder path.</param>
        /// <param name="folderName">The name of the new folder.</param>
        /// <returns>True if successful, False if not.</returns>        
        bool CreateFolder(string repoFolderPath, string folderName);

        /// <summary>
        /// Deletes a repository folder. 
        /// This method cannot be used to delete a repository branch folder.
        /// </summary>
        /// <remarks>
        /// The deletion may fail if the folder is not empty.
        /// </remarks>
        /// <param name="repoFolderPath">The repository folder that should be deleted.</param>        
        /// <returns>True if successful, False if not.</returns>
        bool DeleteFolder(string repoFolderPath);

        /// <summary>
        /// Determines whether a document exists in the specified repository folder.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder in which the document should be located.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <returns>True if the document exists in the specified repository folder, False if it does not.</returns>
        bool DocumentExists(string repoFolderPath, string documentName);

        /// <summary>
        /// Returns a list of <see cref="Document"/> objects in the specified repository folder.        
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the documents.</param>
        /// <param name="recursive">True to also list documents in any sub-folder of the <paramref name="repoFolderPath"/>.</param>
        /// <returns>A List with <see cref="Document"/> objects.</returns>      
        List<Document> ListDocuments(string repoFolderPath, bool recursive);

        /// <summary>
        /// Retrieves information on a document in the specified repository folder.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <returns>A <see cref="Document"/> type.</returns>
        Document GetDocumentInfo(string repoFolderPath, string documentName);

        /// <summary>
        /// Adds a file to the specified repository folder. Overwrites the existing document (if any) and freezes it.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder in which to add the file.</param>
        /// <param name="fileName">The fully-qualified name of the file.</param>
        /// <param name="documentVersion">Contains the current document version number if the check-in was successful.</param>
        void CheckInDocument(string repoFolderPath, string fileName, out string documentVersion);

        /// <summary>
        /// Checks out the document in the specified repository folder and saves it to disc. Overwrites the local document (if any).
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document to check out.</param>
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the document.</param>
        void CheckOutDocument(string repoFolderPath, string documentName, string targetFolder);

        /// <summary>
        /// Checks out the document in the specified repository folder and saves it to disc. Overwrites the local document (if any).
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document to check out.</param>
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the document.</param>
        /// <param name="targetFileName">The file name for the document.</param>
        void CheckOutDocument(string repoFolderPath, string documentName, string targetFolder, string targetFileName);

        /// <summary>
        /// Checks out a specific version of the document in the specified repository folder and saves it to disc. Overwrites the local document (if any).
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document to check out.</param>
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the document.</param>
        /// <param name="version">The document version. The version must belong to the same branch as the current object.</param>
        void CheckOutDocument(string repoFolderPath, string documentName, string targetFolder, int version);

        /// <summary>
        /// Checks out a specific version of the document in the specified repository folder and saves it to disc. Overwrites the local document (if any).
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the document.</param>
        /// <param name="documentName">The name of the document to check out.</param>
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the document.</param>
        /// <param name="targetFileName">The file name for the document.</param>
        /// <param name="version">The document version. The version must belong to the same branch as the current object.</param>
        void CheckOutDocument(string repoFolderPath, string documentName, string targetFolder, string targetFileName, int version);

        /// <summary>
        /// Checks out the documents in the specified repository folder and saves them in the target folder. 
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the documents.</param>
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the documents.</param>
        /// <param name="recursive">True to also check out the documents in any sub-folder of the <paramref name="repoFolderPath"/>.</param>
        /// <param name="preserveFolderStructure">True to mimic the repository folder structure on the local disc when checking out. Applies to recursive check-outs only.</param>
        void CheckOutDocuments(string repoFolderPath, string targetFolder, bool recursive, bool preserveFolderStructure);

        /// <summary>
        /// Locks a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to lock.</param>
        /// <param name="comment">Lock comment.</param>
        /// <returns>True if successful, False if not.</returns>
        bool LockDocument(string repoFolderPath, string documentName, string comment);

        /// <summary>
        /// Unlocks a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to unlock.</param>        
        /// <returns>True if successful, False if not.</returns>
        bool UnlockDocument(string repoFolderPath, string documentName);

        /// <summary>
        /// Freezes a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to freeze.</param>
        /// <param name="comment">Freeze comment.</param>
        /// <returns>True if successful, False if not (the document may already be frozen).</returns>
        bool FreezeDocument(string repoFolderPath, string documentName, string comment);

        /// <summary>
        /// Unfreezes a repository document, making it updateable.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to unfreeze.</param>        
        /// <returns>True if successful, False if not (the document may already be updateable).</returns>
        bool UnfreezeDocument(string repoFolderPath, string documentName);

        /// <summary>
        /// Completely removes a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to remove completely.</param>        
        /// <returns>True if successful, False if not.</returns>
        bool DeleteDocument(string repoFolderPath, string documentName);

        /// <summary>
        /// Removes the current version of a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document.</param>   
        /// <returns>True if successful, False if not.</returns>
        bool DeleteDocumentVersion(string repoFolderPath, string documentName);

        /// <summary>
        /// Retrieves the permission on a repository document for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <param name="userOrGroupName">The user login or group name for which to check its permission.</param>
        /// <returns>A <see cref="PermissionTypeEnum"/> type.</returns>
        PermissionTypeEnum GetPermission(string repoFolderPath, string documentName, string userOrGroupName);

        /// <summary>
        /// Grants permissions to a repository document for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <param name="permission">The <see cref="Permission"/> that is to be granted to the folder document.</param>
        /// <returns>True if successful, False if not.</returns>
        bool SetPermission(string repoFolderPath, string documentName, Permission permission);

        /// <summary>
        /// Deletes all permissions from a repository document for a specific user login or group name.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <param name="permission">A <see cref="Permission"/> type that specifies the user login or group name and whether to remove the permissions from all child objects as well (if any).</param>
        /// <returns>True if successful, False if not.</returns>
        bool DeletePermission(string repoFolderPath, string documentName, Permission permission);
    }
}
