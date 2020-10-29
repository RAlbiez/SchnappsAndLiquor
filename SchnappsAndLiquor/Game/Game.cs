using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SchnappsAndLiquor.Game
{
    public class Game
    {
        private Board oBoard = new Board();
        private PlayerList oPlayers = new PlayerList();
        private short shtCurrentPlayer = 0;

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
            this.oBoard[shtFieldNumber].Action(gPlayerID, this);
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
    }

    public static class GameParams
    {
        public const int MAX_FIELDS = 64;

        public const int DIM_1 = 8;
        public const int DIM_2 = 8;

        public static Random oRandomInstance = new Random();
    }
}
