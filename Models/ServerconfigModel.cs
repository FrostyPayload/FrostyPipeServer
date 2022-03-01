using FrostyPipeServer.ServerFiles;
using System.Diagnostics;
using System;
using System.Collections.Generic;
namespace FrostyPipeServer.Models
{
    public class ServerconfigModel
    {
        public int CurrentRam = 0;
        public int AvailableRam = 0;
        public int Port;
        public bool Publicise;
        public int Maxplayers;
        public int Currenttickrate;
        public int TickMax;
        public int Tickmin;
        public string Servername;
        public List<Player> PlayerList;

    }
}
