using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace SchnappsAndLiquor.Game
{
    public class SnakeOrLadder
    {
        public short shtStartPoint;
        public short shtEndPoint;
        public short shtSkipCost;
        public bool bSnake;

        public void Generate(Game oGame)
        {
            int intLinesToSkip = GameParams.oRandomInstance.Next(1, 3);
            bool bGeneratedSuccessfully = false;

            while (!bGeneratedSuccessfully)
            {
                int intPos1 = GameParams.oRandomInstance.Next(1, GameParams.MAX_FIELDS - 2);
                int intPos2 = GetPos2(intPos1, intLinesToSkip);

                if (CheckPos2(intPos2))
                {
                    continue;
                }

                if (oGame.oBoard[(short)intPos1].bIsStartPoint || oGame.oBoard[(short)intPos1].bIsEndPoint || oGame.oBoard[(short)intPos2].bIsStartPoint || oGame.oBoard[(short)intPos2].bIsEndPoint)
                    continue;

                shtSkipCost = (short)(Math.Abs(intPos2 - intPos1) / 4);

                bGeneratedSuccessfully = true;

                oGame.oBoard[(short)intPos1].bIsStartPoint = true;
                oGame.oBoard[(short)intPos2].bIsEndPoint = true;

                shtStartPoint = (short)intPos1;
                shtEndPoint = (short)intPos2;
            }
        }

        public virtual int GetPos2(int intPos1, int intLinesToSkip) => 0;

        public virtual bool CheckPos2(int intPos2) => false;
    }

    public class Snake : SnakeOrLadder
    {
        public Snake(Game oGame)
        {
            this.Generate(oGame);

            bSnake = true;
        }

        public override int GetPos2(int intPos1, int intLinesToSkip) => intPos1 - intLinesToSkip * GameParams.WIDTH + GameParams.oRandomInstance.Next(-1 * intLinesToSkip - 1, intLinesToSkip + 1);

        public override bool CheckPos2(int intPos2) => intPos2 < 1; 
    }

    public class Ladder : SnakeOrLadder
    {
        public Ladder(Game oGame)
        {
            this.Generate(oGame);

            bSnake = false;
        }

        public override int GetPos2(int intPos1, int intLinesToSkip) => intPos1 + intLinesToSkip * GameParams.WIDTH + GameParams.oRandomInstance.Next(-1 * intLinesToSkip - 1, intLinesToSkip + 1);

        public override bool CheckPos2(int intPos2) => intPos2 > GameParams.MAX_FIELDS - 2;
    }

    public static class SnakeAndLadderService
    {
        public static List<SnakeOrLadder> GenerateSnakesAndLadders(Game oGame)
        {
            List<SnakeOrLadder> oList = new List<SnakeOrLadder>();

            short shtCount = (GameParams.MAX_FIELDS / 10 + 1) / 2;

            for(int i = 0; i<shtCount; i++)
            {
                oList.Add(new Snake(oGame));
                oList.Add(new Ladder(oGame));
            }

            return oList;
        }
    }
}
