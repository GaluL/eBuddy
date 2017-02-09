using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using eBuddyService.DataObjects;
using Microsoft.AspNet.SignalR;

namespace eBuddyService.Hubs
{
    public class SocialRunsHub : Hub
    {
        public static ConcurrentDictionary<string, string> mapUidToConnection = new ConcurrentDictionary<string, string>();

        public void Register(string facebookId)
        {
            mapUidToConnection[facebookId] = Context.ConnectionId;
            Trace.TraceInformation(String.Format("Added user: {0} connectionId {1}", facebookId, mapUidToConnection[facebookId]));
        }

        public void SendLocation(LocationMessage msg)
        {
            if (mapUidToConnection.ContainsKey(msg.DestUserId))
            {
                Trace.TraceInformation(String.Format("Sending to user: {0} connectionId {1}", msg.DestUserId, mapUidToConnection[msg.DestUserId]));

                Clients.Client(mapUidToConnection[msg.DestUserId]).buddyLocationUpdate(msg);
            }
            else
            {
                Trace.TraceInformation(String.Format("User {0} doesn't exist", msg.DestUserId));
            }
        }
    }
}