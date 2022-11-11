namespace HttpServer.Attributes;

public class HttpPOST : Attribute
{
    public string MethodUri { get; }

    public HttpPOST(string methodUri)
    {
        MethodUri = methodUri;
    }
}