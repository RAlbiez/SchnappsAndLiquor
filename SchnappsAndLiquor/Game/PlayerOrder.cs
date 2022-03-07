using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchnappsAndLiquor.Game
{
    public class PlayerOrder
    {
        private List<string> oNames = new List<string>();
        private int intIndex = 0;

        public void AddPlayer(string sName)
        {
            oNames.Add(sName);
        }

        public void RemovePlayer(string sName)
        {
            oNames.Remove(sName);
        }

        public string GetCurrent() => oNames[intIndex];

        public string Next()
        {
            intIndex++;
            if (intIndex >= oNames.Count)
                intIndex = 0;

            return GetCurrent();
        }
    }
}
