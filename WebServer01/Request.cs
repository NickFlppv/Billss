using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer01
{
    class Request
    {
        public string Type { get; set; }
        public string Uri { get; set; }
        //public string Host { get; set; }

        public Request(string type, string uri)
        {
            Type = type;
            Uri = uri;
            //Host = host;
        }

        internal static Request GetRequest(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return null;
            }
            Match reqMatch = Regex.Match(message, @"^\w+\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");
            string[] tokens = reqMatch.Value.Split(' ');
            Console.WriteLine("Type: {0}, Uri: {1}", tokens[0], tokens[1]);
            return new Request(tokens[0], tokens[1]);
        }
    }
}
