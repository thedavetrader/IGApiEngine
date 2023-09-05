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

    internal class DBContextConnectionStringNullException : NullReferenceException
    {
        internal DBContextConnectionStringNullException() : base($"The dbcontext does not have a connection string.") { }
    }

    internal class RestCallHttpResponseNullException : NullReferenceException
    {
        internal RestCallHttpResponseNullException([CallerMemberName] string? caller = null)
            : base($"The restcall \"{caller}\" failed. Check internet connection or IG api service status.") { }
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

    internal class AsyncTaskException : AggregateException
    {
        internal AsyncTaskException(string? message = null, [CallerMemberName] string? task = null)
            : base($"Task \"{task}\"stopped unexpectedly withouth exception. {message ?? ""}") { }
    }

    internal static class TaskException
    {
        /// <summary>
        /// Handles exceptions raised by a running task. Cancellation exceptions will be logged, but will not rethrow. All other exceptions will be rethrowed.
        /// </summary>
        /// <param name="task"></param>
        public static void CatchTaskIsCanceledException(Task task)
        {
            const string unknownException = "Critical error. The task refers to an empty exception. This should never happen after an exception was raised within a task.";

            if (task.IsFaulted)
            {
                if (!task.IsCanceled)
                {
                    throw task.Exception ?? new AggregateException(unknownException);
                }
                else
                    Log.WriteException(new AsyncTaskException((task.Exception ?? new AggregateException(unknownException)).Message));
            }
        }
    }
}