namespace PersonalSite.Domain.Exceptions;

[Serializable]
internal class HttpResponseException : Exception
{
    public HttpResponseException()
    {
    }

    public HttpResponseException(string message)
        : base(message)
    {
    }

    public HttpResponseException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}