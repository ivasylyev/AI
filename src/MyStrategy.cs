using System;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed class MyStrategy : IStrategy
    {
        public void Move(Player me, World world, Game game, Move move)
        {
            try
            {
                Initializer.Init(me, world, game, move);

                SelectionTest();

                Global.ActionQueue.Process();
            }
            catch (Exception ex)
            {
                var exception = ex;
            }
        }


        private void SelectionTest()
        {
            if (Global.World.TickIndex == 1)
            {
              //  TacticalActions.SplitFormation(Global.MyIfvs, 50);
            }
            if (Global.MyAirFormation.Alive)
            {
            }
            // todo: убрать после экспериментов

            //            if (Global.World.TickIndex % 10 == 0)
            //            {
            //                foreach (var key1 in Global.Formations.Keys)
            //                {
            //                    foreach (var key2 in Global.Formations.Keys.Where(k=>k!=key1))
            //                    {
            //                        var f1 = Global.Formations[key1];
            //                        var f2 = Global.Formations[key2];
            //                        var distBetweenCenters = f1.Rect.Center.SqrDistance(f2.Rect.Center);
            //                        if (distBetweenCenters < (f1.Rect.SqrDiameter + f1.Rect.SqrDiameter) / 2)
            //                        {
            //                            
            //                        }
            //                    }
            //                }
            //            }
        }
    }
}