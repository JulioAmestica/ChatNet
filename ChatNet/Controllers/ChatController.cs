using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Auth0.AspNetCore.Authentication;

namespace ChatNet.Controllers
{
    public class ChatController : Controller
    {
        public static Dictionary<int, string> Rooms =
            new Dictionary<int, string>()
            {
                {1,"Jobs"},
                {2,"Projects"},
                {3,"Ideas"}
            };

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Room(int room)
        {
            Rooms.TryGetValue(room, out string theRoom);
            ViewData["theRoom"]=theRoom;
            return View("Room", room);
        }
    }
}
