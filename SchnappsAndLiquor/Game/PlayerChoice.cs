using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace SchnappsAndLiquor.Game
{
    public class Choice
    {
        public string[] oChoices;
        [JsonIgnore]
        public bool bCanSkip = false;
        public string sPlayer;
        public string sSpecialType;

        public Choice(IEnumerable<string> oInput, string sPlayerP)
        {
            oChoices = oInput.ToArray<string>();
            sPlayer = sPlayerP;
        }

        public Choice(IEnumerable<string> oInput, string sPlayerP, bool bCanSkipP) : this(oInput, sPlayerP)
        {
            bCanSkip = bCanSkipP;
        }
    }
}
