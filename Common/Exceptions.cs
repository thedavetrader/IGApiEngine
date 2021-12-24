using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IGApi.Common
{
    internal class DBContextNullReferenceException : NullReferenceException
    {
        internal DBContextNullReferenceException(string dbContextEntity) : base($"The entity \"{dbContextEntity}\" was not found. Make sure the DBContext is properly configured and initialized.")
        {
        }
    }

    internal class RestCallHttpRequestException : HttpRequestException
    {
        internal RestCallHttpRequestException(HttpStatusCode httpStatusCode, [CallerMemberName] string? caller = null)
            : base($"The restcall \"{caller}\" failed. Check internet connection or IG api service status. HttpStatusCode: \"{httpStatusCode}\".", null, httpStatusCode) { }

    }

    internal class RestCallNullReferenceException : NullReferenceException
    {
        internal RestCallNullReferenceException([CallerMemberName] string? caller = null)
            : base($"The restcall \"{caller}\" succeeded, however the response was empty. Check for IG api service status.") { }
    }

    internal class PrimaryKeyNullReferenceException : NullReferenceException
    {
        internal PrimaryKeyNullReferenceException(string primaryKey)
            : base($"The primary key \"{primaryKey}\" is empty.") { }
    }
    internal class EssentialPropertyNullReferenceException : NullReferenceException
    {
        internal EssentialPropertyNullReferenceException(string property)
            : base($"The essential property \"{property}\" is empty.") { }
    }
    internal class EssentialPropertyInvalidCastException : InvalidCastException
    {
        internal EssentialPropertyInvalidCastException(string property)
            : base($"The essential property \"{property}\" could not be casted.") { }
    }
    internal class InvalidRequestMissingParametersException : Exception
    {
        internal InvalidRequestMissingParametersException([CallerMemberName] string? request = null)
            : base($"The restrequest \"{request}\" is invalid. This request requires parameters.") { }
    }

    internal class IGApiConncectionError : Exception
    {
        public IGApiConncectionError([CallerMemberName] string? caller = null) : base($"Error while executing {caller ?? "UNKNOWN_CALLER"}. Not, or no longer logged in. Check internet or IG Api service status.") { }
    }
}