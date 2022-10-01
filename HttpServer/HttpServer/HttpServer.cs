using System;
using System.Net;
using System.IO;

namespace HttpServer;

public class HttpServer
{
   private readonly string _url;
   private readonly HttpListener _listener;
   private HttpListenerContext _httpListenerContext;
   private bool isWorking;

   public HttpServer(string url)
   {
      _listener = new HttpListener();
      _url = url;
      
      _listener.Prefixes.Add(_url);
   }

   public void Start()
   {
      if (isWorking)
      {
         Console.WriteLine("Сервер уже работает");
         return;
      }

      Console.WriteLine("Запуск сервера...");
      _listener.Start();
      isWorking = true;
      Console.WriteLine("Сервер запущен");

      Listen();
   }
   
   public void Stop()
   {
      if(!isWorking) return;
      isWorking = false;
      Console.WriteLine("Остановка сервера...");
      _listener.Stop();
      Console.WriteLine("Сервер остановлен");
   }

   private void Listen()
   {
      _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
   }
   
      private void ListenerCallback(IAsyncResult result)
   {
      if (_listener.IsListening)
      {
         HttpListenerContext context = _listener.EndGetContext(result);
         HttpListenerRequest request = context.Request;
         // получаем объект ответа
         HttpListenerResponse response = context.Response;
         // создаем ответ в виде кода html
         
         response.Headers.Set("Content-Type", "text/html");
         byte[] buffer = File.ReadAllBytes(@"./Google/google.html");
         
         // получаем поток ответа и пишем в него ответ
         response.ContentLength64 = buffer.Length;
         
         Stream output = response.OutputStream;
         output.Write(buffer, 0, buffer.Length);
         
         // закрываем поток
         output.Close();
         
         Listen();
      }
   }
}