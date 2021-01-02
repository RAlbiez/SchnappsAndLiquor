using WebSocketSharp;
using WebSocketSharp.Server;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SchnappsAndLiquor.Server
{
    public class MasterServer
    {
        private HttpServer oHttpServer = null;
        private Dictionary<string, Game.Game> oGames = new Dictionary<string, Game.Game>();
        private Dictionary<string, List<ClientConnection>> oConnections = new Dictionary<string, List<ClientConnection>>();
        private Dictionary<string, string> oLobbyLeader = new Dictionary<string, string>();
        private static Random random = new Random();

        public MasterServer()
        {
            var port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            var secure = bool.Parse(ConfigurationManager.AppSettings["Secure"]);
            this.oHttpServer = new HttpServer(IPAddress.Any, port, secure);
            this.oHttpServer.Log.Level = LogLevel.Info;
            this.oHttpServer.DocumentRootPath = ConfigurationManager.AppSettings["DocumentRootPath"];
            this.oHttpServer.OnGet += ServerStaticContent;
            this.oHttpServer.AddWebSocketService("/", (ClientConnection c) => c.oMasterServer = this);

            this.oHttpServer.Start();
            if (!this.oHttpServer.IsListening)
            {
                this.oHttpServer.Log.Fatal("Masterserver failed to start...");
                return;
            }
            this.oHttpServer.Log.Info("Started Masterserver on port " + port);
            this.oHttpServer.Log.Info("Press Enter key to stop the server...");
            Console.ReadLine();
        }

        public string CreateGame(ClientConnection connection)
        {
            while (true)
            {
                var id = GenerateLobbyId();
                if (this.oGames.ContainsKey(id)) { continue; }
                var game = new Game.Game();
                game.sGameId = id;
                this.oGames.Add(id, game);
                this.oConnections.Add(id, new List<ClientConnection>());
                this.oLobbyLeader.Add(id, connection.sName);
                this.oHttpServer.Log.Info("Created new game session " + id + " \t (" + this.oGames.Count + " Total)");
                return id;
            }
        }

        public Game.Game JoinGame(string id, ClientConnection connection)
        {
            if (!this.oGames.ContainsKey(id)) { return null; }

            foreach (var i in this.oConnections[id])
            {
                if (i.sName == connection.sName)
                {
                    // Don't two connections with the same name
                    return null;
                }
            }
            this.oConnections[id].Add(connection);
            var game = this.oGames[id];
            foreach (var i in game.oPlayers)
            {
                if (i.sName == connection.sName)
                {
                    // Player was already in the game, no need to add one
                    return game;
                }
            }
            game.AddPlayer(connection.sName);
            return game;
        }

        public bool KickPlayer(string id, string name, ClientConnection connetion)
        {
            if (connetion.sName != this.oLobbyLeader[id]) { return false; }
            foreach (var i in this.oConnections[id])
            {
                if (i.sName == name)
                {
                    i.CloseConnection();
                    this.oGames[id].RemovePlayer(name);
                }
            }
            return true;
        }

        public void PushGameState(string id)
        {
            if (!this.oGames.ContainsKey(id)) { return; }
            var options = new JsonSerializerOptions { IncludeFields = true };
            var state = JsonSerializer.Serialize(this.oGames[id], options);
            foreach (var c in this.oConnections[id])
            {
                c.SendData(state);
            }
        }

        public void OnClientDisconnect(ClientConnection connection, string id)
        {
            foreach (var j in oConnections[id])
            {
                if (j == connection)
                {
                    oConnections[id].Remove(connection);
                    if (oConnections[id].Count == 0)
                    {
                        oConnections.Remove(id);
                        oGames.Remove(id);
                        oLobbyLeader.Remove(id);
                        this.oHttpServer.Log.Info("Closed game session " + id + " \t (" + this.oGames.Count + " Total)");
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Handle normal https requests to static content in the public folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerStaticContent(object sender, HttpRequestEventArgs e)
        {
            var req = e.Request;
            var res = e.Response;
            var path = req.RawUrl;
            if (path == "/") { path += "index.html"; }

            byte[] contents;
            if (!e.TryReadFile(path, out contents))
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                this.oHttpServer.Log.Info("Static file not found " + path);
                return;
            }

            if (path.EndsWith(".html"))
            {
                res.ContentType = "text/html";
                res.ContentEncoding = Encoding.UTF8;
            }
            else if (path.EndsWith(".js"))
            {
                res.ContentType = "application/javascript";
                res.ContentEncoding = Encoding.UTF8;
            }

            res.ContentLength64 = contents.LongLength;
            res.Close(contents, true);
        }

        private static string GenerateLobbyId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        ~MasterServer()
        {
            this.oHttpServer.Log.Info("Shutting down");
            try { this.oHttpServer.Stop(); }
            catch (System.Exception) { }
        }

    }
}
