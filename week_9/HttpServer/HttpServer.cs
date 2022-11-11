using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using HttpServer.Attributes;
using System;

namespace HttpServer;

public enum ServerStatus
{
   Abort,
   Close,
   Start,
   Stop,
}

public class Response
{
   public byte[] Buffer { get; set; }
   public HttpStatusCode StatusCode { get; set; }
   public string? Content { get; set; }
}


public class HttpServer : IDisposable
{
   private readonly HttpListener _listener;
   private ServerStatus _serverStatus = ServerStatus.Close;

   private static readonly Dictionary<string, string> _extensions = new()
   {
      {"html", "text/html"},
      {"css", "text/css"},
      {"php", "text/php"},
      {"png", "image/png"},
      {"gif", "image/gif"},
      {"jpeg", "image/jpeg"},
      {"svg", "image/svg+xml"},
      {"jpg", "image/jpg"}
   };

   private ServerSettings? _settings;
   
   private Response ResponseInfo = new() {Buffer = new byte[] { }, StatusCode = default, Content = default};

   public HttpServer()
   {
      _listener = new HttpListener();
   }

   public async Task Start()
   {
      if (_serverStatus == ServerStatus.Start)
      {
         Console.WriteLine("Сервер уже работает");
         return;
      }

      if (File.Exists(@"./settings.json"))
      {
         _settings = JsonSerializer.Deserialize<ServerSettings>(await File.ReadAllTextAsync("./settings.json"));
         _listener.Prefixes.Clear();  
      }
      else
         Console.WriteLine("settings.json not found");
      
      _listener.Prefixes.Add($"http://localhost:{_settings?.Port}/");
      Console.WriteLine("Запуск сервера...");
      _listener.Start();

      _serverStatus = ServerStatus.Start;
      Console.WriteLine("Сервер запущен");

      await ListenAsync();
   }

   public void Stop()
   {
      if (_serverStatus == ServerStatus.Stop) return;

      Console.WriteLine("Остановка сервера...");
      _listener.Stop();

      _serverStatus = ServerStatus.Stop;
      Console.WriteLine("Сервер остановлен");
   }

   private async Task ListenAsync()
   {
      while (_serverStatus == ServerStatus.Start)
      {
         HttpListenerContext context = await _listener.GetContextAsync();
         HttpListenerRequest request = context.Request;

         HttpListenerResponse response = context.Response;

         var methodHandled = MethodHandler(context);
         if (await methodHandled)
         {
            response.Headers.Set(HttpResponseHeader.ContentType, ResponseInfo.Content);
            response.StatusCode = (int) ResponseInfo.StatusCode;

            if (response.StatusCode == (int) HttpStatusCode.Redirect)
               response.Headers.Set(HttpResponseHeader.Location,
                  "https://store.steampowered.com/login/?redir=&redir_ssl=1&snr=1_4_4__global-header");

            using var output = response.OutputStream;
            output.Write(ResponseInfo.Buffer);
         }
         else
            await StaticResponseProviderMethod(request, response);
      }
   }

   private async Task StaticResponseProviderMethod(HttpListenerRequest request,HttpListenerResponse response)
   {
      byte[]? buffer;
      if (Directory.Exists(_settings?.Path))
      {
         buffer = GetFile(request.RawUrl?.Replace("%20", " "), response);
         if (buffer == null)
         {
            response.Headers.Set(HttpResponseHeader.ContentType, "text/plain");
            response.StatusCode = (int) HttpStatusCode.NotFound;
            var error = "404 - not found";
            buffer = Encoding.UTF8.GetBytes(error);
         }
      }
      else
      {
         var error = $"Directory '{_settings?.Path}' not found";
         buffer = Encoding.UTF8.GetBytes(error);
      }

      response.ContentLength64 = buffer.Length;
      Stream output = response.OutputStream;
      await output.WriteAsync(buffer);

      output.Close();
   }

   private static void AddHeaders(HttpListenerResponse response, string extension) =>
      response.Headers.Add(HttpResponseHeader.ContentType, _extensions[extension]);

   private byte[] GetFile(string? rawUrl, HttpListenerResponse response)
   {
      byte[]? buffer = null;
      var path = _settings?.Path + rawUrl;
      var extension = Path.GetExtension(rawUrl)?.Trim('.').TrimStart('+');
      if (Directory.Exists(path))
      {
         // Каталог
         path += "/index.html";
         if (File.Exists(path))
         {
            extension = "html";
            buffer = File.ReadAllBytes(path);
            AddHeaders(response, extension);
         }
      }
      else if (File.Exists(path))
      {
         // Файл
         buffer = File.ReadAllBytes(path);
         if (extension != null) 
            AddHeaders(response, extension);
      }

      return buffer;
   }

   private async Task<bool> MethodHandler(HttpListenerContext httpContext)
   {
      // объект запроса
      HttpListenerRequest request = httpContext.Request;

      // объект ответа
      HttpListenerResponse response = httpContext.Response;

      if (request.Url?.Segments.Length < 2) return false;

      var uri = string.Join("", request.Url?.Segments!);
      var controllerName = uri.Split('/')[1];
      var httpMethod = $"Http{httpContext.Request.HttpMethod}";
      
      Stream body = request.InputStream;
      Encoding encoding = request.ContentEncoding;
      using StreamReader reader = new StreamReader(body, encoding);
      var str = await reader.ReadToEndAsync();

      List<string> strParams = new List<string>();
      if(httpMethod == "HttpGET")
         strParams = httpContext.Request.Url?
            .Segments
            .Skip(2)
            .Select(s => s.Replace("/", ""))
            .ToList() ?? new List<string>();
      else if(httpMethod == "HttpPOST")
         strParams?.Add(str);

      var assembly = Assembly.GetExecutingAssembly();

      var controller = assembly
         .GetTypes()
         .Where(t => Attribute.IsDefined(t, typeof(HttpController)))
         .FirstOrDefault(c => c.Name.ToLower() == controllerName?.ToLower());

      if (controller == null) return false;
      
      var method = controller
         .GetMethods()
         .FirstOrDefault(method =>
         {
            var attribute = method.CustomAttributes
               .FirstOrDefault(attr => attr.AttributeType.Name == $"{httpMethod}");
            if (attribute == null) return false;
            var methodUri = attribute.AttributeType
               .GetProperty("MethodUri")
               ?.GetValue(method.GetCustomAttribute(attribute.AttributeType))?
               .ToString() ?? "";
            var httpMethodUri = request.Url?.AbsolutePath.Split('/')[2];
            if (httpMethodUri == null) return false;
            return httpMethodUri == methodUri;
         });
      
      if (method == null) return false;

      object?[] queryParams = method.GetParameters()
         .Select((p, i) => Convert.ChangeType(strParams?[i], p.ParameterType))
         .ToArray();

      var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);

      var buffer = ret != null ? Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret))
            : Encoding.ASCII.GetBytes("404 - not found");
      response.ContentLength64 = buffer.Length;

      ResponseInfo = ret == null ? 
         new Response {Buffer = buffer, Content = "Application/json", StatusCode = HttpStatusCode.Redirect} : 
         new Response {Buffer = buffer, Content = "Application/json", StatusCode = HttpStatusCode.OK};
      return true;
   }

   public void Dispose() => Stop();
}