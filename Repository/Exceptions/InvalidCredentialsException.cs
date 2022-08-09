using System;

namespace PDRepository.Exceptions
{
    /// <summary>
    /// The exception that is thrown in case invalid credentials are used to access the repository.
    /// </summary>
    public sealed class InvalidCredentialsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCredentialsException"/> class.
        /// </summary>
        public InvalidCredentialsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCredentialsException"/> class with a custom exception message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for this exception.</param>
        public InvalidCredentialsException(string message)
            : base(message)
        {
        }
    }
}
