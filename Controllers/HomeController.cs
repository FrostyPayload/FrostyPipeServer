using FrostyPipeServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FrostyPipeServer.ServerFiles;
using Valve.Sockets;

namespace FrostyPipeServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Configuration()
        {
            return View();
        }
        [Authorize]
        public IActionResult Docs()
        {
            return View();
        }
        [Authorize]
        public IActionResult Players()
        {
            return View(new PlayersModel(Server.Players.Values.ToList()));
        }
        [Authorize]
        public IActionResult Objects()
        {
            return View();
        }
        [Authorize]
        public IActionResult Games()
        {
            return View();
        }




        [HttpGet("/serverconfig")]
        public string Getconfig()
        {
            return ServerFiles.Servermanager.GiveConfigasJSONString();
        }


        [HttpPost("/applyconfig")]
        public string ApplyChangestoConfig([FromHeader] string jsonconfig)
        {
            bool success = Servermanager.OverwriteConfigFile(jsonconfig);
            if (!success)
            {
                return "Something went wrong";
            }
            else
            {
                return "Changes Applied";
            }
        }


        [HttpGet("/serverstats")]
        public string GetStats()
        {
            return ServerFiles.Servermanager.GiveStatsasJSONString();
        }


        [HttpGet("/reloadconfig")]
        public string SendReloadConfig()
        {
            ServerFiles.Servermanager.ReloadConfig();
            return "Reloaded config";
        }


        [HttpPost("/banridersend")]
        public string BanRiderSend([FromHeader] string id, [FromHeader] string mins)
        {
            if(uint.TryParse(id, out uint playerid))
            {
                int _mins = int.Parse(mins);
                Console.WriteLine("Banning id: " + playerid + " for " + _mins.ToString() + " Mins");
                if (Server.Players.ContainsKey(playerid))
                {
                    ConnectionInfo info = new ConnectionInfo();
                    Server.Connection.GetConnectionInfo(playerid, ref info);
                    ServerData.BanPlayer(Server.Players[playerid].Username, info.address.GetIP(),playerid, _mins);
                    return "Ban rider Applied";
                }
                else
                {
                    return "No player found";
                }

            }
            else
            {
                return "Bad id";
            }
        }

        [HttpGet("/Garagesetup")]
        public string Garagesetup([FromHeader] string riderid)
        {
            if(uint.TryParse((string)riderid, out uint playerid))
            {
                if (Server.Players.ContainsKey(playerid))
                {
                    return Server.Players[playerid].Gear.garagexml;
                }
            }

            return "failed to find garage data for id: " + riderid;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
