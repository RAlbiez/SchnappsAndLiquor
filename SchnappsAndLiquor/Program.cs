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

            //new game erstellt wird

            Game.Game oGame = new Game.Game(oConnection);

            oGame.AddPlayer("asdf");
            oGame.AddPlayer("2134");

            new Connection().SendGameToEveryone(oGame);
        }
    }
}
