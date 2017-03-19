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
        public static ConcurrentDictionary<string, List<string>> mapRunIDToConnection = new ConcurrentDictionary<string, List<string>>();

        public void Register(string facebookId)
        {
            mapUidToConnection[facebookId] = Context.ConnectionId;
            Trace.TraceInformation(String.Format("Added user: {0} connectionId {1}", facebookId, mapUidToConnection[facebookId]));
            //TODO DELETE THIS LINE ^^
        }

        public void SendLocation(LocationMessage msg)
        {
            if (mapUidToConnection.ContainsKey(msg.DestUserId))
            {
                Trace.TraceInformation(String.Format("Sending to user: {0} connectionId {1}", msg.DestUserId, mapUidToConnection[msg.DestUserId]));

                try
                {
                    Clients.Client(mapUidToConnection[msg.DestUserId]).buddyLocationUpdate(msg);
                }
                catch (Exception e)
                {
                    string deadConnectionId;
                    mapUidToConnection.TryRemove(msg.DestUserId, out deadConnectionId);
                }
            }
            else
            {
                Trace.TraceInformation(String.Format("User {0} doesn't exist", msg.DestUserId));
            }
        }

        public void HandShake(String runId, String userId)
        {
            if (mapRunIDToConnection.ContainsKey(runId))
            {
                mapRunIDToConnection[runId].Add(userId);
                foreach (string user in mapRunIDToConnection[runId])
                {
                    try
                    {
                        Clients.Client(mapUidToConnection[user]).runStart("start");
                    }
                    catch (Exception e)
                    {
                        string deadConnectionId;
                        mapUidToConnection.TryRemove(user, out deadConnectionId);
                    }
                }
            }
            else
            {
                mapRunIDToConnection[runId] = new List<string>();
                mapRunIDToConnection[runId].Add(userId);
            }
        }

    }

}