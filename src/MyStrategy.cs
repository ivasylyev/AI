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

                var eps = 10;
                var delteShift = 4.5;
                var commonCoordinate = 250;
                var nearCoordinate = 80;
                var farCoordinate = 200;
                var factor = 1.5;
                if (Math.Abs(fighters.Rectangle.Left - helicopters.Rectangle.Left) < eps)
                {
                    Formation topFormation;
                    Formation bottomFormation;
                    if (fighters.Rectangle.Top < helicopters.Rectangle.Top)
                    {
                        topFormation = fighters;
                        bottomFormation = helicopters;
                    }
                    else
                    {
                        topFormation = helicopters;
                        bottomFormation = fighters;
                    }
                    var topSequence = new ActionSequence(
                        topFormation.MoveLeftTopTo(commonCoordinate + delteShift, nearCoordinate,
                            Global.Game.HelicopterSpeed),
                        topFormation.ScaleLeftTop(factor)
                    );
                    Global.ActionQueue.Add(topSequence);
                    var bottomSequence = new ActionSequence(
                        bottomFormation.MoveLeftTopTo(commonCoordinate, farCoordinate, Global.Game.HelicopterSpeed),
                        bottomFormation.ScaleLeftTop(factor)
                    );
                    Global.ActionQueue.Add(bottomSequence);

                    var moveDownAction = topFormation.ShiftTo(0, farCoordinate - nearCoordinate);
                    moveDownAction.StartCondition = () => topSequence.IsFinished && bottomSequence.IsFinished;
                    var moveDownSequence = new ActionSequence(moveDownAction);
                    Global.ActionQueue.Add(moveDownSequence);

                    var moveUpAction = bottomFormation.ShiftTo(0, -farCoordinate + nearCoordinate);
                    moveUpAction.StartCondition = () => topSequence.IsFinished && bottomSequence.IsFinished;
                    var moveUpSequence = new ActionSequence(moveUpAction);
                    Global.ActionQueue.Add(moveUpSequence);
                }
                else
                {
                    Formation leftFormation;
                    Formation rightFormation;
                    if (fighters.Rectangle.Left < helicopters.Rectangle.Left)
                    {
                        leftFormation = fighters;
                        rightFormation = helicopters;
                    }
                    else
                    {
                        leftFormation = helicopters;
                        rightFormation = fighters;
                    }
                    var leftSequence = new ActionSequence(
                        leftFormation.MoveLeftTopTo(nearCoordinate, commonCoordinate + delteShift,
                            Global.Game.HelicopterSpeed),
                        leftFormation.ScaleLeftTop(factor)
                    );
                    Global.ActionQueue.Add(leftSequence);
                    var rightSequence = new ActionSequence(
                        rightFormation.MoveLeftTopTo(farCoordinate, commonCoordinate, Global.Game.HelicopterSpeed),
                        rightFormation.ScaleLeftTop(factor)
                    );
                    Global.ActionQueue.Add(rightSequence);

                    var moveLeftAction = leftFormation.ShiftTo(farCoordinate - nearCoordinate, 0);
                    moveLeftAction.StartCondition = () => leftSequence.IsFinished && rightSequence.IsFinished;

                    var moveLeftSequence = new ActionSequence(moveLeftAction);
                    Global.ActionQueue.Add(moveLeftSequence);

                    var moveRightAction = rightFormation.ShiftTo(-farCoordinate + nearCoordinate, 0);
                    moveRightAction.StartCondition = () => leftSequence.IsFinished && rightSequence.IsFinished;

                    var moveRightSequence = new ActionSequence(moveRightAction);
                    Global.ActionQueue.Add(moveRightSequence);
                }


//
//                var moveAction = fighters.MoveLeftTopTo(50, 250);
//                var moveAction2 = fighters.MoveLeftTopTo(100, 250);
//                var moveAction3 = fighters.MoveLeftTopTo(300, 350);
//                var sequence = new ActionSequence(moveAction, moveAction2, moveAction3);

//                                var splitActions = fighters.Split(50);
//                                sequence.Add(splitActions);

                //       Global.ActionQueue.Add(sequence);
//
//                var moveAction4 = helicopters.MoveLeftTopTo(250, 50);
//                var moveAction5 = helicopters.MoveLeftTopTo(250, 100);
//                var moveAction6 = helicopters.MoveLeftTopTo(350, 300);
//                var sequence2 = new ActionSequence(moveAction4, moveAction5, moveAction6);
//
//
//                Global.ActionQueue.Add(sequence2);
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