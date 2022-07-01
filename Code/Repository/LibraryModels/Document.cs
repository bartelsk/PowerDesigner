namespace PDRepository.LibraryModels
{
    /// <summary>
    /// Represents a repository document.
    /// </summary>
    public class Document
    {
        /// <summary>
        /// The name of the document.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The class name of the document.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Determines whether the document is frozen or not.
        /// </summary>
        public bool IsFrozen { get; set; }

        /// <summary>
        /// The location of the document in the repository.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The object type of the document.
        /// </summary>
        public string ObjectType { get; set; }
        /// <summary>
        /// The document version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The document version comment.
        /// </summary>
        public string VersionComment { get; set; }
    }
}
