using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using System.Security.Cryptography;


namespace FrostyPipeServer.ServerFiles
{
    static class ServerData
    {
        public static List<SendReceiveIndex> OutgoingIndexes = new List<SendReceiveIndex>();
        public static List<SendReceiveIndex> IncomingIndexes = new List<SendReceiveIndex>();
        public static List<FileSegment> UpdateSegments = new List<FileSegment>();
        public static string Rootdir = "Game Data/";
        static string TempDir = Rootdir + "Temp/";
        public static string UpdateDir = Rootdir + $"FrostyPGameManager/Updates/{Server.VERSIONNUMBER}/";
        public static string ModelsDir = Rootdir + "Custom Players/";
        public static string MapsDir = Rootdir + "CustomMaps/";
        public static string ParkAssetsDir = Rootdir + "FrostyPGameManager/ParkBuilder/Assetbundles/";
        public static List<string> BannedWords = new List<string>();
        public static List<string> BanMessageAlternates = new List<string>();
        public static string AdminPassword = "DaveMirra";
        public static string Garagepath = "PIPE_Data/GarageContent/";
        public static string GarageSavespath =  Rootdir + "GarageContent/GarageSaves/";





        /// <summary>
        /// Load servers saved data
        /// </summary>
        public static void LoadData()
        {
            if (!Directory.Exists(Rootdir))
            {
                Directory.CreateDirectory(Rootdir);
            }
            if (!Directory.Exists(TempDir))
            {
                Directory.CreateDirectory(TempDir);
            }
            if (!Directory.Exists(UpdateDir))
            {
                Directory.CreateDirectory(UpdateDir);
            }
            if (!Directory.Exists(GarageSavespath))
            {
                Directory.CreateDirectory(GarageSavespath);
            }

            BannedWords = new List<string>
            {
                   {"gay"},
                   {"homo"},
                    {"queer"},
                     {"cunt"},
                      {"nigga"},
                       {"paki"},
                        {"bitch"},
                         {"niga" },
                          {"nga" },
                          {"ngga" },
                           {"btch" },
                            {"fuck" },
                             {"fck" },
                             {"prick"},

            };

            BanMessageAlternates = new List<string>
            {
                {"I admire you all" },
                {"You guys are Awesome" },
                {"Lets Session!" },

            };

            if (File.Exists("Config/Banwords.json"))
            {
                BannedWords = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText("Config/Banwords.json"));

            }
            else
            {
                string words = JsonConvert.SerializeObject(BannedWords);
                File.WriteAllText("Config/Banwords.json", words);
            }
            if (File.Exists("Config/Banprofiles.json"))
            {
                Server.BanProfiles = JsonConvert.DeserializeObject<List<BanProfile>>(File.ReadAllText("Config/Banprofiles.json"));

            }
            else
            {
                string profiles = JsonConvert.SerializeObject(Server.BanProfiles);
                File.WriteAllText("Config/Banprofiles.json", profiles);
            }
            if (File.Exists("Config/ServerConfig.json"))
            {
                Server.Config = JsonConvert.DeserializeObject<Config.ServerConfig>(Servermanager.ServerconfigString=File.ReadAllText("Config/ServerConfig.json"));

            }
            else
            {
                Console.WriteLine("Config needed, making default file at Config/ServerConfig.json");
                Server.Config = new Config.ServerConfig(true);
                string confstring = JsonConvert.SerializeObject(Server.Config);
                Servermanager.ServerconfigString = confstring;
                File.WriteAllText("Config/ServerConfig.json", confstring);
            }
            if (new DirectoryInfo(UpdateDir).GetFiles().Length < 2)
            {
                Console.WriteLine($"There aren't any files in Game Data/FrostyPGameManager/Updates/{Server.VERSIONNUMBER}, consider adding all relevant Mod-Side files there for Auto update serving to clients PIPE_Data/FrostyPGameManager/Updates/{Server.VERSIONNUMBER}");
            }

            Server.Port = Server.Config.Port.ToString();
            Servermanager.TicksPerSecMin = Server.Config.TickrateMin;
            Servermanager.TicksPerSecMax = Server.Config.TickrateMax;
            Server.SERVERNAME = Server.Config.Servername;
            Server.MaxPlayers = Server.Config.Maxplayers;
            AdminPassword = Server.Config.Adminpassword;
            Server.Public_server = Server.Config.Publicise;
            Server.posturl = Server.Config.POST;
            Server.puturl = Server.Config.PUT;
            Server.APIKEY = Server.Config.KEY;
            System.Environment.SetEnvironmentVariable("ASPNETCORE_URLS", Server.Config.PortalUrl);
            Console.WriteLine("Set Url: " + System.Environment.GetEnvironmentVariable("ASPNETCORE_URLS"));


        }

        public static void SaveServerData()
        {
           
            string jsonbanwords = JsonConvert.SerializeObject(BannedWords);
            File.WriteAllText("Config/Banwords.json", jsonbanwords);

            string jsonbanprofiles = JsonConvert.SerializeObject(Server.BanProfiles);
            File.WriteAllText("Config/Banprofiles.json", jsonbanprofiles);
        }

        public static void BanPlayer(string _username, string IP, uint connid, int mins)
        {
            DateTime Time_of_release = DateTime.Now.AddMinutes(mins);

            Server.BanProfiles.Add(new BanProfile(IP, _username, connid, Time_of_release));
            SaveServerData();
            ServerSend.DisconnectPlayer("Banned until " + Time_of_release.ToString() + " UTC", connid);

        }

        public static void AlterBanWords(uint admin, bool add, string word)
        {
            if (add)
            {
                bool found = false;
                for (int i = 0; i < BannedWords.Count; i++)
                {
                    if (BannedWords[i].ToLower() == word.ToLower())
                    {
                        found = true;
                    }

                }
                if (!found)
                {
                    BannedWords.Add(word);
                    SaveServerData();
                    ServerSend.SendTextFromServerToOne(admin, $"Added {word}");

                }
                else
                {
                    ServerSend.SendTextFromServerToOne(admin, $"{word} already stored");
                }
            }
            else
            {
                for (int i = 0; i < BannedWords.Count; i++)
                {
                    if (BannedWords[i] == word)
                    {
                        BannedWords.RemoveAt(i);
                        SaveServerData();
                        ServerSend.SendTextFromServerToOne(admin, $"Removed {word}");
                    }

                }

            }
        }

        public static void FileCheckAndSend(string FileName, List<int> _packetsowned, uint _from, string dir)
        {
            FileInfo _fileinfo = null;

            int lastslash = FileName.LastIndexOf("/");
            if (lastslash != -1)
            {
                FileName = FileName.Remove(0, lastslash + 1);
            }
            // make dir
            int Dta = dir.ToLower().LastIndexOf("pipe_data");
            Console.WriteLine($"Dta: {Dta}, Dir: {dir}");
            string fulldir = Rootdir + dir.Remove(0, Dta + 10);




            // Find Fileinfo
            FileInfo finfo = FileNameMatcher(new DirectoryInfo(fulldir).GetFiles(), FileName);
            if (finfo != null)
            {

                _fileinfo = finfo;
                Console.WriteLine($"Located {FileName}");



            }

            // file not found
            if (_fileinfo == null)
            {
                ServerSend.SendTextFromServerToOne(_from, "Server doesn't have this file yet, ask owner to upload");
                return;
            }


            FileStream Stream = _fileinfo.OpenRead();

            // get total packets and bytelength
            float PacketCount = 0;
            long length = Stream.Length;
            long mod = length % 400000;
            if (mod > 0)
            {
                PacketCount = length / 400000 + 1;
            }
            if (mod == 0)
            {
                PacketCount = length / 400000;
            }
            if (length < 400000)
            {
                PacketCount = 1;
            }


            SendReceiveIndex NewSend = new SendReceiveIndex(FileName, (int)PacketCount, dir);
            NewSend.PacketNumbersStored = _packetsowned;
            NewSend.ByteLength = length;
            NewSend.PlayerTosendTo = _from;
            NewSend.Fileinfo = _fileinfo;
            NewSend.isSending = true;

            Stream.Close();

            OutgoingIndexes.Add(NewSend);


        }

        public static void FileSaver(byte[] bytes, string name, int SegsTotal, int SegNo, uint _player, long Totalbytes, string path)
        {

            Console.WriteLine($"Receiving file: {name}, path: {path}, Segment: {SegNo}");
            try
            {
                if (Totalbytes > 400000000)
                {
                    if (Server.Players.TryGetValue(_player, out Player player))
                    {
                        ServerSend.FileStatus(_player, name, 1);
                        ServerSend.SendTextFromServerToOne(_player, "File over max limit");
                        return;
                    }
                }
                string originname = name;
                // make sure filename is just filename
                int lastslash = name.LastIndexOf("/");
                if (lastslash != -1)
                {
                    name = name.Remove(0, lastslash + 1);
                }
                // make our directory
                int pdata = path.ToLower().LastIndexOf("pipe_data");
                string _mypath = path.Remove(0, pdata + 10);


                if (!Directory.Exists(TempDir + _mypath)) Directory.CreateDirectory(TempDir + _mypath);
                if (!Directory.Exists(Rootdir + _mypath)) Directory.CreateDirectory(Rootdir + _mypath);

                FileInfo Finfo = FileNameMatcher(new DirectoryInfo(Rootdir + _mypath).GetFiles(), name);
                FileInfo TempFinfo = FileNameMatcher(new DirectoryInfo(TempDir + _mypath).GetFiles(), name + ".json");

                if (Finfo != null)
                {
                    Console.WriteLine($"incoming data for file already found in location: {Finfo.FullName} ");
                    if (Server.Players.TryGetValue(_player, out Player player))
                    {
                        ServerSend.FileStatus(_player, name, (int)FileStatus.Received);
                    }
                    return;
                }



                // if no Temp file exists create one 
                if (TempFinfo == null)
                {
                    TempFile Temp = new TempFile();
                    Temp.ByteLengthOfFile = Totalbytes;
                    File.WriteAllText(TempDir + _mypath + name + ".json", JsonConvert.SerializeObject(Temp));

                }
                else
                {
                    name = TempFinfo.Name.Replace(".json", "");
                }
                
                



                // grab Receive index and FileStream
                SendReceiveIndex InIndex = null;
                foreach (SendReceiveIndex r in IncomingIndexes)
                {
                    if (r.NameOfFile.Replace("_", " ").Replace("(1)", "").Replace("(2)", "").Replace("(3)", "").ToLower() == name.Replace("_", " ").Replace("(1)", "").Replace("(2)", "").Replace("(3)", "").ToLower())
                    {
                        InIndex = r;
                        InIndex.NameOfFile = name;
                    }
                }
                if (InIndex == null)
                {
                    Console.WriteLine($"File receive with no Index");
                    return;
                }



                if (InIndex.PacketNumbersStored.Contains(SegNo))
                {
                    return;
                }
                FileStream _f = File.OpenWrite(TempDir + _mypath + name);

                if (!InIndex.IsReceiving)
                {
                    InIndex.IsReceiving = true;
                }
                InIndex.ByteLength = Totalbytes;
                InIndex.TotalPacketsinFile = SegsTotal;


                // determine where to write data
                int Startpos = (SegNo - 1) * 400000;

                // Write packet to its location in file
                _f.Seek(Startpos, SeekOrigin.Begin);
                _f.Write(bytes, 0, bytes.Length);
                _f.Close();


                // update SendReceive index
                InIndex.PacketNumbersStored.Add(SegNo);


                // Do Save or just update temp file
                if (InIndex.PacketNumbersStored.Count == InIndex.TotalPacketsinFile)
                {

                    string FullPath = _mypath.Insert(0, Rootdir);
                    Console.WriteLine("Saving file to path: " + FullPath);

                    if (!Directory.Exists(FullPath)) Directory.CreateDirectory(FullPath);

                    // take out anything weird
                    string savename = name.Replace("_", " ").Replace("(1)", "").Replace("(2)", "").Replace("(3)", "");


                    if (!File.Exists(FullPath + savename))
                    {
                        File.Move(TempDir + _mypath + name, FullPath + savename);

                    }
                    else
                    {
                        Console.WriteLine("Dumped file, duplicate found");
                    }
                    File.Delete(TempDir + _mypath + name + ".json");

                    // Tell Anyone that i've asked to send this file that i have it now ,if still online
                    foreach (uint id in InIndex.PlayersRequestedFrom)
                    {
                        if (Server.Players.TryGetValue(id, out Player player))
                        {
                            ServerSend.FileStatus(id, originname, (int)FileStatus.Received);

                        }
                    }

                    IncomingIndexes.Remove(InIndex);
                    Console.WriteLine($"Saved {savename} to {FullPath}");



                }
                else
                {
                    // update temp file
                    Console.Write($"Reading json from {TempDir + _mypath + name + ".json"}");
                    string jsonpath = (TempDir + _mypath + name + ".json").ToString();
                    TempFile _temp = JsonConvert.DeserializeObject<TempFile>(File.ReadAllText(jsonpath));
                    _temp.PacketNumbersStored.Add(SegNo);
                    File.WriteAllText(TempDir + _mypath + name + ".json", JsonConvert.SerializeObject(_temp));

                }


            }
            catch (Exception x)
            {

                Console.WriteLine(x);
            }


        }


        public static void FileCheckAndRequest(string Filename, uint _fromclient, string dir)
        {

            try
            {


                bool found = false;
                if (Filename.ToLower() != "e" && Filename != "" && Filename != " " && Filename != "stock")
                {

                    int lastslash = Filename.LastIndexOf("/");
                    if (lastslash != -1)
                    {
                        Filename = Filename.Remove(0, lastslash + 1);
                    }

                    string name = Filename;
                    Filename = ConvertToUnicode(name);

                    int pdata = dir.ToLower().LastIndexOf("pipe_data");
                    string mydir = dir.Remove(0, pdata + 10);

                    Console.WriteLine($"Filecheck: Full path: {Rootdir + mydir} :: file: {Filename}");

                    if (!Directory.Exists(Rootdir + mydir)) Directory.CreateDirectory(Rootdir + mydir);
                    if (!Directory.Exists(TempDir + mydir)) Directory.CreateDirectory(TempDir + mydir);



                    FileInfo Finfo = FileNameMatcher(new DirectoryInfo(Rootdir + mydir).GetFiles(), Filename);



                    // if not found
                    if (Finfo == null)
                    {
                        Console.WriteLine($"File not located, looking for temp data..");

                        List<int> Packetsiown = new List<int>();
                        if (File.Exists(TempDir + mydir + Filename + ".json"))
                        {
                            TempFile temp = JsonConvert.DeserializeObject<TempFile>(TempDir + mydir + Filename + ".json");

                            foreach (int seg in temp.PacketNumbersStored)
                            {
                                Packetsiown.Add(seg);

                            }
                            Console.WriteLine($"Temp data located: packets stored: {Packetsiown.Count}: byte length: {temp.ByteLengthOfFile}");
                        }

                        if (Server.Players.TryGetValue(_fromclient, out Player player))
                        {

                            ServerSend.RequestFile(_fromclient, Filename, Packetsiown, dir);
                            Console.WriteLine(Filename + $" requested from {player.Username}");

                        }

                    }



                }




            }
            catch (Exception)
            {


            }

        }

        public static List<string> GiveUpdateFileNames()
        {
            List<string> files = new List<string>();
            foreach (FileInfo f in new DirectoryInfo(UpdateDir).GetFiles())
            {
                files.Add(f.Name);
            }



            return files;

        }

        public static string ConvertToUnicode(string text)
        {
            Encoding Uni = Encoding.Unicode;
            string outstring = Uni.GetString(Uni.GetBytes(text));


            outstring = outstring.Trim(Path.GetInvalidPathChars());
            outstring = outstring.Trim(Path.GetInvalidFileNameChars());
            return outstring;
        }

        public static FileInfo FileNameMatcher(FileInfo[] myfiles, string filetomatch)
        {
            bool found = false;
            for (int i = 0; i < myfiles.Length; i++)
            {
                if (!found)
                {

                    if (myfiles[i].Name.ToLower() == filetomatch.ToLower())
                    {
                        found = true;
                        return myfiles[i];
                    }
                    else if (myfiles[i].Name.Replace("_", " ").ToLower() == filetomatch.Replace("_", " ").ToLower())
                    {
                        found = true;
                        return myfiles[i];
                    }
                    else if (myfiles[i].Name.Replace("_", " ").Replace("(1)", "").ToLower() == filetomatch.Replace("_", " ").Replace("(1)", "").ToLower())
                    {
                        found = true;
                        return myfiles[i];
                    }
                    else if (myfiles[i].Name.Replace("_", " ").Replace("(2)", "").ToLower() == filetomatch.Replace("_", " ").Replace("(2)", "").ToLower())
                    {
                        found = true;
                        return myfiles[i];
                    }
                    else if (myfiles[i].Name.Replace("_", " ").Replace("(3)", "").ToLower() == filetomatch.Replace("_", " ").Replace("(3)", "").ToLower())
                    {
                        found = true;
                        return myfiles[i];
                    }

                }
            }


            return null;


        }


    }

    /// <summary>
    /// Used for storing a segment of a textures byte array until all have been received
    /// </summary>
    [Serializable]
    class FileSegment
    {

        public byte[] segment;
        public string NameofFile;
        public int segment_count;
        public int this_segment_num;
        public uint client;
        public int Filetype;
        public int senttimes;
        public long FileByteCount;
        public string path;



        public FileSegment(byte[] _seg, string _name, int _segcount, int _thissegno, uint _player, int _filetype, long _bytecount, string _path)
        {
            segment = _seg;
            NameofFile = _name;
            segment_count = _segcount;
            this_segment_num = _thissegno;
            client = _player;
            Filetype = _filetype;
            FileByteCount = _bytecount;
            path = _path;
        }

    }

    [Serializable]
    public class BanProfile
    {
        public string IP;
        public string Username;
        public uint ConnId;
        public DateTime Timeofbanrelease;


        public BanProfile(string ip, string username, uint conid, DateTime timeofrelease)
        {
            IP = ip;
            Username = username;
            ConnId = conid;
            Timeofbanrelease = timeofrelease;
        }


    }

    public enum FileType
    {
        Texture = 1,
        Map = 2,
        PlayerModel = 3,
        Mesh = 4,
        ParkAsset = 5,
        Update = 6,
    }

    public class SendReceiveIndex
    {
        public List<int> PacketNumbersStored = new List<int>();
        public string NameOfFile;
        public int Filetype;
        public int TotalPacketsinFile;
        public bool isSending;
        public bool IsReceiving;
        public long ByteLength;
        public FileInfo Fileinfo;
        public uint PlayerTosendTo;
        public List<uint> PlayersRequestedFrom = new List<uint>();
        public string directory;
        public string Hash;


        public SendReceiveIndex(string _filename, int _totalpackets, string dir)
        {
            NameOfFile = _filename;
            TotalPacketsinFile = _totalpackets;
            directory = dir;
        }

        public SendReceiveIndex(string _filename)
        {
            NameOfFile = _filename;
        }



    }

    public enum FileStatus
    {
        Received = 1,
        Cancel = 2,

    }



}
