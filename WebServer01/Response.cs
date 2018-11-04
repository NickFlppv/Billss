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
        string contentType;
        public Response(string status, string mime, byte[] data)
        {
            this.status = status;
            this.contentType = mime;
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
                string path = Environment.CurrentDirectory + HttpServer.WEB_DIR + request.Uri;
                FileInfo file = new FileInfo(path);

                if (file.Exists && file.Extension.Contains("."))
                {
                    return MakeFrom(file);
                }
                else
                {
                    DirectoryInfo directory = new DirectoryInfo(file + "\\");
                    if (!directory.Exists)
                    {
                        return OnBadRequest("404.html", "404 Page Not Found");
                    }
                    FileInfo[] files = directory.GetFiles();
                    foreach (FileInfo ff in files)
                    {
                        if (ff.Name.Contains("default.html") || ff.Name.Contains("index.html"))
                            return MakeFrom(ff);
                    }
                }
            }
            else
            {
                return OnBadRequest("405.html", "405 Method Not Allowed");
            }
            return OnBadRequest("404.html", "404 Page Not Found");
        }

        private static Response MakeFrom(FileInfo file)
        {
            FileStream stream = file.OpenRead();
            Byte[] buf = new Byte[stream.Length];
            BinaryReader reader = new BinaryReader(stream);
            reader.Read(buf, 0, buf.Length);

            return new Response("200 OK", "text/html", buf);
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
            stream.BeginWrite(data, 0, data.Length, Callback, stream);
        }

        private void Callback(IAsyncResult ar)
        {
            Stream stream = ar.AsyncState as Stream;
            if (stream != null)
            {
                stream.Close();
            }
        }
    }
}
