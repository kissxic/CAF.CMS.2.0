﻿using System;
using CAF.Infrastructure.SearchModule.Model;

namespace CAF.Infrastructure.SearchModule.Providers.Azure
{

    public class AzureSearchException : SearchException
    {
        public AzureSearchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AzureSearchException(string message)
            : base(message)
        {
        }
    }
}
