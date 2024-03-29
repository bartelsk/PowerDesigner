﻿// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

namespace PDRepository.Common
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
        /// The file name of the document when checked-out.
        /// </summary>
        public string ExtractionFileName { get; set; }

        /// <summary>
        /// Determines whether the document is frozen or not.
        /// </summary>
        public bool IsFrozen { get; set; }

        /// <summary>
        /// Determines whether the document is locked or not.
        /// </summary>
        public bool IsLocked { get; set; }

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
