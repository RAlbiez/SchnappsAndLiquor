﻿using SchnappsAndLiquor.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SchnappsAndLiquor.Game
{
    public class Game
    {
        public Board oBoard = new Board();
        public PlayerList oPlayers = new PlayerList();
        public short shtCurrentPlayer = 0;
        public string sGameId = "";

        public Game()
        {
            this.InitBoard();
        }

        public Player GetCurentPlayer()
        {
            return this.oPlayers[shtCurrentPlayer];
        }

        public void WinGame(Guid gPlayerId)
        {

        }

        protected void ActivateField(Guid gPlayerID, short shtFieldNumber)
        {
            var oReturn = this.oBoard[shtFieldNumber].FieldAction(gPlayerID, this);

            if(oReturn.oChoice != null)
            {

            }
        }

        public void MovePlayerBy(short shtPlayerNumber, short shtNumOfFields)
        {
            Player oPlayer = this.oPlayers[shtPlayerNumber];
            short shtNewPos = oPlayer.MoveBy(shtNumOfFields);
            this.ActivateField(oPlayer.gPlayerID, shtNewPos);
        }

        public void MovePlayerBy(Guid gPlayerId, short shtNumOfFields)
        {
            Player oPlayer = this.oPlayers.GetByID(gPlayerId);
            short shtNewPos = oPlayer.MoveBy(shtNumOfFields);
            this.ActivateField(oPlayer.gPlayerID, shtNewPos);
        }

        public void MovePlayerBy(string sPlayerName, short shtNumOfFields)
        {
            Player oPlayer = this.oPlayers.GetByName(sPlayerName);
            short shtNewPos = oPlayer.MoveBy(shtNumOfFields);
            this.ActivateField(oPlayer.gPlayerID, shtNewPos);
        }

        public void AddPointsToPlayer(short shtPlayerNumber, short shtPointsToAdd)
        {
            this.oPlayers[shtPlayerNumber].AddPoints(shtPointsToAdd);
        }

        public void AddPointsToPlayer(Guid gPlayerId, short shtPointsToAdd)
        {
            this.oPlayers.GetByID(gPlayerId).AddPoints(shtPointsToAdd);
        }

        public void AddPointsToPlayer(string sPlayerName, short shtPointsToAdd)
        {
            this.oPlayers.GetByName(sPlayerName).AddPoints(shtPointsToAdd);
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
            this.oPlayers.Add(new Player(sNameP));
        }

        public void HandleClientAction(ClientAction action)
        {
            // tu die dinge
        }
    }

    public static class GameParams
    {
        public const int MAX_FIELDS = 64;

        public const int DIM_1 = 8;
        public const int DIM_2 = 8;

        public static Random oRandomInstance = new Random();
    }
}
