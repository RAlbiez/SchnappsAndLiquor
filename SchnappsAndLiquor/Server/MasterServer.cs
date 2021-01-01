using WebSocketSharp;
using WebSocketSharp.Server;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SchnappsAndLiquor.Server
{
    public class MasterServer
    {
        private HttpServer oHttpServer = null;
        private Dictionary<string, Game.Game> oGames = new Dictionary<string, Game.Game>();
        private Dictionary<string, List<ClientConnection>> oConnections = new Dictionary<string, List<ClientConnection>>();
        private static Random random = new Random();

        public MasterServer()
        {
            var port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            var secure = bool.Parse(ConfigurationManager.AppSettings["Secure"]);
            this.oHttpServer = new HttpServer(IPAddress.Any, port, secure);
            this.oHttpServer.Log.Level = LogLevel.Info;
            this.oHttpServer.DocumentRootPath = ConfigurationManager.AppSettings["DocumentRootPath"];
            this.oHttpServer.OnGet += ServerStaticContent;
            this.oHttpServer.AddWebSocketService("/", (ClientConnection c) => c.SetMasterServer(this));

            this.oHttpServer.Start();
            if (this.oHttpServer.IsListening)
            {
                this.oHttpServer.Log.Info("Started Masterserver on port " + port);
                foreach (var path in this.oHttpServer.WebSocketServices.Paths)
                {
                    this.oHttpServer.Log.Info("- " + path);
                }
                this.oHttpServer.Log.Info("Press Enter key to stop the server...");
                Console.ReadLine();
            }
            else
            {
                this.oHttpServer.Log.Fatal("Masterserver failed to start...");
            }
        }

        public string CreateGame(string playerName, ClientConnection connection)
        {
            while (true)
            {
                var id = GenerateLobbyId();
                if (this.oGames.ContainsKey(id)) { continue; }
                var game = new Game.Game();
                game.sGameId = id;
                this.oGames.Add(id, game);
                this.oConnections.Add(id, new List<ClientConnection>());
                return id;
            }
        }

        public Game.Game JoinGame(string id, string name, ClientConnection connection)
        {
            if (this.oGames.ContainsKey(id))
            {
                this.oConnections[id].Add(connection);
                return this.oGames[id];
            }
            return null;
        }

        public void PushGameState(string id)
        {
            var state = JsonConvert.SerializeObject(this.oGames[id]);
            foreach (var c in this.oConnections[id])
            {
                c.SendData(state);
            }
        }

        public void OnClientDisconnect(ClientConnection connection, string gameId)
        {
            foreach (var j in oConnections[gameId])
            {
                if (j == connection)
                {
                    oConnections[gameId].Remove(connection);
                    if (oConnections[gameId].Count == 0)
                    {
                        oConnections.Remove(gameId);
                        oGames.Remove(gameId);
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
