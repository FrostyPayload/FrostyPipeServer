using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using FrostyPipeServer.ServerFiles;
using System.Text;
using System.Diagnostics;

namespace FrostyPipeServer
{
    public class Program
    {
    static bool isrunning;
      
        
        public static void Main(string[] args)
        {
            
            Console.WriteLine($"PIPE ONLINE SERVER V{Server.VERSIONNUMBER}");
            Console.WriteLine("Powered by Valve's GamenetworkingSockets");
            Console.WriteLine("Checking Directories..");


            Console.WriteLine("Directories good");
            ServerData.LoadData();
           
            Console.WriteLine($"Booting with maxplayers: {Server.MaxPlayers}, port: {Server.Port}, tick rate max: {Servermanager.TicksPerSecMax} ,min: {Servermanager.TicksPerSecMin}");
           
            isrunning = true;

            StartSendthread();
            StartReceiveThread();

            Console.OutputEncoding = Encoding.Unicode;


            Console.WriteLine("Url: " + System.Environment.GetEnvironmentVariable("ASPNETCORE_URLS"));
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    
                });
            
        /// <summary>
        /// Server's Sending thread loop
        /// </summary>
        private static void StartSendthread()
        {
            Thread _Sendingthread = new Thread(new ThreadStart(SendThread))
            {
                IsBackground = true
            };
            _Sendingthread.Start();

        }
        private static void SendThread()
        {
            Console.WriteLine($"Main Thread running at {Servermanager.TicksPerSecMax} ticks per second");
            DateTime _nextloop = DateTime.Now;

            
            while (isrunning)
            {
                while (_nextloop < DateTime.Now)
                {
                    GameLogic.Update();
                    double processtime = (DateTime.Now - _nextloop).TotalMilliseconds;
                    Servermanager.lastloopsendthread = processtime;
                    _nextloop = _nextloop.AddMilliseconds(Servermanager.MSPerTickTarget);
                    double timeout = (_nextloop - DateTime.Now).TotalMilliseconds;
                    Servermanager.lastloopsendthreadtimeout = timeout;
                    if (timeout>0)
                    {
                        Thread.Sleep((int)timeout);
                    }
                }
            }
        }

        /// <summary>
        /// Server's receving thread loop
        /// </summary>
        private static void StartReceiveThread()
        {
            Thread _receivethread = new Thread(new ThreadStart(ReceiveThread));
            _receivethread.Start();
        }
        private static void ReceiveThread()
        {
            Server.Run();

        }



    }
}
