// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System;

namespace PDRepository.Exceptions
{
    /// <summary>
    /// The exception that is thrown in case invalid permissions are parsed.
    /// </summary>
    public sealed class InvalidPermissionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPermissionException"/> class.
        /// </summary>
        public InvalidPermissionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPermissionException"/> class with a custom exception message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for this exception.</param>
        public InvalidPermissionException(string message)
            : base(message)
        {
        }
    }
}
