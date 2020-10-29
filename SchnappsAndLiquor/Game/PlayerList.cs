using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchnappsAndLiquor.Game
{
    public class PlayerList
    {
        private List<Player> oList = new List<Player>();

        public Player this[short shtCurrentPlayerP]
        {
            get => this.oList[shtCurrentPlayerP];
            set => this.oList[shtCurrentPlayerP] = value;
        }

        public void Add(Player oPlayerP)
        {
            this.oList.Add(oPlayerP);
        }

        public Player GetByID(Guid gPlayerIDP) => this.oList.Where(x => x.gPlayerID == gPlayerIDP).FirstOrDefault();

        public Player GetByName(string sNameP) => this.oList.Where(x => x.sName == sNameP).FirstOrDefault();
    }
}
