using PDRepository.LibraryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <returns>True if successful, False if not.</returns>
        bool FreezeDocument(string repoFolderPath, string documentName, string comment);

        /// <summary>
        /// Unfreezes a repository document.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder that contains the document.</param>
        /// <param name="documentName">The name of the document to unfreeze.</param>        
        /// <returns>True if successful, False if not.</returns>
        bool UnfreezeDocument(string repoFolderPath, string documentName);
    }
}
