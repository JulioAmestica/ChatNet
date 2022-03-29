using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth0.AspNetCore.Authentication;
using System.Configuration;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ChatNet
{
    public class ChatNetHub : Hub
    {

        private readonly static string urlBot = "http://localhost:38567/stock";
        private readonly static string MESSAGERRORCOMMAND = "Command format incorrect, verify and fix. Ex:/stock=aapl.us";
        private readonly static string MESSAGECODENOTFOUND = "Stock code don't found or don't have availabe data";
        private readonly static string PROCESSINGERROR = "My Apologies I can't complete your command. Try again."; 

        /// <summary>
        /// Send message to the room
        /// </summary>
        /// <param name="room"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string room, string user, string message)
        {
            if (!message.StartsWith("/"))
            {
                await Clients.Group(room).SendAsync("ReceiveMessage", user, message);
            }
            else
            {
                string answerBot = GetData(message);
                await Clients.Group(room).SendAsync("ReceiveMessage", "STOCKBOT", answerBot);
            }
        }

        /// <summary>
        /// Add a client to group and comunitate it to others 
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task AddToGroup(string room)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, room);
            await Clients.Groups(room).SendAsync("ShowNew", $"Welcome {Context.User.Identity.Name}");
        }

        private string GetData(string message)
        {
            try
            {
                string stockCode = string.Empty;
                try
                {
                    stockCode = message.Split('=')[1];
                }
                catch
                {
                    return MESSAGERRORCOMMAND;
                }
                HttpClient http = new HttpClient();
                string url = $"{urlBot}?stock_code={stockCode}";
                string data = http.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
                if (data.Equals("Error"))
                {
                    return MESSAGECODENOTFOUND;
                }
                else
                {
                    return $"{stockCode} quote is ${data} per share";
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
                return PROCESSINGERROR;
            }
        }
    }
}
