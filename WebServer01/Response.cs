using System;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer01
{
    class Response
    {
        byte[] data = null;
        string status;
        static string contentType = "";
        public Response(string status, string mime, byte[] data)
        {
            contentType = mime;
            this.status = status;
            this.data = data;
        }

        internal static Response From(Request request)
        {
            if (request == null)
            {
                return OnBadRequest("400.html", "400 Bad Request");
            }

            if (request.Type == "GET")
            {
                if (request.Uri.EndsWith("/"))
                {
                    request.Uri += "index.html";
                }
                string path = Environment.CurrentDirectory + HttpServer.WEB_DIR + request.Uri.Replace('/', '\\');
                FileInfo file = new FileInfo(path);

                if (file.Exists && file.Extension.Contains("."))
                {
                    return SendFrom(file);
                }
            }
            else
            {
                return OnBadRequest("405.html", "405 Method Not Allowed");
            }
            return OnBadRequest("404.html", "404 Page Not Found");
        }

        private static Response SendFrom(FileInfo file)
        {
            FileStream stream = file.OpenRead();
            byte[] buf = new byte[stream.Length];
            //BinaryReader reader = new BinaryReader(stream);
            //reader.Read(buf, 0, buf.Length);
            IAsyncResult asyncResult = stream.BeginRead(buf, 0, buf.Length, new AsyncCallback(Callback), stream);
            stream.EndRead(asyncResult);
            //string extension = file.Name.Substring(file.Name.LastIndexOf('.'));
            switch (file.Extension)
            {
                case ".htm":
                case ".html":
                    contentType = "text/html";
                    break;
                case ".css":
                    contentType = "text/stylesheet";
                    break;
                case ".js":
                    contentType = "text/javascript";
                    break;
                case ".jpg":
                    contentType = "image/jpeg";
                    break;
                case ".jpeg":
                case ".png":
                case ".gif":
                    contentType = "image/" + file.Extension.Substring(1);
                    break;
                default:
                    if (file.Extension.Length > 1)
                    {
                        contentType = "application/" + file.Extension.Substring(1);
                    }
                    else
                    {
                        contentType = "application/unknown";
                    }
                    break;
            }
            return new Response("200 OK", contentType, buf);
        }

        private static Response OnBadRequest(string filename, string status)
        {
            string path = Environment.CurrentDirectory + HttpServer.MSG_DIR + filename;
            byte[] data = null;
            if (new FileInfo(path).Exists)
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                }


            }
            return new Response(status, "text/html", data);

        }

        public void Post(NetworkStream stream)
        {
            StreamWriter writer = new StreamWriter(stream);

            Console.WriteLine(String.Format("Response:\r\n{0} {1}\r\nServer: {2}\r\nContent-Language: " +
                "ru\r\nContent-Type: {3}\r\nAccept-Ranges: bytes\r\nContent-Length: {4}\r\nConnection: close\r\n",
                HttpServer.VERSION, status, HttpServer.SERVERNAME, contentType, data.Length));
            //Console.WriteLine(Encoding.UTF8.GetString(data, 0, data.Length));

            writer.WriteLine(String.Format("{0} {1}\r\nServer: {2}\r\nContent-Language: " +
                "ru\r\nContent-Type: {3}\r\nAccept-Ranges: bytes\r\nContent-Length: {4}\r\nConnection: close\r\n",
                HttpServer.VERSION, status, HttpServer.SERVERNAME, contentType, data.Length));
            writer.Flush();
            //stream.Write(data, 0, data.Length);
            try
            {
                stream.BeginWrite(data, 0, data.Length, Callback, stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Callback(IAsyncResult ar)
        {
            Stream stream = ar.AsyncState as Stream;
            if (stream != null)
            {
                stream.Close();
            }
        }
    }
}
