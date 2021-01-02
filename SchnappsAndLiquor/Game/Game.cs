using SchnappsAndLiquor.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SchnappsAndLiquor.Game
{
    enum State
    {
        Roll,
        FieldAction,
        Skip
    };

    public class Game
    {
        public Board oBoard = new Board();
        public PlayerList oPlayers = new PlayerList();
        public List<SnakeOrLadder> oSnakesAndLadders;
        public short shtCurrentPlayer = 0;
        public string sGameId = "";
        public string sLobbyLeader = "";
        private State oGameState;
        private short shtCurrentField;
        private Action<Game, Answer> oCurrentCallback;

        public Game()
        {
            this.InitBoard();

            oSnakesAndLadders = SnakeAndLadderService.GenerateSnakesAndLadders(this);

            oGameState = State.Roll;
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

            oCurrentCallback = oReturn.Callback;

            if(oReturn.oChoice != null && oReturn.oChoice.bCanSkip )
            {
                oGameState = State.Skip;
            }

            if(oReturn.oChoice != null)
            {
                oGameState = State.FieldAction;
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
        
        public void RemovePlayer(string sNameP)
        {
            this.oPlayers.Remove(this.oPlayers.GetByName(sNameP));
        }

        /// <summary>
        /// Handle client actions
        /// </summary>
        /// <param name="action"></param>
        /// <returns>Returns true if the game state was altered in order to push it to every client</returns>
        public bool HandleClientAction(ClientAction action)
        {
            if (oPlayers.GetByName(action.GetFirst("name")).gPlayerID != this.GetCurentPlayer().gPlayerID)
            {
                return false;
            }

            State oPlayerState = State.Roll;

            if (oPlayerState != oGameState)
                return false;

            switch (oPlayerState)
            {
                case State.Roll:
                    if(short.TryParse(action.GetFirst("nicerdicer"), out short shtNumberRolled))
                    {
                        this.MovePlayerBy(this.GetCurentPlayer().gPlayerID, shtNumberRolled);
                    }
                    return true;
                case State.Skip:
                    oCurrentCallback(this, new Answer() { bSkipped = action.GetFirst("answer") == "Ja" });

                    return true;
                default:
                    break;
            }


            return true;
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
