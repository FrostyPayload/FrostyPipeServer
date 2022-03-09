using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime;
using System.Xml.Serialization;
using System.Reflection;


namespace FrostyPipeServer.ServerFiles
{
    public static class Servermanager
    {
        static Process proc = Process.GetCurrentProcess();
        public static int TicksPerSecMax = 60;
        public static int MSPerTickMax = 1000 / TicksPerSecMax;
        public static int TicksPerSecMin = 1;
        public static int MSPerTickMin = 1000 / TicksPerSecMin;
        public static float MSPerTickTarget = 1000;
        public static string ServerconfigString;
        public static double lastloopreceivethread;
        public static double lastloopreceivethreadtimeout;
        public static double lastloopsendthread;
        public static double lastloopsendthreadtimeout;
        public static void Update()
        {
            UpdateTicks();
        }

        static void UpdateTicks()
        {
            if (Server.Players.Count < 1)
            {
                if (MSPerTickTarget == MSPerTickMax)
                {
                    Console.WriteLine("Tick rate dropping to standby");
                }
                MSPerTickTarget = MSPerTickMin;
            }
            else
            {
                if (MSPerTickTarget == MSPerTickMin)
                {
                    Console.WriteLine("Tick rate leaving standby");
                }
                MSPerTickTarget = MSPerTickMax;
            }
        }

        public static bool ReloadConfig()
        {
            Console.WriteLine("Reloading config");
            ServerData.LoadData();
            return true;
        }

        public static string GiveConfigasJSONString()
        {
            return ServerconfigString;
        }
        public static string GiveStatsasJSONString()
        {
            double tickSecs = (double)(MSPerTickTarget / 1000);
            int tickratenow = (int)Math.Floor(1/tickSecs);

            Dictionary<string,string> stats = new Dictionary<string,string>();
            stats.Add("ramused", GetRamUsedMBytes().ToString());
            stats.Add("players", Server.Players.Count.ToString());
            stats.Add("cpuused", GetCpuUsage());
            stats.Add("trnow",tickratenow.ToString());
            stats.Add("lastlooprec", lastloopreceivethread.ToString());
            stats.Add("lastloopsen", lastloopsendthread.ToString());
            stats.Add("lastloopsentimeout", lastloopsendthreadtimeout.ToString());
            stats.Add("lastlooprectimeout", lastloopreceivethreadtimeout.ToString());
            stats.Add("bytesout", GetKBOut().ToString());
            stats.Add("bytesin", GetKBIn().ToString());
            stats.Add("avping", Server.GetAveragePing().ToString());
            return JsonConvert.SerializeObject(stats);
        }
        public static bool OverwriteConfigFile(string jsonconfig)
        {
            Console.WriteLine("Overwriting config..");
            
            try
            {
                Config.ServerConfig configtest = JsonConvert.DeserializeObject<Config.ServerConfig>(jsonconfig);
                if (configtest != null)
                {
                    File.WriteAllText("Config/ServerConfig.json", jsonconfig);
                    Server.Config = configtest;
                    ServerconfigString = jsonconfig;
                    Console.WriteLine("Done");
                    return true;
                }
                else
                {
                    Console.WriteLine("invalid config");
                    return false;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("invalid config");
                return false;
            }
        }


        public static long GetRamUsedMBytes()
        {
            /*
            Console.WriteLine($"  Physical memory usage     : {proc.WorkingSet64}");
            Console.WriteLine($"  Base priority             : {proc.BasePriority}");
            Console.WriteLine($"  Priority class            : {proc.PriorityClass}");
            Console.WriteLine($"  User processor time       : {proc.UserProcessorTime}");
            Console.WriteLine($"  Privileged processor time : {proc.PrivilegedProcessorTime}");
            Console.WriteLine($"  Total processor time      : {proc.TotalProcessorTime}");
            Console.WriteLine($"  Paged system memory size  : {proc.PagedSystemMemorySize64}");
            Console.WriteLine($"  Paged memory size         : {proc.PagedMemorySize64}");
            Console.WriteLine($"  Private memory size  64   : {proc.PrivateMemorySize64}");
            Console.WriteLine($"  Private memory size       : {proc.PrivateMemorySize}");
            */
            proc = Process.GetCurrentProcess();
            return proc.PrivateMemorySize64/1024/1024;
        }
        public static void GetRamAvailable()
        {

        }
        public static float GetKBOut()
        {
            Valve.Sockets.ConnectionStatus constat = new Valve.Sockets.ConnectionStatus();
            Server.Connection.GetQuickConnectionStatus(Server.ConnectedRiders, ref constat);
            return constat.outBytesPerSecond/1024;
        }
        public static float GetKBIn()
        {
            Valve.Sockets.ConnectionStatus constat = new Valve.Sockets.ConnectionStatus();
            Server.Connection.GetQuickConnectionStatus(Server.ConnectedRiders, ref constat);
            return constat.inBytesPerSecond / 1024;
        }
        public static string GetCpuUsage()
        {
            proc = Process.GetCurrentProcess();
            return proc.TotalProcessorTime.Milliseconds.ToString();
        }





        public static SaveList GarageDeserialize(string presetname, uint playerid)
        {
            try
            {
                if (Server.Players.ContainsKey(playerid))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(SaveList));
                    TextReader reader = new StreamReader(ServerData.GarageSavespath + presetname + "_ID_" + Server.Players[playerid].Username.ToLower() + ".preset");
                    object obj = deserializer.Deserialize(reader);
                    reader.Close();
                    return (SaveList)obj;
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message + " : " + e);
                return null;
            }
        }
        public static bool SaveGaragePreset(string rawxml, uint playerid, string presetname)
        {
            if (Server.Players.ContainsKey(playerid))
            { 
                File.WriteAllText(ServerData.GarageSavespath + presetname + "_ID_" + Server.Players[playerid].Username.ToLower()+".preset",rawxml);
                return true;

            }
            else
            {
                return false;
            }

        }


    }
}
