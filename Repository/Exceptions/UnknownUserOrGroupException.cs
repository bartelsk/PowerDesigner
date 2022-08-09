// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System;

namespace PDRepository.Exceptions
{
    /// <summary>
    /// The exception that is thrown in case of an unknown user or group. 
    /// </summary>
    public sealed class UnknownUserOrGroupException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownUserOrGroupException"/> class.
        /// </summary>
        public UnknownUserOrGroupException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownUserOrGroupException"/> class with a custom exception message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for this exception.</param>
        public UnknownUserOrGroupException(string message)
            : base(message)
        {
        }
    }
}
