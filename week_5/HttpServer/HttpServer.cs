using System.Net;
using System.Text;
using System.Text.Json;

namespace HttpServer;

public enum ServerStatus
{
   Abort,
   Close,
   Start,
   Stop,
}

public class HttpServer : IDisposable
{
   //private readonly string _url;
   //private HttpListenerContext _httpListenerContext;
   //private bool isWorking;
   private readonly HttpListener _listener;
   private ServerStatus _serverStatus = ServerStatus.Close;

   private readonly Dictionary<string, string> _extensions = new()
   {
      {"html", "text"},
      {"css", "text"},
      {"php", "text"},
      {"png", "image"},
      {"gif", "image"},
      {"jpeg", "image"},
      {$"svg", "image"}, //+xml Как добавить, svg не воспринимает, не понимаю почему
      {"jpg", "image"}
   };

   private ServerSettings? _settings;

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
         _settings = JsonSerializer.Deserialize<ServerSettings>(File.ReadAllText("./settings.json"));
         _listener.Prefixes.Clear();  
      }
      else
         Console.WriteLine("settings.json not found");
      
      _listener.Prefixes.Add($"http://localhost:{_settings?.Port}/");
      Console.WriteLine("Запуск сервера...");
      _listener.Start();

      _serverStatus = ServerStatus.Start;
      Console.WriteLine("Сервер запущен");

      await Listen();
   }

   public async Task Stop()
   {
      if (_serverStatus == ServerStatus.Stop) return;

      Console.WriteLine("Остановка сервера...");
      _listener.Stop();

      _serverStatus = ServerStatus.Stop;
      Console.WriteLine("Сервер остановлен");
   }

   private async Task Listen() => await ListenCallBack();

   private async Task ListenCallBack()
   {
      if (_listener.IsListening)
      {
         HttpListenerContext context = await _listener.GetContextAsync();
         HttpListenerRequest request = context.Request;

         HttpListenerResponse response = context.Response;

         byte[]? buffer;

         if (Directory.Exists(_settings?.Path))
         {
            buffer = GetFile(context.Request.RawUrl?.Replace("%20", " "), response);
            if (buffer == null)
            {
               response.Headers.Set("Content-Type", "text/plain");
               response.StatusCode = (int) HttpStatusCode.NotFound;
               string error = "404 - not found";
               buffer = Encoding.UTF8.GetBytes(error);
            }
         }
         else
         {
            string error = $"Directory '{_settings?.Path}' not found";
            buffer = Encoding.UTF8.GetBytes(error);
         }

         response.ContentLength64 = buffer.Length;
         Stream output = response.OutputStream;
         output.Write(buffer, 0, buffer.Length);

         output.Close();

         await Listen();
      }
   }

   private void AddHeaders(HttpListenerResponse response, string extension)
   {
      response.Headers.Add("Content-Type",
         extension == "svg" ? "image/svg+xml" : $"{_extensions[extension]}/{extension}");
   }

   private byte[]? GetFile(string? rawUrl, HttpListenerResponse response)
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
         AddHeaders(response, extension);
      }

      return buffer;
   }

   public void Dispose() => Stop();
}