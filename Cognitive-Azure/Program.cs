﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Cognitive_Azure
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseKestrel(x => x.AddServerHeader = false)
                   .UseStartup<Startup>()
                   .Build();
    }
}