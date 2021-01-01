using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using SchnappsAndLiquor.Net;

namespace SchnappsAndLiquor.Server
{
    public class ClientConnection : WebSocketBehavior
    {
        private MasterServer oMasterServer = null;
        private Game.Game oCurrentGame = null;
        private string sName = null;

        public void SetMasterServer(MasterServer master)
        {
            this.oMasterServer = master;
        }

        public void SendData(string data)
        {
            Send(data);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            if (this.oCurrentGame != null)
            {
                this.oMasterServer.OnClientDisconnect(this, this.oCurrentGame.sGameId);
                this.oMasterServer.PushGameState(this.oCurrentGame.sGameId);
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var action = JsonConvert.DeserializeObject<ClientAction>(e.Data);
            if (action.Type == "ClientCreateLobby")
            {
                this.sName = action.GetFirst("name");
                var id = this.oMasterServer.CreateGame(this.sName, this);
                this.oCurrentGame = this.oMasterServer.JoinGame(id, this.sName, this);
            }
            else if (action.Type == "ClientJoinGame")
            {
                var id = action.GetFirst("lobbyId");
                this.sName = action.GetFirst("name");
                this.oCurrentGame = this.oMasterServer.JoinGame(id, this.sName, this);
            }
            else
            {
                if (this.oCurrentGame != null)
                {
                    this.oCurrentGame.HandleClientAction(action);
                }
            }

            if (this.oCurrentGame != null)
            {
                this.oMasterServer.PushGameState(this.oCurrentGame.sGameId);
            }
            
        }

        protected override void OnOpen()
        {
        }
    }
}
