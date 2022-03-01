using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Diagnostics;


namespace FrostyPipeServer.ServerFiles
{

    /// <summary>
    /// A Successful connection to the server ultimatley results in this class being made and populated for the client, if your ValveConnectionID is linked to a Player class in the servers player list your online
    /// </summary>
    public class Player
    {
        public string Username;
        public uint RiderID;
        public string Ridermodel;
        public string Ridermodelbundlename;
        public string MapName;
        public float VersionNo;


        public Vector3 RiderRootPosition;
        public Vector3 RiderRootRotation;

        /// <summary>
        /// All rider and bike customizable data
        /// </summary>
        public GearUpdate Gear;
        /// <summary>
        /// switching to replaymode etc sends command to flip this bool, causing streamed data (positions and audio) to not be sent to them
        /// </summary>
        public bool ReadytoRoll = false;
        /// <summary>
        /// Player can toggle mapoverride for individual players, causing map name matching to be bypassed (positions and audio will stream)
        /// </summary>
        public Dictionary<uint, bool> SendDataOverrides = new Dictionary<uint, bool>();

        // 5 gets a 10 mins ban
        public int AmountofObjectBoots;
        public List<NetGameObject> PlayerObjects;

        public bool AdminLoggedIn = false;
        public bool AdminStream = false;
        public Stopwatch AdminStreamWatch = new Stopwatch();
        public int ping;


        /// <summary>
        /// Constructor to initiailise with Connection id and rider info, also initialises vectors for storage
        /// </summary>
        public Player(uint connofPlayer, string _riderModel, string _username, string _ridermodelbundlename, string currentlevel, float versionNo)
        {
            RiderID = connofPlayer;
            Username = _username;
            Ridermodel = _riderModel;
            Ridermodelbundlename = _ridermodelbundlename;
            MapName = currentlevel;

            PlayerObjects = new List<NetGameObject>();
            Gear = new GearUpdate();
            Gear.RiderTextures = new List<TextureInfo>();
            VersionNo = versionNo;


        }
    }


    /// <summary>
    /// Used for keeping track of texture name and the gameobject its on for when it reaches remote players
    /// </summary>
    public class TextureInfo
    {
        public string Nameoftexture;
        public string NameofparentGameObject;
        public bool isNormal;
        public int Matnum;
        public string directory;

        public TextureInfo(string nameoftex, string nameofG_O, bool isnormal, int matnum, string _directory)
        {
            Nameoftexture = nameoftex;
            NameofparentGameObject = nameofG_O;
            isNormal = isnormal;
            Matnum = matnum;
            directory = _directory;
        }


    }



    public class NetGameObject
    {
        public string NameofObject;
        public string NameofAssetBundle;
        public string NameOfFile;
        public Vector3 Rotation;
        public Vector3 Position;
        public Vector3 Scale;
        public bool IsPhysics;
        public int ObjectID;
        public List<uint> Votestoremove = new List<uint>();
        public string Directory;

        public NetGameObject(string _nameofobject, string _nameoffile, string _nameofassetbundle, Vector3 _rotation, Vector3 _position, Vector3 _scale, bool _IsPhysicsenabled, int Objectid, string dir)
        {
            NameofObject = _nameofobject;
            NameOfFile = _nameoffile;
            NameofAssetBundle = _nameofassetbundle;
            Rotation = _rotation;
            Position = _position;
            Scale = _scale;
            IsPhysics = _IsPhysicsenabled;
            ObjectID = Objectid;
            Directory = dir;
        }


    }


    public class GearUpdate
    {
        public bool isRiderUpdate;
        public List<TextureInfo> RiderTextures;
        public bool Capforward;
        public byte[] Garagesave;



        /// <summary>
        /// to Send Just Riders gear
        /// </summary>
        /// <param name="ridertextures"></param>
        public GearUpdate(List<TextureInfo> ridertextures)
        {
            isRiderUpdate = true;
            RiderTextures = ridertextures;


        }


        public GearUpdate()
        {
            RiderTextures = new List<TextureInfo>();
            Garagesave = new byte[1];
        }

    }



}
