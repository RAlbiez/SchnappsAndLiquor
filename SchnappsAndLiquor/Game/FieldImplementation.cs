using System;
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
            typeof(DrinkField),
            typeof(DrinkAndMoveField),
            typeof(SwapPositionField),
            typeof(DoubleUpField),
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
        public Guid gKey { get; set; }
        public bool bCanAppearMulitpleTimes { get; set; }
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
            gKey = Guid.NewGuid();
            bCanAppearMulitpleTimes = false;
            sText = "Start";
            shtBoardPos = 0;
        }

        public (Choice oChoice, Action<Game, Answer> Callback) FieldAction(Guid gPlayerID, Game oGame) => (null, null);

    }

    public class FinishField : IField
    {
        public Guid gKey { get; set; }
        public bool bCanAppearMulitpleTimes { get; set; }
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
            gKey = Guid.NewGuid();
            bCanAppearMulitpleTimes = false;
            sText = "Ziel";
            shtBoardPos = (short)(GameParams.MAX_FIELDS-1);
        }

        public (Choice oChoice, Action<Game, Answer> Callback) FieldAction(Guid gPlayerID, Game oGame)
        {
            oGame.WinGame(gPlayerID);

            return (null, null);
        }
    }

    public class MoveBackByField : IField
    {
        private List<string> oReasons = new List<string>()
        {
            "Hättest eigentlich nach vorne laufen dürfen, warst aber zu dumm. Gehe {0} zurück."
        };

        public Guid gKey { get; set; }
        public bool bCanAppearMulitpleTimes { get; set; }
        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        private short shtNumberToMove;

        public void Init(Game oGame, short shtPos)
        {
            shtNumberToMove = (short)GameParams.oRandomInstance.Next(-2, 0);
            string sTextToUse = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)];


            gKey = Guid.NewGuid();
            bCanAppearMulitpleTimes = true;
            sText = String.Format(sTextToUse, shtNumberToMove);
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, Answer> Callback) FieldAction(Guid gPlayerID, Game oGame)
        {
            oGame.MovePlayerBy(gPlayerID, shtNumberToMove);

            return (null, null);
        }
    }

    public class DrinkField : IField
    {
        private List<(string sTextToUse,short shtMinRange,short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Beleidige einen Mittrinker deiner Wahl. Trink {0} Schluck.", 1,3),
            ("Trink {0} und behalt sie im Mund bis du wieder an der Reihe bist.", 1,3),
            ("Schick dein letztes Bild an eine beliebige WhatsApp Gruppe und trink {0}", 1,3),
            ("Trink {0} und geh eine Runde auf die stille Treppe", 1, 3)
        };

        public Guid gKey { get; set; }
        public bool bCanAppearMulitpleTimes { get; set; }
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

            gKey = Guid.NewGuid();
            bCanAppearMulitpleTimes = true;
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, Answer> Callback) FieldAction(Guid gPlayerId, Game oGame)
        {
            oGame.AddPointsToPlayer(gPlayerId, shtNumberToDrink);

            return (null, null);
        }
    }

    public class DrinkAndMoveField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Komme aus dem Gefängnis frei. Warte falsches Spiel...Egal. Trink {0} Schluck und gehe {1} vor.", 2, 4)
        };

        public Guid gKey { get; set; }
        public bool bCanAppearMulitpleTimes { get; set; }
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

            gKey = Guid.NewGuid();
            bCanAppearMulitpleTimes = true;
            sText = String.Format(oReason.sTextToUse, shtNumberToDrink, shtNumberToMove);
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, Answer> Callback) FieldAction(Guid gPlayerId, Game oGame)
        {
            oGame.AddPointsToPlayer(gPlayerId, shtNumberToDrink);
            oGame.MovePlayerBy(gPlayerId, shtNumberToMove);

            return (null, null);
        }
    }

    public class SwapPositionField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Tausche die Position mit einem beliebigem Spieler, ihr trinkt beide {0}",1,4)
        };

        public Guid gKey { get; set; }
        public bool bCanAppearMulitpleTimes { get; set; }
        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        private short shtNumberToDrink;
        private short shtSkipCost;

        public void Init(Game oGame, short shtPos)
        {
            var oReason = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)];
            shtNumberToDrink = (short)GameParams.oRandomInstance.Next(oReason.shtMinRange, oReason.shtMaxRange);

            shtSkipCost = 3;
            gKey = Guid.NewGuid();
            bCanAppearMulitpleTimes = false;
            sText = String.Format(oReason.sTextToUse, shtNumberToDrink);
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game,Answer> Callback) FieldAction(Guid gPlayerId, Game oGame)
        {
            var oPlayerList = oGame.oPlayers as IEnumerable<Player>;

            return (new Choice(oPlayerList.Select(x => x.sName), oGame.oPlayers.GetByID(gPlayerId).sName, true, shtSkipCost), ReturnAction);
        }

        public void ReturnAction(Game oGame, Answer oAnswer)
        {
            if (!oAnswer.bSkipped)
            {
                var oFirstPlayer = oGame.GetCurentPlayer();
                var oSecondPlayer = oGame.oPlayers.GetByName(oAnswer.sAnswer);

                oFirstPlayer.AddPoints(shtNumberToDrink);
                oSecondPlayer.AddPoints(shtNumberToDrink);

                var t = oFirstPlayer.shtBoardPosition;

                oFirstPlayer.shtBoardPosition = oSecondPlayer.shtBoardPosition;
                oSecondPlayer.shtBoardPosition = t;
            }
            else
            {
                oGame.GetCurentPlayer().AddPoints(-1 * shtSkipCost);
            }
        }
    }

    public class DoubleUpField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Du trinkst {0}, wähle einen anderen Spieler, er trinkt das doppelte.", 1,3)
        };

        public Guid gKey { get; set; }
        public bool bCanAppearMulitpleTimes { get; set; }
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

            gKey = Guid.NewGuid();
            bCanAppearMulitpleTimes = true;
            sText = String.Format(oReason.sTextToUse, shtNumberToDrink);
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, Answer> Callback) FieldAction(Guid gPlayerId, Game oGame)
        {
            var oPlayerList = oGame.oPlayers as IEnumerable<Player>;

            return (new Choice(oPlayerList.Select(x => x.sName), oGame.oPlayers.GetByID(gPlayerId).sName), ReturnAction);
        }

        public void ReturnAction(Game oGame, Answer oAnswer)
        {
            var oFirstPlayer = oGame.GetCurentPlayer();
            var oSecondPlayer = oGame.oPlayers.GetByName(oAnswer.sAnswer);

            oFirstPlayer.AddPoints(shtNumberToDrink);
            oSecondPlayer.AddPoints(shtNumberToDrink * 2);
        }
    }

    public class DrinkOrDoStuffField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Rotz auf deinen Tisch oder trinke {0}.", 3,5),
            ("Ruf deiner Mutti an und sag das du sie lieb hast oder trinke {0}", 2,5)
        };

        public Guid gKey { get; set; }
        public bool bCanAppearMulitpleTimes { get; set; }
        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        private short shtNumberToDrink;

        public void Init(Game oGame, short shtPos)
        {
            var oReason = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)];
            shtNumberToDrink = (short)GameParams.oRandomInstance.Next(oReason.shtMinRange, oReason.shtMaxRange);

            gKey = Guid.NewGuid();
            bCanAppearMulitpleTimes = true;
            sText = String.Format(oReason.sTextToUse, shtNumberToDrink);
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, Answer> Callback) FieldAction(Guid gPlayerId, Game oGame)
        {
            var oPlayerList = oGame.oPlayers as IEnumerable<Player>;

            return (new Choice(new List<string>() {"Ja", "Nein" }, oGame.oPlayers.GetByID(gPlayerId).sName), ReturnAction);
        }

        public void ReturnAction(Game oGame, Answer oAnswer)
        {
            if (oAnswer.sAnswer == "Ja")
            {
                return;
            }
            else if (oAnswer.sAnswer == "Nein")
            {
                var oFirstPlayer = oGame.GetCurentPlayer();

                oFirstPlayer.AddPoints(shtNumberToDrink);
            }
        }
    }
}
