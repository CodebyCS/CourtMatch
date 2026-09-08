using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Exceptions
{

    /// <summary>
    /// Thrown when a requested resource cannot be found.
    /// </summary>
    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message, 404) { }
    }
}
