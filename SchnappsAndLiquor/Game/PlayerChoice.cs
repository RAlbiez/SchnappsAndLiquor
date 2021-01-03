using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchnappsAndLiquor.Game
{
    public class Choice
    {
        public string[] oChoices;
        public bool bCanSkip = false;
        public short shtSkipCost = 0;
        public string sPlayer;
        public string sSpecialType;

        public Choice(IEnumerable<string> oInput, string sPlayerP)
        {
            oChoices = oInput.ToArray<string>();
            sPlayer = sPlayerP;
        }

        public Choice(IEnumerable<string> oInput, string sPlayerP, bool bCanSkipP, short shtSkipCostP) : this(oInput, sPlayerP)
        {
            bCanSkip = bCanSkipP;
            shtSkipCost = shtSkipCostP;
        }
    }
}
