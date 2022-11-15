namespace HttpServer.Attributes;

public class HttpGET : Attribute
{
    public string MethodUri { get; }

    public HttpGET(){ }
    
    public HttpGET(string methodUri)
    {
        MethodUri = methodUri;
    }
}