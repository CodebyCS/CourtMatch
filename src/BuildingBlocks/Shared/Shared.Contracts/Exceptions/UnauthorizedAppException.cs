using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Exceptions
{
    /// <summary>
    /// Exception thrown when an unauthorized access (HTTP 401) error occurs within the application.
    /// </summary>
    /// <seealso cref="Shared.Contracts.Exceptions.AppException" />
    public class UnauthorizedAppException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedAppException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnauthorizedAppException(string message) : base(message, 401) { }
    }
}
