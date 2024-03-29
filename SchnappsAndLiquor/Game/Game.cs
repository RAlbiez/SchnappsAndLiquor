﻿using SchnappsAndLiquor.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SchnappsAndLiquor.Game
{
    public class Game
    {
        private int intRecursionCounter = 0;
        public int intTurnNumber = 0;
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
        public List<(string sTurnPlayer, string sConnectedPlayer, int intTurnCounter)> oPlayerConnections = new List<(string sTurnPlayer, string sConnectedPlayer, int intTurnCounter)>();
        private Queue<Message> oMessageQueue = new Queue<Message>();
        private List<string> oColors = new List<string>();
        public List<(string, int)> oDrinksThisTurn = new List<(string, int)>();

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
            this.oColors.Add("#AAAAAA");
            this.oColors.Add("#990033");
            this.oColors.Add("#4DD0E1");
            this.oColors.Add("#654416");
            this.oColors.Add("#0055AA");
            this.oColors.Add("#DD5500");
            oSnakesAndLadders = SnakeAndLadderService.GenerateSnakesAndLadders(this);         
        }

        public Player GetCurentPlayer()
        {
            return oPlayers[oPlayerOrder.GetCurrent()];
        }

        public void WinGame(string sPlayerName)
        {
            oMessageQueue.Enqueue(new Message("GameWon", sPlayerName));
        }

        protected void ActivateField(string sPlayerName, short shtFieldNumber)
        {
            oDrinksThisTurn = new List<(string, int)>();

            intRecursionCounter++;

            if (intRecursionCounter > 5)
            {
                foreach(Player oPlayer in oPlayers.Values)
                {
                    oPlayer.AddPoints(5);
                }

                oMessageQueue.Enqueue(new Message("StackOverflowExceptionAchieved", sPlayerName));
                return;
            }

            var oReturn = oBoard[shtFieldNumber].FieldAction(sPlayerName, this);

            Message oMessageFieldAction = null;
            Message oMessageSkipField = null;

            if (oReturn.oChoice != null)
            {
                oMessageFieldAction = new Message("FieldAction", sPlayerName, oReturn.oChoice, oReturn.Callback);

                if (oReturn.oChoice.bCanSkip)
                {
                    if (oPlayers[sPlayerName].lngPoints >= shtFieldNumber)
                    {
                        oMessageSkipField = new Message("SkipField", sPlayerName,
                        new Choice(new List<string> { "Ja", "Nein" }, sPlayerName), DequeueMessage, new List<string>() { oMessageFieldAction.sMessageID });
                    }
                    else
                    {
                        oMessageSkipField = new Message("NoSkipField", sPlayerName);
                    }
                }
            }
            else
            {
                oMessageFieldAction = new Message("NoFieldAction", sPlayerName, null, oReturn.Callback);
            }

            if (oBoard[shtFieldNumber].bIsStartPoint)
            {
                SnakeOrLadder oSnakeOrLadder = oSnakesAndLadders.First(x => x.shtStartPoint == shtFieldNumber);
                if ((oPlayers[sPlayerName].lngPoints >= shtFieldNumber && oSnakeOrLadder.bSnake) 
                    || (oPlayers[sPlayerName].lngPoints < shtFieldNumber && !oSnakeOrLadder.bSnake))
                {
                    oMessageQueue.Enqueue(new Message("NoSnakeOrLadder", sPlayerName));
                }
                else
                {
                    oMessageQueue.Enqueue(new Message("SnakeOrLadder", sPlayerName));
                    MovePlayerBy(sPlayerName, (short)(oSnakeOrLadder.shtEndPoint - shtFieldNumber));
                    return;
                }
            }

            if(oMessageSkipField != null)
                oMessageQueue.Enqueue(oMessageSkipField);
            if(oMessageFieldAction != null)
                oMessageQueue.Enqueue(oMessageFieldAction);
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

            if (oPlayer.shtBoardPosition + shtNumOfFields > GameParams.MAX_FIELDS)
            {
                oMessageQueue.Enqueue(new Message("MissedEnd", sPlayerName));
                this.ActivateField(sPlayerName, oPlayer.shtBoardPosition);
                return;
            }

            short shtNewPos = oPlayer.MoveBy(shtNumOfFields);
            this.ActivateField(sPlayerName, shtNewPos);
        }

        public void AddPointsToPlayer(string sPlayerName, short shtPointsToAdd)
        {
            foreach (var oPlayerConnection in oPlayerConnections.Where(x => x.sTurnPlayer == sPlayerName))
            {
                if(oPlayerConnection.sConnectedPlayer == "☭☭☭")
                {
                    foreach (var oPlayer in oPlayers)
                    {
                        oPlayer.Value.AddPoints(1);
                        this.oDrinksThisTurn.Add((oPlayer.Value.sName, 1));
                    }
                    return;
                }
                oPlayers[oPlayerConnection.sConnectedPlayer].AddPoints(shtPointsToAdd);
                this.oDrinksThisTurn.Add((oPlayers[oPlayerConnection.sConnectedPlayer].sName, shtPointsToAdd));
            }

            oPlayers[sPlayerName].AddPoints(shtPointsToAdd);
            this.oDrinksThisTurn.Add((oPlayers[sPlayerName].sName, shtPointsToAdd));

        }

        public void InitBoard()
        {
            FieldService oFieldService = new FieldService();

            this.oBoard[0] = new StartField();

            int intCountOfRerollFields = 0;

            for( short i = 1; i < GameParams.MAX_FIELDS - 1; i++)
            {
                IField oField = oFieldService.Next(this, i);

                if(oField.GetType() == typeof(RerollBoardField))
                {
                    if (intCountOfRerollFields > 0)
                    {
                        i--;
                        continue;
                    }
                    else
                    {
                        intCountOfRerollFields++;
                    }
                }    

                this.oBoard[i] = oField;
            }

            this.oBoard[(short)(GameParams.MAX_FIELDS - 1)] = new FinishField();
        }

        public void AddPlayer(string sNameP)
        {
            string sColor = String.Format("#{0:X6}", GameParams.oRandomInstance.Next(0x1000000));
            if (this.oColors.Count != 0)
            {
                sColor = this.oColors[GameParams.oRandomInstance.Next(0, this.oColors.Count)];
            }
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
            this.oPlayerOrder.RemovePlayer(sNameP);
        }

        private void RemoveConnectedPlayers()
        {
            oPlayerConnections.RemoveAll(x => x.intTurnCounter == intTurnNumber);
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
                    if (short.TryParse(action.GetFirst("answer"), out short shtNumberRolled))
                    {
                        intRecursionCounter = 0;
                        intTurnNumber++;

                        RemoveConnectedPlayers();

                        this.MovePlayerBy(oCurrentMessage.sPlayerName, shtNumberRolled);
                    }
                    break;
                case "SkipField":
                    if(action.GetFirst("answer") == "Ja")
                        foreach(string sMessageToDequeue in oCurrentMessage.oAdditionalFields)
                            oCurrentMessage.oCallback(this, sMessageToDequeue);
                    break;
                case "FieldAction":
                    oCurrentMessage.oCallback(this, action.GetFirst("answer"));
                    break;
                case "NoFieldAction":
                    oCurrentMessage.oCallback(this, "");
                    break;
                case "StackOverflowExceptionAchieved":
                case "MissedEnd":
                case "NoSkipField":
                case "WinGame":
                case "NoSnakeOrLadder":
                case "SnakeOrLadder":
                    bReturn = true;
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
