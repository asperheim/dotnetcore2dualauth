using System;
using System.Runtime.Serialization;

public class IPNotInWhitelistException : Exception
{
    public IPNotInWhitelistException()
    {
    }

    public IPNotInWhitelistException(string message) : base(message)
    {
    }

    public IPNotInWhitelistException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected IPNotInWhitelistException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
