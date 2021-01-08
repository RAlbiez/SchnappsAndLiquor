using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace SchnappsAndLiquor.Game
{
    public interface IField
    {
        bool bIsStartPoint { get; set; }
        bool bIsEndPoint { get; set; }
        string sText { get; set; }
        short shtBoardPos { get; set; }
        void Init(Game oGame, short shtPos);
        (Choice oChoice, Action<Game, string> Callback) FieldAction(string sPlayerName, Game oGame);
    }


    public class FieldService
    {
        private HashSet<Type> oAlreadyAdded = new HashSet<Type>();

        public IField Next(Game oGame, short shtPos)
        {
            IField oGenerated = FieldTypes.GetRandomField();

            oGenerated.Init(oGame, shtPos);

            return oGenerated;
        }
    }
}
