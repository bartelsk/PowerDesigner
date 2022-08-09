// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

namespace PDRepository.Common
{
    /// <summary>
    /// Represents a repository branch.
    /// </summary>
    public class Branch
    {
        /// <summary>
        /// The name of the branch.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The location of the branch relative to the specified root path.
        /// </summary>
        public string RelativePath { get; set; }
    }
}
