using System;

// TODO: select an appropriate name and move this into the SDK
//
// This generic exception should be used as a parent type for any other custom
// exceptions thrown by the high level API.
[Serializable]
public class OpaException : Exception
{
    public OpaException()
    { }

    public OpaException(string message)
        : base(message)
    { }

    public OpaException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
