using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using SchnappsAndLiquor.Game;
using Newtonsoft.Json;

namespace SchnappsAndLiquor.Net
{
    class Connection
    {
        List<Socket> oAllConnections = new List<Socket>();       

        public void SendGameToEveryone(Game.Game oGame)
        {
            foreach(Socket oSocket in oAllConnections)
            {
                oSocket.Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(oGame)));
            }
        }


    }
}
