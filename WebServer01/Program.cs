﻿using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer01
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpServer server = new HttpServer(8080);
            server.Start();

        }
    }
}
