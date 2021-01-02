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
        private Game.Game oCurrentGame = null;
        public MasterServer oMasterServer { get; set; }
        public string sName { get; private set; }

        public void SendData(string data) { Send(data); }

        protected override void OnClose(CloseEventArgs e)
        {
            if (this.oCurrentGame == null) { return; }
            this.oMasterServer.OnClientDisconnect(this, this.oCurrentGame.sGameId);
            this.oMasterServer.PushGameState(this.oCurrentGame.sGameId);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var action = JsonConvert.DeserializeObject<ClientAction>(e.Data);
            if (action.Type == "ClientCreateLobby")
            {
                this.sName = action.GetFirst("name");
                var id = this.oMasterServer.CreateGame(this);
                this.oCurrentGame = this.oMasterServer.JoinGame(id, this);
            }
            else if (action.Type == "ClientJoinGame")
            {
                this.sName = action.GetFirst("name");
                var id = action.GetFirst("lobbyId");
                this.oCurrentGame = this.oMasterServer.JoinGame(id, this);
            }
            else
            {
                if (this.oCurrentGame != null)
                {
                    if (!this.oCurrentGame.HandleClientAction(action))
                    {
                        return;
                    }
                }
            }

            if (this.oCurrentGame != null)
            {
                this.oMasterServer.PushGameState(this.oCurrentGame.sGameId);
            }
        }
    }
}
