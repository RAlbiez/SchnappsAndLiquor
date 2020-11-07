using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SchnappsAndLiquor.Game
{
    public interface IField
    {
        Guid gKey { get; set; }
        bool bCanAppearMulitpleTimes { get; set; }
        string sText { get; set; }
        short shtBoardPos { get; set; }
        void Init(Game oGame, short shtPos);
        (Choice oChoice, Action<Game, Answer> Callback) FieldAction(Guid gPlayerId, Game oGame);
    }


    public class FieldService
    {
        private HashSet<Type> oAlreadyAdded = new HashSet<Type>();

        public IField Next(Game oGame, short shtPos)
        {
            IField oGenerated;
            bool bNewFieldGeneratedSuccessfully = true;
            do
            {
                oGenerated = FieldTypes.GetRandomField();

                if (oGenerated.bCanAppearMulitpleTimes && !oAlreadyAdded.Add(oGenerated.GetType()))
                {
                    bNewFieldGeneratedSuccessfully = false;
                }
            }
            while (!bNewFieldGeneratedSuccessfully);

            oGenerated.Init(oGame, shtPos);

            return oGenerated;
        }
    }
}
