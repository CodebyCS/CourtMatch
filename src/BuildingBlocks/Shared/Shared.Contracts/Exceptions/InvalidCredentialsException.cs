using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Exceptions
{
    public class InvalidCredentialsException : UnauthorizedAppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCredentialsException"/> class.
        /// </summary>
        public InvalidCredentialsException()
            : base("Email ou password inválidos.") { }
    }
}
