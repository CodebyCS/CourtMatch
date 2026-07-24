using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Exceptions
{
    /// <summary>
    /// Exception thrown when a bad request (HTTP 400) occurs within the application.
    /// </summary>
    /// <seealso cref="Shared.Contracts.Exceptions.AppException" />
    public class BadRequestException : AppException
    {
        public BadRequestException(string message) : base(message, 400) { }
    }
}
