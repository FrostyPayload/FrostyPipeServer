namespace FrostyPipeServer.Config
{
    public class ServerConfig
    {
        public int Maxplayers;
        public int Port;
        public string Servername;
        public int TickrateMax;
        public int TickrateMin;
        public string Adminpassword;
        public bool Publicise;
        public string POST;
        public string PUT;
        public string KEY;
        public string PortalUrl;


        public ServerConfig(bool createdefault = false)
        {
            if (createdefault)
            {
                Maxplayers = 8;
                Port = 7777;
                TickrateMax = 60;
                TickrateMin = 1;
                Servername = "PIPE server";
                Adminpassword = "DaveMirraXXX";
                Publicise = true;
                POST = "https://pipe-bmx-api.herokuapp.com/post";
                PUT = "https://pipe-bmx-api.herokuapp.com/update";
                KEY = "PIPE_BMX_Multiplayer_FROSTYP";
                PortalUrl = "http://*:5000;https://*:5001";

            }
        }

    }
}
