using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer01
{
    class Request
    {
        public string Type { get; set; }
        public string Uri { get; set; }
        public string Host { get; set; }
        public string Referer { get; set; }

        public Request(string type, string uri, string host, string referer )
        {
            Type = type;
            Uri = uri;
            Host = host;
            Referer = referer; 
        }

        internal static Request GetRequest(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return null;
            }
            string referer = "";
            string[] tokens = message.Split(' ', '\n', '/');
            //referer??
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == "Referer")
                {
                    referer = tokens[i + 1];
                }
            }
            Console.WriteLine("Type: {0}, Uri: {1}, Host: {2}, Referer: {3} ", tokens[0], tokens[2], tokens[6], referer);
            return new Request(tokens[0], tokens[2], tokens[6], referer);
        }
    }
}
