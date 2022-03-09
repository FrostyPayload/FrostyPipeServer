using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FrostyPipeServer.ServerFiles
{
    [JsonObject]
    class ServerDataFile
    {

        public List<string> Banwords;
        public List<BanProfile> Banprofiles;

        public ServerDataFile(List<string> banwords, List<BanProfile> bans)
        {
            Banwords = banwords;
            Banprofiles = bans;
        }



    }
}
