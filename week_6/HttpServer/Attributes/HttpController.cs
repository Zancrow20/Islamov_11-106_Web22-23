namespace HttpServer.Attributes;

public class HttpController : Attribute
{
    public string ControllerName { get; }

    public HttpController(string controllerName)
    {
        ControllerName = controllerName;
    }
}