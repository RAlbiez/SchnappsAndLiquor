using System;
using System.Collections.Generic;
using System.Text;

namespace SchnappsAndLiquor.Net
{
    public class KeyValPair
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// Action sent from client
    /// </summary>
    public class ClientAction
    {
        public string Type { get; set; }
        public List<KeyValPair> Parameters { get; set; }

        public string GetFirst(string key)
        {
            foreach (var i in this.Parameters)
            {
                if (i.Key == key)
                {
                    return i.Value;
                }
            }
            return null;
        }
    }
}
