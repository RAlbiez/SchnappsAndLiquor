using System;
using System.Collections.Generic;
using System.Text;

namespace SchnappsAndLiquor.Game
{
    public class Player
    {
        public string sName;
        public Guid gPlayerID;
        short shtBoardPosition = 0;

        long lngPoints = 0;


        public Player(string sNameP)
        {
            this.sName = sNameP;
            this.gPlayerID = Guid.NewGuid();
        }

        public short MoveBy(short shtNumFieldsP)
        {
            this.shtBoardPosition += shtNumFieldsP;
            this.shtBoardPosition = this.shtBoardPosition > GameParams.MAX_FIELDS - 1 ? (short)(GameParams.MAX_FIELDS - 1) : this.shtBoardPosition;
            return this.shtBoardPosition;
        }

        public void AddPoints(long lngNumberOfPoints)
        {
            this.lngPoints += lngNumberOfPoints;
        }
    }
}
