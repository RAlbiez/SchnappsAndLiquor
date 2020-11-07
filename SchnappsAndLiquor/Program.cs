using System;
using SchnappsAndLiquor.Game;
using SchnappsAndLiquor.Net;

namespace SchnappsAndLiquor
{
    class Program
    {
        static void Main(string[] args)
        {
            Connection oConnection = new Connection();
            Game.Game oGame = new Game.Game(oConnection);

            new Connection().SendGameToEveryone(oGame);
        }
    }
}
