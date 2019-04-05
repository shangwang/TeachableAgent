using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nico.csharp.functions;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.AspNet.SignalR;

namespace Nico.Hubs
{    
    [Authorize]
    public class MyHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections =
           new ConnectionMapping<string>();

        private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();

        public static void Start(string userId, string responseTxt)
        {
                               

           //string name = Context.User.Identity.Name;
            /*
            if (hubContext != null)
            {
                //hubContext.Clients.Group(userId).playSpeech();
                hubContext.Clients.All.playSpeech(agentAudioPath);
            }
            */
            if (hubContext != null)
            { 
                foreach (var connectionId in _connections.GetConnections(userId))
                {
                    hubContext.Clients.Client(connectionId).playSpeech(responseTxt);
                }
            }
        }


        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;

            _connections.Add(name, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            string name = Context.User.Identity.Name;

            _connections.Remove(name, Context.ConnectionId);

            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            string name = Context.User.Identity.Name;

            if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
            {
                _connections.Add(name, Context.ConnectionId);
            }

            return base.OnReconnected();
        }





    }
}