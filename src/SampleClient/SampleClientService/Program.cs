using System;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SampleClientService
{
    class Program
    {
        static void Main(string[] args)
        {
            var host =  WebHost.CreateDefaultBuilder(args)
                   .UseUrls("http://localhost:5000")
                   .UseKestrel()
                   .UseStartup<Startup>()
                   .Build();
            //var webHost = new WebHostBuilder()
            //        .UseKestrel()
            //        .UseStartup(typeof (Startup))
            //        .Build();

            //webHost.Start();

            host.Start();

            Console.ReadLine();
        }
    }
}
