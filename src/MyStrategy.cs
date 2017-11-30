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
            const double eps = 10D;
            const double deltaShift = 4.5D;
            const double commonCoordinate = 250D;
            const double nearCoordinate = 80D;
            const double farCoordinate = 200D;
            const double factor = 1.5D;
            

            if (Global.World.TickIndex == 1)
            {
                var fighters = Global.Formations[-(int) VehicleType.Fighter];
                var helicopters = Global.Formations[-(int) VehicleType.Helicopter];

                var isLeftTheSame = Math.Abs(fighters.Rectangle.Left - helicopters.Rectangle.Left) < eps;

                Formation f1;
                Formation f2;
                double f1MoveX;
                double f1MoveY;
                double f2MoveX;
                double f2MoveY;
                double shiftX;
                double shiftY;
                if (isLeftTheSame)
                {
                    if (fighters.Rectangle.Top < helicopters.Rectangle.Top)
                    {
                        f1 = fighters;
                        f2 = helicopters;
                    }
                    else
                    {
                        f1 = helicopters;
                        f2 = fighters;
                    }
                    f1MoveX = commonCoordinate + deltaShift;
                    f1MoveY = nearCoordinate;
                    f2MoveX = commonCoordinate;
                    f2MoveY = farCoordinate;
                    shiftX = 0;
                    shiftY = (farCoordinate - nearCoordinate) / 2;
                    
                    var sMove1 = new ActionSequence(
                        f1.MoveLeftTopTo(f1MoveX, f1MoveY, Global.Game.HelicopterSpeed),
                        f1.ScaleLeftTop(factor)
                    );
                    Global.ActionQueue.Add(sMove1);
                    var sMove2 = new ActionSequence(
                        f2.MoveLeftTopTo(f2MoveX, f2MoveY, Global.Game.HelicopterSpeed),
                        f2.ScaleLeftTop(factor)
                    );
                    Global.ActionQueue.Add(sMove2);


                    var aPenetrate1 = f1.ShiftTo(shiftX, shiftY);
                    aPenetrate1.StartCondition = () => sMove1.IsFinished && sMove2.IsFinished;
                    var sPenetrate1 = new ActionSequence(aPenetrate1);
                    Global.ActionQueue.Add(sPenetrate1);

                    var aPenetrate2 = f2.ShiftTo(-shiftX, -shiftY);
                    aPenetrate2.StartCondition = () => sMove1.IsFinished && sMove2.IsFinished;
                    var sPenetrate2 = new ActionSequence(aPenetrate2);
                    Global.ActionQueue.Add(sPenetrate2);

                    var fTrunc = FormationFactory.CreateFormation(f1.Rectangle.Left, f1.Rectangle.Top,
                        f1.Rectangle.Right, f1.Rectangle.Bottom - 4);


                    var sCompact = new ActionSequence(fTrunc.ActionList.ToArray());
                    sCompact.First().StartCondition = () => sPenetrate2.IsFinished && sPenetrate1.IsFinished;
                    foreach (var action in sCompact)
                    {
                        action.Urgent = true;
                    }
                    sCompact.Add(fTrunc.Formation.ShiftTo(shiftX, shiftY));
                    Global.ActionQueue.Add(sCompact);
                }
                else
                {
                    if (fighters.Rectangle.Left < helicopters.Rectangle.Left)
                    {
                        f1 = fighters;
                        f2 = helicopters;
                    }
                    else
                    {
                        f1 = helicopters;
                        f2 = fighters;
                    }
                    f1MoveX = nearCoordinate;
                    f1MoveY = commonCoordinate + deltaShift;
                    f2MoveX = farCoordinate;
                    f2MoveY = commonCoordinate;
                    shiftX = (farCoordinate - nearCoordinate) / 2;
                    shiftY = 0;
                    var sMove1 = new ActionSequence(
                        f1.MoveLeftTopTo(f1MoveX, f1MoveY,Global.Game.HelicopterSpeed),
                        f1.ScaleLeftTop(factor)
                    );
                    Global.ActionQueue.Add(sMove1);
                    
                    var sMove2 = new ActionSequence(
                        f2.MoveLeftTopTo(f2MoveX, f2MoveY, Global.Game.HelicopterSpeed),
                        f2.ScaleLeftTop(factor)
                    );
                    Global.ActionQueue.Add(sMove2);

                    var aPenetrate1 = f1.ShiftTo(shiftX, shiftY);
                    aPenetrate1.StartCondition = () => sMove1.IsFinished && sMove2.IsFinished;

                    var sPenetrate1 = new ActionSequence(aPenetrate1);
                    Global.ActionQueue.Add(sPenetrate1);

                    var aPenetrate2 = f2.ShiftTo(-shiftX, -shiftY);
                    aPenetrate2.StartCondition = () => sMove1.IsFinished && sMove2.IsFinished;

                    var sPenetrate2 = new ActionSequence(aPenetrate2);
                    Global.ActionQueue.Add(sPenetrate2);

                    var fTrunc = FormationFactory.CreateFormation(f1.Rectangle.Left, f1.Rectangle.Top,
                        f1.Rectangle.Right - 4, f1.Rectangle.Bottom);

                    var sCompact = new ActionSequence(fTrunc.ActionList.ToArray());
                    sCompact.First().StartCondition = () => sPenetrate2.IsFinished && sPenetrate1.IsFinished;
                    foreach (var action in sCompact)
                    {
                        action.Urgent = true;
                    }
                    sCompact.Add(fTrunc.Formation.ShiftTo(shiftX, shiftY));
                    Global.ActionQueue.Add(sCompact);
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