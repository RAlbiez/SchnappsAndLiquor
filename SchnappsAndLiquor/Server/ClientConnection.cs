using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;
using SchnappsAndLiquor.Net;
using System.Text.Json;

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

        public void CloseConnection() { Close(); }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                var action = JsonSerializer.Deserialize<ClientAction>(e.Data);
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
                else if (action.Type == "ClientKick")
                {
                    var sName = action.GetFirst("name");
                    this.oMasterServer.KickPlayer(this.oCurrentGame.sGameId, sName, this);
                    return;
                }
                else
                {
                    if (this.oCurrentGame != null)
                    {
                        if (!this.oCurrentGame.HandleClientAction(action, this.sName))
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
            catch (Exception exeption)
            {

                this.Log.Error(exeption.ToString());
            }

        }
    }
}
