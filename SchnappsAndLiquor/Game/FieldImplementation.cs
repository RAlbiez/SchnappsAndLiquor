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
            typeof(SwapPositionField)
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

        private short shtNumberToMove;

        public void Init(Game oGame, short shtPos)
        {
            shtNumberToMove = (short)GameParams.oRandomInstance.Next(-4, 0);
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
        private List<string> oReasons = new List<string>()
        {
            "Beleidige einen Mittrinker deiner Wahl. Trink {0} Schluck."
        };

        public Guid gKey { get; set; }
        public bool bCanAppearMulitpleTimes { get; set; }
        public string sText { get; set; }
        public short shtBoardPos { get; set; }

        private short shtNumberToDrink;

        public void Init(Game oGame, short shtPos)
        {
            shtNumberToDrink = (short)GameParams.oRandomInstance.Next(1, 3);
            string sTextToUse = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)];

            gKey = Guid.NewGuid();
            bCanAppearMulitpleTimes = true;
            sText = String.Format(sTextToUse, shtNumberToDrink);
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
        private List<string> oReasons = new List<string>()
        {
            "Komme aus dem Gefängnis frei. Warte falsches Spiel...Egal. Trink {0} Schluck und gehe {1} vor."
        };

        public Guid gKey { get; set; }
        public bool bCanAppearMulitpleTimes { get; set; }
        public string sText { get; set; }
        public short shtBoardPos { get; set; }

        private short shtNumberToDrink;
        private short shtNumberToMove;

        public void Init(Game oGame, short shtPos)
        {
            shtNumberToDrink = (short)GameParams.oRandomInstance.Next(2, 4);
            shtNumberToMove = (short)GameParams.oRandomInstance.Next(1, 3);

            string sTextToUse = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)];

            gKey = Guid.NewGuid();
            bCanAppearMulitpleTimes = true;
            sText = String.Format(sTextToUse, shtNumberToDrink, shtNumberToMove);
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
        private List<string> oReasons = new List<string>()
        {
            "Tausche die Position mit einem beliebigem Spiel, ihr trinkt beide {0}"
        };

        public Guid gKey { get; set; }
        public bool bCanAppearMulitpleTimes { get; set; }
        public string sText { get; set; }
        public short shtBoardPos { get; set; }

        private short shtNumberToDrink;
        private short shtSkipCost;

        public void Init(Game oGame, short shtPos)
        {
            shtNumberToDrink = (short)GameParams.oRandomInstance.Next(1, 4);

            string sTextToUse = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)];

            shtSkipCost = 3;
            gKey = Guid.NewGuid();
            bCanAppearMulitpleTimes = false;
            sText = String.Format(sTextToUse, shtNumberToDrink);
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
}
