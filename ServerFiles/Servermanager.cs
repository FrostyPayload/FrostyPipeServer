using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net;
using System.IO;


namespace FrostyPipeServer.ServerFiles
{
    public static class Servermanager
    {
        static Process proc = Process.GetCurrentProcess();
        public static int TicksPerSecMax = 60;
        public static int MSPerTickMax = 1000 / TicksPerSecMax;
        public static int TicksPerSecMin = 1;
        public static int MSPerTickMin = 1000 / TicksPerSecMin;
        public static int MSPerTickCurrent;
        public static string ServerconfigString;
        public static void Update()
        {
            UpdateTicks();
                       

        }

        static void UpdateTicks()
        {
            if (Server.Players.Count < 1)
            {
                if (MSPerTickCurrent == MSPerTickMax)
                {
                    Console.WriteLine("Tick rate dropping to standby");
                }
                MSPerTickCurrent = MSPerTickMin; return;
            }
            else
            {
                if (MSPerTickCurrent == MSPerTickMin)
                {
                    Console.WriteLine("Tick rate leaving standby");
                }
                MSPerTickCurrent = MSPerTickMax;
            }
        }

        public static bool ReloadConfig()
        {
            Console.WriteLine("Reloading config");
            ServerData.LoadData();
            return true;
        }

        public static string GiveConfigasString()
        {
            return ServerconfigString;
        }

        public static long GetRamUsedBytes()
        {
            return proc.PrivateMemorySize64;
        }
        public static void GetRamAvailable()
        {

        }

        public static long GetBytesOut()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return 0;

            NetworkInterface[] interfaces
                = NetworkInterface.GetAllNetworkInterfaces();

            long bytesout = 0;
            foreach (NetworkInterface ni in interfaces)
            {
                bytesout = bytesout + ni.GetIPv4Statistics().BytesSent;
            }
            return bytesout;
        }



    }
}
