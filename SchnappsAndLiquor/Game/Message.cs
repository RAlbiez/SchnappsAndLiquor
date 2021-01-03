using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchnappsAndLiquor.Game
{
    class Message
    {
        public string sMessageID = Guid.NewGuid().ToString();

        public string sMessageType;
        public string sSpecialField;
        public string sPlayerName;
        public Choice oChoice;
        [JsonIgnore]
        public Action<Game, string> oCallback;

        public Message(string sMessageTypeP, string sPlayerNameP, Choice oChoiceP, Action<Game, string> oCallbackP, string sSpecialFieldP) : this(sMessageTypeP, sPlayerNameP, oChoiceP, oCallbackP)
        {
            sSpecialField = sSpecialFieldP;
        }

        public Message(string sMessageTypeP, string sPlayerNameP, Choice oChoiceP, Action<Game,string> oCallbackP) : this(sMessageTypeP, sPlayerNameP)
        {
            oChoice = oChoiceP;
            oCallback = oCallbackP;
        }

        public Message(string sMessageTypeP, string sPlayerNameP)
        {
            sMessageType = sMessageTypeP;
            sPlayerName = sPlayerNameP;
        }
    }
}
