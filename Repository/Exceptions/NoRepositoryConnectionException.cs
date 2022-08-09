using System;

namespace PDRepository.Exceptions
{
    /// <summary>
    /// The exception that is thrown in case there is no connection to the repository.
    /// </summary>
    public sealed class NoRepositoryConnectionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoRepositoryConnectionException"/> class.
        /// </summary>
        public NoRepositoryConnectionException()
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoRepositoryConnectionException"/> class with a custom exception message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for this exception.</param>
        public NoRepositoryConnectionException(string message)
            : base(message)
        {
        }
    }
}
