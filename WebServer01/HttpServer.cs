using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WebServer01
{
    class HttpServer
    {
        public const String MSG_DIR = "\\root\\msg\\";
        public const String WEB_DIR = "\\root\\web\\";
        public const String VERSION = "HTTP/1.0";
        public const String SERVERNAME = "myserv/1.1";

        System.Net.Sockets.TcpListener tcpListener;
        bool running = false;

        public HttpServer(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            Thread thread = new Thread(Run);
            thread.Start();
        }

        private void Run()
        {
            tcpListener.Start();
            running = true;
            Console.WriteLine("Server is running...");
            while (running)
            {
                Console.WriteLine("Waiting for connection...");
                TcpClient client = tcpListener.AcceptTcpClient();
                Console.WriteLine("client is connected.");
                Console.WriteLine();
                HandleClient(client);
                client.Close();
            }
            running = false;
            tcpListener.Stop();

        }

        private void HandleClient(TcpClient client)
        {
            StreamReader stream = new StreamReader(client.GetStream());
            string message = null;
            while (stream.Peek() != -1)
            {
                message += stream.ReadLine() + "\n";
            }
            Console.WriteLine("REQUEST: {0}", message);
            Request request = Request.GetRequest(message);
            Response response = Response.From(request);
            if (response != null)
            {
                response.Post(client.GetStream());
            }
            else
            {
                Console.WriteLine("File not found.");
            }
        }
    }
}
