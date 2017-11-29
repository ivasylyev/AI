using System;
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
                var fighters = Global.Formations[-(int) VehicleType.Fighter];
                var helicopters = Global.Formations[-(int) VehicleType.Helicopter];
                var formation = fighters.Rectangle.Left + fighters.Rectangle.Top >
                                helicopters.Rectangle.Left + helicopters.Rectangle.Top
                    ? fighters
                    : helicopters;

                var moveAction = formation.MoveLeftTopTo(250, 250);
                var moveAction2 = formation.MoveLeftTopTo(300, 250);
                var moveAction3 = formation.MoveLeftTopTo(300, 350);
                var sequence = new ActionSequence(moveAction, moveAction2, moveAction3);

                                var splitActions = formation.Split(50);
                                sequence.Add(splitActions);

                Global.ActionQueue.Add(sequence);
            }

//            if (Global.World.TickIndex % 60 == 0)
//            {
//                var fighters = Global.Formations[-(int) VehicleType.Fighter];
//                /*
//                 * split and shift in different ways
//                 * */
//                if (!fighters.Busy && !fighters.Children.Any())
//                {
//                    fighters.Split(50);
//                    count = 0;
//                }
//                if (!_isFightersScaled)
//                {
//                    
//                    if (fighters.Children.Count == 4 && count ==1)
//                    {
//                        fighters.Children[2].Shift(100, 100);
//                        fighters.Children[3].Shift(-100, 100);
//                        _isFightersScaled = true;
//                    }
//                    count++;
//
//                
//                }
//                
//            }
//            if (Global.World.TickIndex % 10 == 0)
//            {
//                foreach (var key1 in Global.Formations.Keys)
//                {
//                    foreach (var key2 in Global.Formations.Keys.Where(k=>k!=key1))
//                    {
//                        var f1 = Global.Formations[key1];
//                        var f2 = Global.Formations[key2];
//                        var distBetweenCenters = f1.Rectangle.Center.SqrDistance(f2.Rectangle.Center);
//                        if (distBetweenCenters < (f1.Rectangle.SqrDiameter + f1.Rectangle.SqrDiameter) / 2)
//                        {
//                            
//                        }
//                    }
//                }
//            }
        }
    }
}