namespace HttpServer.Attributes;

public class HttpGET : Attribute
{
    private string MethodUri { get; }

    public HttpGET(string methodUri)
    {
        MethodUri = methodUri;
    }
}