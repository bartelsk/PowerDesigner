// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System;

namespace PDRepository.Exceptions
{
    /// <summary>
    /// The exception that is thrown in case invalid user or group rights are parsed.
    /// </summary>
    public sealed class InvalidRightsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRightsException"/> class.
        /// </summary>
        public InvalidRightsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRightsException"/> class with a custom exception message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for this exception.</param>
        public InvalidRightsException(string message)
            : base(message)
        {
        }
    }
}
