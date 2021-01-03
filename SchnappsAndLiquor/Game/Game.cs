using SchnappsAndLiquor.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SchnappsAndLiquor.Game
{
    public class Game
    {
        public Board oBoard = new Board();
        public Dictionary<string, Player> oPlayers = new Dictionary<string, Player>();
        public PlayerOrder oPlayerOrder = new PlayerOrder();
        public List<SnakeOrLadder> oSnakesAndLadders;
        public string sGameId = "";
        public string sLobbyLeader = "";
        public int intMaxFields = GameParams.MAX_FIELDS;
        public int intWidth = GameParams.WIDTH;
        public int intHeight = GameParams.HEIGHT;
        public Message oCurrentMessage;
        private Queue<Message> oMessageQueue = new Queue<Message>();
        private List<string> oColors = new List<string>();

        public Game()
        {
            this.InitBoard();
            this.oColors.Add("#000000");
            this.oColors.Add("#0000FF");
            this.oColors.Add("#00FF00");
            this.oColors.Add("#00FFFF");
            this.oColors.Add("#FF0000");
            this.oColors.Add("#FF00FF");
            this.oColors.Add("#FFFF00");
            this.oColors.Add("#FFFFFF");
            oSnakesAndLadders = SnakeAndLadderService.GenerateSnakesAndLadders(this);         
        }

        public Player GetCurentPlayer()
        {
            return oPlayers[oPlayerOrder.GetCurrent()];
        }

        public void WinGame(string sPlayerName)
        {

        }

        protected void ActivateField(string sPlayerName, short shtFieldNumber)
        {
            var oReturn = oBoard[shtFieldNumber].FieldAction(sPlayerName, this);

            if(oBoard[shtFieldNumber].bIsStartPoint)
            {
                //Messages for snakes and ladders
            }


            if(oReturn.oChoice != null)
            {
                Message oMessage = new Message("FieldAction", sPlayerName, oReturn.oChoice, oReturn.Callback);

                if (oReturn.oChoice.bCanSkip)
                {
                    oMessageQueue.Enqueue(new Message("SkipField", sPlayerName, 
                        new Choice(new List<string> { "Ja", "Nein" }, sPlayerName), DequeueMessage, oMessage.sMessageID));
                }

                oMessageQueue.Enqueue(oMessage);
            }
        }

        protected void DequeueMessage(Game oGame, string sAnswer)
        {
            Queue<Message> oTempQueue = new Queue<Message>();

            Message oMessage = oGame.oMessageQueue.Dequeue();

            if (oMessage.sMessageID != sAnswer)
                oTempQueue.Enqueue(oMessage);

            oGame.oMessageQueue = oTempQueue;
        }

        public void MovePlayerBy(string sPlayerName, short shtNumOfFields)
        {
            Player oPlayer = this.oPlayers[sPlayerName];
            short shtNewPos = oPlayer.MoveBy(shtNumOfFields);
            this.ActivateField(sPlayerName, shtNewPos);
        }

        public void AddPointsToPlayer(string sPlayerName, short shtPointsToAdd)
        {
            this.oPlayers[sPlayerName].AddPoints(shtPointsToAdd);
        }

        protected void InitBoard()
        {
            FieldService oFieldService = new FieldService();

            this.oBoard[0] = new StartField();

            for( short i = 1; i < GameParams.MAX_FIELDS - 1; i++)
            {
                IField oField = oFieldService.Next(this, i);

                this.oBoard[i] = oField;
            }

            this.oBoard[(short)(GameParams.MAX_FIELDS - 1)] = new FinishField();
        }

        public void AddPlayer(string sNameP)
        {
            var sColor = this.oColors[GameParams.oRandomInstance.Next(0, this.oColors.Count)];
            this.oColors.Remove(sColor);
            this.oPlayers.Add(sNameP, new Player(sNameP, sColor));
            this.oPlayerOrder.AddPlayer(sNameP);

            if(this.oPlayers.Count == 1)
                oCurrentMessage = new Message("MoveFields", sNameP);
        }
        
        public void RemovePlayer(string sNameP)
        {
            this.oColors.Add(this.oPlayers[sNameP].sColor);
            this.oPlayers.Remove(sNameP);
        }

        /// <summary>
        /// Handle client actions
        /// </summary>
        /// <param name="action"></param>
        /// <returns>Returns true if the game state was altered in order to push it to every client</returns>
        public bool HandleClientAction(ClientAction action, string sNameP)
        {
            if(action.GetFirst("messageid") != oCurrentMessage.sMessageID || sNameP != oCurrentMessage.sPlayerName)
            {
                return false;
            }

            bool bReturn = true;  

            switch (oCurrentMessage.sMessageType)
            {
                case "MoveFields":
                    if(short.TryParse(action.GetFirst("answer"), out short shtNumberRolled))
                        this.MovePlayerBy(oCurrentMessage.sPlayerName, shtNumberRolled);
                    break;
                case "SkipField":
                    if(action.GetFirst("answer") == "Ja")
                        oCurrentMessage.oCallback(this, oCurrentMessage.sSpecialField);
                    break;
                case "FieldAction":
                    oCurrentMessage.oCallback(this, action.GetFirst("answer"));
                    break;
                default:
                    bReturn = false;
                    break;
            }

            if (bReturn)
            {
                oCurrentMessage = oMessageQueue.Count == 0 ? new Message("MoveFields", oPlayerOrder.Next()) : oMessageQueue.Dequeue();
            }

            return bReturn;
        }
    }

    public static class GameParams
    {
        public const int MAX_FIELDS = 64;

        public const int WIDTH = 8;
        public const int HEIGHT = 8;

        public static Random oRandomInstance = new Random();
    }
}
