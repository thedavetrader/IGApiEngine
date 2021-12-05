﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IGApi.Common
{
    internal class DBContextNullReferenceException : NullReferenceException
    {
        internal DBContextNullReferenceException(string dbContextEntity) : base($"The entity {dbContextEntity} was not found. Make sure the DBContext is properly configured and initialized.")
        {
        }
    }

    internal class RestCallHttpRequestException : HttpRequestException
    {
        internal RestCallHttpRequestException(string restCall, HttpStatusCode httpStatusCode) 
            : base($"The restcall {restCall} failed. Check internet connection or IG api service status.", null, httpStatusCode) { }
    }

    internal class RestCallNullReferenceException : NullReferenceException
    {
        internal RestCallNullReferenceException(string restCall)
            : base($"The restcall {restCall} succeeded, however the response was empty. Check for IG api service status.") { }
    }

    internal class PrimaryKeyNullReferenceException : NullReferenceException
    {
        internal PrimaryKeyNullReferenceException(string primaryKey)
            : base($"The primary key {primaryKey} is empty.") { }
    }
    internal class EssentialPropertyNullReferenceException : NullReferenceException
    {
        internal EssentialPropertyNullReferenceException(string property)
            : base($"The essential propertie {property} is empty.") { }
    }
}