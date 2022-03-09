using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FrostyPipeServer.ServerFiles
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    class TempFile
    {
        public List<int> PacketNumbersStored = new List<int>();
        public long ByteLengthOfFile;
        public int FileType;

    }
}
