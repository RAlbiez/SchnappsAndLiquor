﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchnappsAndLiquor.Game
{
    public static class FieldTypes
    {
        private static List<Type> oFieldTypes = new List<Type>()
        {
            typeof(MoveBackByField),
            typeof(MoveBackByField),
            typeof(MoveBackByField),
            typeof(DrinkField),
            typeof(DrinkField),
            typeof(DrinkField),
            typeof(DrinkField),
            typeof(DrinkAndMoveField),
            typeof(DrinkAndMoveField),
            typeof(SwapPositionField),
            typeof(DoubleUpField),
            typeof(DoubleUpField),
            typeof(DrinkOrDoStuffField),
            typeof(DrinkOrDoStuffField)
        };

        public static IField GetRandomField()
        {
            Type oType = oFieldTypes[GameParams.oRandomInstance.Next(oFieldTypes.Count)];

            return (IField)Activator.CreateInstance(oType);
        }
    }

    public class StartField : IField
    {
        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        public StartField()
        {
            this.Init(null, 0);
        }

        public void Init(Game oGame, short shtPos)
        {
            sText = "Start";
            shtBoardPos = 0;
        }

        public (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame) => (null, null);

    }

    public class FinishField : IField
    {
        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        public FinishField()
        {
            this.Init(null, 0);
        }

        public void Init(Game oGame, short shtPos)
        {
            sText = "Ziel";
            shtBoardPos = (short)(GameParams.MAX_FIELDS - 1);
        }

        public (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame)
        {
            oGame.WinGame(sPlayerName);

            return (null, null);
        }
    }

    public class MoveBackByField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Hättest eigentlich nach vorne laufen dürfen, warst aber zu dumm. Gehe {0} zurück.", -2 , 0),
            ("Kein Rosinenbrötchen mit Leberwurst, keine 3 Bier, Faktor nicht wieder drinne. Gehe {0} zurück.", -3, -1)
        };

        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        private short shtNumberToMove;

        public void Init(Game oGame, short shtPos)
        {

            var oReason= oReasons[GameParams.oRandomInstance.Next(oReasons.Count)];
            shtNumberToMove = (short)GameParams.oRandomInstance.Next(oReason.shtMinRange, oReason.shtMaxRange);

            sText = String.Format(oReason.sTextToUse, Math.Abs(shtNumberToMove));
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame)
        {
            oGame.MovePlayerBy(sPlayerName, shtNumberToMove);

            return (null, null);
        }
    }

    public class DrinkField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Beleidige einen Mittrinker deiner Wahl. Trink {0} Schlück/e.", 1,3),
            ("Trink {0} und behalte sie im Mund, bis du wieder an der Reihe bist.", 1,3),
            ("Schick dein letztes Bild an eine beliebige WhatsApp-Gruppe und trink {0}.", 1,3),
            ("Trink {0} und geh eine Runde auf die stille Treppe.", 1, 3),
            ("Alle hören ein Lied deiner Wahl, trink dafür {0}", 2, 4)
        };

        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        private short shtNumberToDrink;

        public void Init(Game oGame, short shtPos)
        {
            var oReason = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)];

            shtNumberToDrink = (short)GameParams.oRandomInstance.Next(oReason.shtMinRange, oReason.shtMaxRange);
            sText = String.Format(oReason.sTextToUse, shtNumberToDrink);

            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame)
        {
            oGame.AddPointsToPlayer(sPlayerName, shtNumberToDrink);

            return (null, null);
        }
    }

    public class DrinkAndMoveField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Komme aus dem Gefängnis frei. Warte, falsches Spiel ... Egal. Trink {0} Schluck und gehe {1} vor.", 2, 4)
        };

        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        private short shtNumberToDrink;
        private short shtNumberToMove;

        public void Init(Game oGame, short shtPos)
        {
            shtNumberToMove = (short)GameParams.oRandomInstance.Next(1, 3);

            var oReason = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)];

            shtNumberToDrink = (short)GameParams.oRandomInstance.Next(oReason.shtMinRange, oReason.shtMaxRange);

            sText = String.Format(oReason.sTextToUse, shtNumberToDrink, shtNumberToMove);
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame)
        {
            oGame.AddPointsToPlayer(sPlayerName, shtNumberToDrink);
            oGame.MovePlayerBy(sPlayerName, shtNumberToMove);

            return (null, null);
        }
    }

    public class SwapPositionField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Tausche die Position mit einem beliebigen Spieler, ihr trinkt beide {0}.",1,4)
        };

        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        private short shtNumberToDrink;

        public void Init(Game oGame, short shtPos)
        {
            var oReason = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)];
            shtNumberToDrink = (short)GameParams.oRandomInstance.Next(oReason.shtMinRange, oReason.shtMaxRange);

            sText = String.Format(oReason.sTextToUse, shtNumberToDrink);
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame)
        {

            return (new Choice(oGame.oPlayers.Keys, sPlayerName, true), ReturnAction);
        }

        public void ReturnAction(Game oGame, string sAnswer)
        {
            var oFirstPlayer = oGame.GetCurentPlayer();
            var oSecondPlayer = oGame.oPlayers[sAnswer];

            oFirstPlayer.AddPoints(shtNumberToDrink);
            oSecondPlayer.AddPoints(shtNumberToDrink);

            var t = oFirstPlayer.shtBoardPosition;

            oFirstPlayer.shtBoardPosition = oSecondPlayer.shtBoardPosition;
            oSecondPlayer.shtBoardPosition = t;
        }
    }

    public class DoubleUpField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Du trinkst {0}, wähle einen anderen Spieler, er trinkt das Doppelte.", 1,3)
        };

        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        private short shtNumberToDrink;

        public void Init(Game oGame, short shtPos)
        {
            shtNumberToDrink = (short)GameParams.oRandomInstance.Next(1, 3);

            var oReason = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)];

            shtNumberToDrink = (short)GameParams.oRandomInstance.Next(oReason.shtMinRange, oReason.shtMaxRange);

            sText = String.Format(oReason.sTextToUse, shtNumberToDrink);
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame)
        {
            return (new Choice(oGame.oPlayers.Keys, sPlayerName), ReturnAction);
        }

        public void ReturnAction(Game oGame, string sAnswer)
        {
            var oFirstPlayer = oGame.GetCurentPlayer();
            var oSecondPlayer = oGame.oPlayers[sAnswer];

            oFirstPlayer.AddPoints(shtNumberToDrink);
            oSecondPlayer.AddPoints(shtNumberToDrink * 2);
        }
    }

    public class DrinkOrDoStuffField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Rotz auf deinen Tisch oder trinke {0}.", 3,5),
            ("Ruf deine Mutti an und sag, dass du sie lieb hast oder trinke {0}.", 2,5)
        };

        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        private short shtNumberToDrink;

        public void Init(Game oGame, short shtPos)
        {
            var oReason = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)];
            shtNumberToDrink = (short)GameParams.oRandomInstance.Next(oReason.shtMinRange, oReason.shtMaxRange);

            sText = String.Format(oReason.sTextToUse, shtNumberToDrink);
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame)
        {
            return (new Choice(new List<string>() { "Ja", "Nein" }, sPlayerName), ReturnAction);
        }

        public void ReturnAction(Game oGame, string sAnswer)
        {
            if (sAnswer == "Ja")
            {
                return;
            }
            else if (sAnswer == "Nein")
            {
                var oFirstPlayer = oGame.GetCurentPlayer();

                oFirstPlayer.AddPoints(shtNumberToDrink);
            }
        }
    }
}
