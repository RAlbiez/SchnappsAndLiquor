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
        private Message oCurrentMessage;
        private Queue<Message> oMessageQueue = new Queue<Message>();

        public Game()
        {
            this.InitBoard();

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
                Message oMessage = new Message("fieldaction", sPlayerName, oReturn.oChoice, oReturn.Callback);

                if (oReturn.oChoice.bCanSkip)
                {
                    oMessageQueue.Enqueue(new Message("skip", sPlayerName, 
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
            this.oPlayers.Add(sNameP, new Player(sNameP));
            this.oPlayerOrder.AddPlayer(sNameP);

            if(this.oPlayers.Count == 1)
                oCurrentMessage = new Message("roll", sNameP);
        }
        
        public void RemovePlayer(string sNameP)
        {
            this.oPlayers.Remove(sNameP);
        }

        /// <summary>
        /// Handle client actions
        /// </summary>
        /// <param name="action"></param>
        /// <returns>Returns true if the game state was altered in order to push it to every client</returns>
        public bool HandleClientAction(ClientAction action)
        {
            if(action.GetFirst("messageid") != oCurrentMessage.sMessageID || action.GetFirst("name") != oCurrentMessage.sPlayerName)
            {
                return false;
            }

            bool bReturn = true;  

            switch (oCurrentMessage.sMessageType)
            {
                case "roll":
                    if(short.TryParse(action.GetFirst("answer"), out short shtNumberRolled))
                        this.MovePlayerBy(oCurrentMessage.sPlayerName, shtNumberRolled);
                    break;
                case "skip":
                    if(action.GetFirst("answer") == "Ja")
                        oCurrentMessage.oCallback(this, oCurrentMessage.sSpecialField);
                    break;
                case "fieldaction":
                    oCurrentMessage.oCallback(this, action.GetFirst("answer"));
                    break;
                default:
                    bReturn = false;
                    break;
            }

            if (bReturn)
            {
                oCurrentMessage = oMessageQueue.Count == 0 ? new Message("roll", oPlayerOrder.Next()) : oMessageQueue.Dequeue();
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
