using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeServices.Hubs
{
    public class ChatHub: Hub
    {
        public class Data
        {
            public string message { get; set; }
            public string user { get; set; } = "A socket";

            //uitbreiding 
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
