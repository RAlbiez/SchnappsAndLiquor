using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using SchnappsAndLiquor.Game;
using Newtonsoft.Json;

namespace SchnappsAndLiquor.Net
{
    public class Connection
    {
        Dictionary<Guid, Socket> oAllConnections = new Dictionary<Guid, Socket>();       

        public void SendGameToEveryone(Game.Game oGame)
        {
            string sGameData = JsonConvert.SerializeObject(oGame);
            byte[] oSendData = Encoding.UTF8.GetBytes(sGameData);
            foreach (Socket oSocket in oAllConnections.Values)
            {
                SocketAsyncEventArgs oSocketArgs = new SocketAsyncEventArgs();
                oSocketArgs.SetBuffer(oSendData);
                oSocket.SendAsync(oSocketArgs);
            }
        }

        public void AcceptConnections()
        {

        }
    }
}
