using System;

namespace PDRepository.Common
{
    /// <summary>
    /// Represents repository document check out event arguments.
    /// </summary>
    public class CheckOutEventArgs : EventArgs
    {
        /// <summary>
        /// The name of the repository document.
        /// </summary>
        public string DocumentName { get; set; }

        /// <summary>
        /// The target file name of the document on disc.
        /// </summary>
        public string CheckOutFileName { get; set; }
    }
}
