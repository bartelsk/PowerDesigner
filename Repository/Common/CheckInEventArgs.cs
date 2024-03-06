// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System;

namespace PDRepository.Common
{
    /// <summary>
    /// Represents repository document check in event arguments.
    /// </summary>
    public class CheckInEventArgs : EventArgs
    {
        /// <summary>
        /// The name of the repository document.
        /// </summary>
        public string DocumentName { get; set; }

        /// <summary>
        /// The location of the repository document.
        /// </summary>
        public string DocumentFolder { get; set; }

        /// <summary>
        /// The version of the repository document.
        /// </summary>
        public string DocumentVersion { get; set; }

        /// <summary>
        /// The source file name of the document on disc.
        /// </summary>
        public string CheckInFileName { get; set; }
    }
}
