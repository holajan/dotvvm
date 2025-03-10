using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.Hosting;

namespace DotVVM.Framework.Security {

    public interface ICsrfProtector {

        /// <summary>
        /// Generates new CSRF protection token, which is supposed to be passed intact on postback.
        /// </summary>
        /// <param name="context">Context for current request.</param>
        /// <returns>Base64-encoded string, which is supposed to be passed intact on postback.</returns>
        string GenerateToken(DotvvmRequestContext context);

        /// <summary>
        /// Validates supplied CSRF token. Throws <see cref="System.Security.SecurityException"/> when token is missing or invalid.
        /// </summary>
        /// <param name="context">Context for current request.</param>
        /// <param name="token">Security token being generated by <see cref="GenerateToken"/> method.</param>
        /// <exception cref="System.Security.SecurityException">Thrown when token string is missing or invalid.</exception>
        void VerifyToken(DotvvmRequestContext context, string token);

    }
}
