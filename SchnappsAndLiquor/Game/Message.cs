using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchnappsAndLiquor.Game
{
    public class Message
    {
        public string sMessageID = Guid.NewGuid().ToString();

        public string sMessageType;
        public List<string > oAdditionalFields;
        public string sPlayerName;
        public Choice oChoice;
        [JsonIgnore]
        public Action<Game, string> oCallback;

        public Message(string sMessageTypeP, string sPlayerNameP, Choice oChoiceP, Action<Game, string> oCallbackP, List<string> oAdditionalFieldsP) : this(sMessageTypeP, sPlayerNameP, oChoiceP, oCallbackP)
        {
            oAdditionalFields = oAdditionalFieldsP;
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
