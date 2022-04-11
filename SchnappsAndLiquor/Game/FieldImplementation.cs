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
            typeof(MoveBackByField),
            typeof(MoveBackByField),
            typeof(MoveBackByField),
            typeof(DrinkField),
            typeof(DrinkField),
            typeof(DrinkField),
            typeof(DrinkField),
            typeof(DrinkField),
            typeof(DrinkField),
            typeof(DrinkField),
            typeof(DrinkAndMoveField),
            typeof(DrinkAndMoveField),
            typeof(DrinkAndMoveField),
            typeof(SwapPositionField),
            typeof(DoubleUpField),
            typeof(DoubleUpField),
            typeof(DrinkOrDoStuffField),
            typeof(DrinkOrDoStuffField),
            typeof(DrinkOrDoStuffField),
            typeof(DrinkOrDoStuffField),
            typeof(PVPField),
            typeof(ConnectPlayersField),
            typeof(CommunismField),
            typeof(RockPaperScissorsField),
            typeof(RerollBoardField),
            typeof(HeadsOrTailsField)
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

        public (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame) => (null, ReturnAction);

        public void ReturnAction(Game oGame, string sAnswer) { }
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
            ("Kein Rosinenbrötchen mit Leberwurst, keine 3 Bier, Faktor nicht wieder drinne. Gehe {0} zurück.", -3, -1),
            ("Gehe {0} zurück und beschwere dich lautstark über die Fairness dieses Spiels.", -2, 0),
            ("Du bist mal wieder auf der Toilette eingeschlafen. Gehe zur Strafe {0} zurück.", -2, 0),
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
            return (null, ReturnAction);
        }

        public void ReturnAction(Game oGame, string sAnswer)
        {
            oGame.MovePlayerBy(oGame.GetCurentPlayer().sName, shtNumberToMove);
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
            ("Alle hören ein Lied deiner Wahl, trink dafür {0}", 2, 4),
            ("Here come dat boi... oh shit, drink up! Trinke {0}", 1, 3),
            ("SAFE ZONE! Glück/Pech gehabt, du darfst nichts trinken!", 0, 1),
            ("Gestern schon wieder wie so'n Achtarmiger ein reingeorgelt mit Gerhard. Trinke {0}", 2, 5),
            ("Bar-Irrtum zu deinen Gunsten, Trinke {0}", 1,3),
            ("Rede eine Runde mit russischem Akzent und Trinke {0}", 1,3),
            ("Rede eine Runde mit französischem Akzent und Trinke {0}", 1,3),
            ("'Du hast Corona', trink {0} Schlücke Bleichmittel/Bier um dich zu heilen.", 1,3)
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
            return (null, ReturnAction);
        }

        public void ReturnAction(Game oGame, string sAnswer)
        {
            oGame.AddPointsToPlayer(oGame.GetCurentPlayer().sName, shtNumberToDrink);
        }
    }

    public class DrinkAndMoveField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Komme aus dem Gefängnis frei. Warte, falsches Spiel ... Egal. Trink {0} Schluck und gehe {1} vor.", 2, 4),
            ("Trinke {0} Schlücke und gehe {1} vor. Halte aber die Klappe bis du wieder dran bist.", 1, 4),
            ("Sage eine Bauernweisheit auf, trinke {0} und gehe {1} vor.", 1,3)
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
            return (null, ReturnAction);
        }

        public void ReturnAction(Game oGame, string sAnswer)
        {
            oGame.AddPointsToPlayer(oGame.GetCurentPlayer().sName, shtNumberToDrink);
            oGame.MovePlayerBy(oGame.GetCurentPlayer().sName, shtNumberToMove);
        }
    }

    public class SwapPositionField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Tausche die Position mit einem beliebigen Spieler, ihr trinkt beide {0}.",1,4),
            ("Wegegangen, Platz gefangen... oder so. Tausche mit jemandem, beide trinken {0}.",1,4)
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

            return (new Choice(oGame.oPlayers.Keys.Except(new List<string>() { sPlayerName }), sPlayerName, true), ReturnAction);
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
            ("Du trinkst {0}, wähle einen anderen Spieler, er trinkt das Doppelte.", 1,3),
            ("Mache einem Spieler ein Kompliment. Du trinkst {0}, er trinkt das Doppelte.", 1,3)
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
            return (new Choice(oGame.oPlayers.Keys.Except(new List<string>() { sPlayerName }), sPlayerName), ReturnAction);
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
            ("Ruf deine Mutti an und sag, dass du sie lieb hast oder trinke {0}.", 2,5),
            ("Erzähl einen Witz, wenn keiner lacht trink {0}, vielleicht wirst ja dann lustiger...", 3,5),
            ("Lies deine zuletzt erhaltene (nicht Gruppenchat-)Nachricht vor oder trinke {0}.", 2,4),
            ("Schicke ein Random Selfie an deinen 4. WhatsApp Kontakt oder trinke {0}.", 2,5),
            ("Schick ein Bild in eure Gruppe, wenn es keiner witzig findet trinke {0}.",2,5)
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
            return (new Choice(new List<string>() { "Challenge erledigt", "Trink" }, sPlayerName), ReturnAction);
        }

        public void ReturnAction(Game oGame, string sAnswer)
        {
            if (sAnswer == "Challenge erledigt")
            {
                return;
            }
            else if (sAnswer == "Trink")
            {
                var oFirstPlayer = oGame.GetCurentPlayer();

                oFirstPlayer.AddPoints(shtNumberToDrink);
            }
        }
    }

    public class PVPField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Denk dir eine Kategorie aus, alle nennen der Reihe nach einen Begriff aus dieser Kategorie, wer nix mehr weiß trinkt {0}.",3,6),
            ("Jeder malt ein Kunstwerk zu einem beliebig gewählten Thema, der Schöpfer des abstraktestem (hässlichsten) trinkt {0}", 3,6),
            ("Wir spielen 'Ich packe meinen Koffer'. Der Verlierer trinkt {0}.",3,6)
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
            var oLooser = oGame.oPlayers[sAnswer];

            oLooser.AddPoints(shtNumberToDrink);
        }
    }

    public class ConnectPlayersField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Wähle einen anderen Spieler, bis zu deinem nächsten Zug trinkt er immer mit dir.",0,1)
        };

        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        public void Init(Game oGame, short shtPos)
        {
            sText = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)].sTextToUse;
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame)
        {

            return (new Choice(oGame.oPlayers.Keys.Except(new List<string>() { sPlayerName }), sPlayerName, true), ReturnAction);
        }

        public void ReturnAction(Game oGame, string sAnswer)
        {
            var oTurnPlayer = oGame.GetCurentPlayer();

            oGame.oPlayerConnections.Add((oGame.GetCurentPlayer().sName, sAnswer, oGame.intTurnNumber + oGame.oPlayers.Count));
        }
    }

    public class CommunismField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("☭KOMMUNISMUS ☭, bis du wieder dran bist trinken alle 1 wenn du trinken müsstest.",0,1)
        };

        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        public void Init(Game oGame, short shtPos)
        {
            sText = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)].sTextToUse;
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame)
        {

            return (null, ReturnAction);
        }

        public void ReturnAction(Game oGame, string sAnswer)
        {
            var oTurnPlayer = oGame.GetCurentPlayer();

            oGame.oPlayerConnections.Add((oGame.GetCurentPlayer().sName, "☭☭☭", oGame.intTurnNumber + oGame.oPlayers.Count));
        }
    }

    public class RockPaperScissorsField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Schere, Stein, Papier, wenn du gewinnst gehe 2 vor, wenn du verlierst 2 zurück, bei unentschieden bleibst du stehen.",0,1)
        };

        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        public void Init(Game oGame, short shtPos)
        {
            sText = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)].sTextToUse;
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame)
        {

            return (new Choice(new List<string>() { "Schere", "Stein", "Papier" }, sPlayerName), ReturnAction);
        }

        public void ReturnAction(Game oGame, string sAnswer)
        {
            var oTurnPlayer = oGame.GetCurentPlayer();

            string sComputerAnswered = new List<string>() { "Schere", "Stein", "Papier" }[GameParams.oRandomInstance.Next(3)];

            if ((sAnswer == "Schere" && sComputerAnswered == "Papier")
               || (sAnswer == "Stein" && sComputerAnswered == "Schere")
               || (sAnswer == "Papier" && sComputerAnswered == "Stein"))
                oTurnPlayer.MoveBy(2);
            else if (sAnswer != sComputerAnswered)
                oTurnPlayer.MoveBy(-2);
        }
    }

    public class RerollBoardField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Das alte Brett stinkt, roll ein neues!.",0,1)
        };

        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        public void Init(Game oGame, short shtPos)
        {
            sText = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)].sTextToUse;
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame)
        {
            oGame.InitBoard();
            return (null, ReturnAction);
        }

        public void ReturnAction(Game oGame, string sAnswer)
        {
            
        }
    }

    public class HeadsOrTailsField : IField
    {
        private List<(string sTextToUse, short shtMinRange, short shtMaxRange)> oReasons = new List<(string sTextToUse, short shtMinRange, short shtMaxRange)>()
        {
            ("Wirf eine Münze bei Kopf trinkst du drei, bei Zahl der Rest einen.",0,1)
        };

        public string sText { get; set; }
        public short shtBoardPos { get; set; }
        public bool bIsStartPoint { get; set; }
        public bool bIsEndPoint { get; set; }

        public void Init(Game oGame, short shtPos)
        {
            sText = oReasons[GameParams.oRandomInstance.Next(oReasons.Count)].sTextToUse;
            shtBoardPos = shtPos;
        }

        public (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame)
        {
            return (new Choice(new List<string>() { "Kopf", "Zahl" }, sPlayerName), ReturnAction);
        }

        public void ReturnAction(Game oGame, string sAnswer)
        {
            if(sAnswer == "Kopf")
            {
                oGame.AddPointsToPlayer(oGame.GetCurentPlayer().sName, 3);
            }
            else
            {
                foreach(var sPlayer in oGame.oPlayers.Keys.Where(x => x != oGame.GetCurentPlayer().sName))
                {
                    oGame.AddPointsToPlayer(sPlayer, 3);
                }
            }
        }
    }
}
