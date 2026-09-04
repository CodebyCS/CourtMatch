using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Exceptions
{
    /// <summary>
    /// Serves as the base class for application-specific exceptions, including an HTTP status code.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public abstract class AppException : Exception
    {
        /// <summary>
        /// Gets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public int StatusCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="statuscode">The statuscode.</param>
        protected AppException(string message, int statuscode) : base(message)
        {
            StatusCode = statuscode;
        }
    }
}
