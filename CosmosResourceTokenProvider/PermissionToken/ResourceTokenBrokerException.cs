using System;

namespace CosmosResourceTokenProvider.PermissionToken
{
    public class ResourceTokenBrokerException : Exception
    {
        public ResourceTokenBrokerException() { }

        public ResourceTokenBrokerException(string message) : base(message) { }

        public ResourceTokenBrokerException(string message, Exception ex) : base(message, ex) { }
    }
}
