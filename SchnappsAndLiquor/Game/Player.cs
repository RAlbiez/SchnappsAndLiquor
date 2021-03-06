﻿using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchnappsAndLiquor.Game
{
    public class Player
    {
        public string sName;
        public short shtBoardPosition = 0;
        public string sColor = "#FF00FF";

        public long lngPoints = 0;


        public Player(string sNameP, string sColorP)
        {
            this.sName = sNameP;
            this.sColor = sColorP;
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
