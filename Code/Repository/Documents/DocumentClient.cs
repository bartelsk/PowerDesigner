using PDRepository.LibraryModels;
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
            return GetFolderDocuments(repoFolderPath);
        }

        /// <summary>
        /// Retrieves information on a document in the specified repository folder.
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the documents.</param>
        /// <param name="documentName">The name of the document.</param>
        /// <returns>A <see cref="Document"/> type.</returns>
        public Document GetDocumentInfo(string repoFolderPath, string documentName)
        {
            if (!IsConnected) ThrowNoRepositoryConnectionException();
            return GetFolderDocumentInfo(repoFolderPath, documentName);
        }

        /// <summary>
        /// Checks out the document in the specified repository folder and saves it in the target folder. 
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the documents.</param>
        /// <param name="filePath">The fully-qualified file path for the file on disc.</param>
        public void CheckOutDocument(string repoFolderPath, string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks out the documents in the specified repository folder and saves them in the target folder. 
        /// </summary>
        /// <param name="repoFolderPath">The repository folder from which to retrieve the documents.</param>
        /// <param name="targetFolder">The folder on disc to use as the check-out location for the documents.</param>
        /// <param name="recursive">True to also check out the documents in any sub-folder of the <paramref name="repoFolderPath"/>.</param>
        public void CheckOutDocuments(string repoFolderPath, string targetFolder, bool recursive)
        {
            throw new NotImplementedException();
        }
    }
}
