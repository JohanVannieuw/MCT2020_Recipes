using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeServices.Hubs
{
    public class ChatHub : Hub
    {
        public class Data
        {
            public string message { get; set; }
            public string user { get; set; } = "A socket";

            //uitbreiding 
            public string color { get; set; }

        }

        public override async Task OnConnectedAsync()
        {
            //TODO: try catch op de hubs
            var data = new Data { message = "" };
            await Clients.Caller.SendAsync("Login", data);
            await base.OnConnectedAsync();
        }

        public async Task Login(Data data)
        {
            data.color = String.Format("#{0:X6}", new Random().Next(0x1000000));

            await Clients.Caller.SendAsync("ServerMessage",
                  new { username = data.user, message = $"Welcome {data.user}!", color = data.color });
            await Clients.Others.SendAsync("ServerMessage",
                  new { username = data.user, message = $"{data.user} just logged in.", color = data.color });
        }
        public async Task ClientMessage(Data data)
        {
            try
            {
                await Clients.All.SendAsync("ServerMessage", data); //object
            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message);
            }
        }






    }
}
