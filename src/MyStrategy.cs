﻿using System;
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
            const double deltaShift = 4.8D;
            const double commonCoordinate = 250D;
            const double nearCoordinate = 50D;
            const double farCoordinate = 200D;
            const double factor = 1.6D;
            const double vehicleSize = 4D;


            if (Global.World.TickIndex == 1)
            {
                var fighters = Global.Formations[-(int) VehicleType.Fighter];
                var helicopters = Global.Formations[-(int) VehicleType.Helicopter];

                var isVertical = Math.Abs(fighters.Rect.Left - helicopters.Rect.Left) < eps;

                Formation f1;
                Formation f2;
                double f1MoveX;
                double f1MoveY;
                double f2MoveX;
                double f2MoveY;
                double shiftX;
                double shiftY;
                double compactX;
                double compactY;

                if (isVertical)
                {
                    f1MoveX = commonCoordinate + deltaShift;
                    f1MoveY = nearCoordinate;
                    f2MoveX = commonCoordinate;
                    f2MoveY = farCoordinate;
                    shiftX = 0;
                    shiftY = (farCoordinate - nearCoordinate) / 2;
                    compactX = 0;
                    compactY = vehicleSize;
                }

                else
                {
                    f1MoveX = nearCoordinate;
                    f1MoveY = commonCoordinate + deltaShift;
                    f2MoveX = farCoordinate;
                    f2MoveY = commonCoordinate;
                    shiftX = (farCoordinate - nearCoordinate) / 2;
                    shiftY = 0;
                    compactX = vehicleSize;
                    compactY = 0;

                }
                if (isVertical && fighters.Rect.Top < helicopters.Rect.Top ||
                    !isVertical && fighters.Rect.Left < helicopters.Rect.Left)
                {
                    f1 = fighters;
                    f2 = helicopters;
                }
                else
                {
                    f1 = helicopters;
                    f2 = fighters;
                }
                // двигаем первую формацию налево или вниз, а потом - масштабируем ее
                var sMove1 = new ActionSequence(
                    f1.MoveLeftTopTo(f1MoveX, f1MoveY, Global.Game.HelicopterSpeed),
                    f1.ScaleLeftTop(factor)
                );
                Global.ActionQueue.Add(sMove1);

                // двигаем вторую формацию налево или вниз, а потом - масштабируем ее
                var sMove2 = new ActionSequence(
                    f2.MoveLeftTopTo(f2MoveX, f2MoveY, Global.Game.HelicopterSpeed),
                    f2.ScaleLeftTop(factor)
                );
                Global.ActionQueue.Add(sMove2);

                // после того, как обе формации отмасштабированы, первая формация движеться наствречу второй
                var aPenetrate1 = f1.ShiftTo(shiftX, shiftY);
                aPenetrate1.StartCondition = () => sMove1.IsFinished && sMove2.IsFinished;
                var sPenetrate1 = new ActionSequence(aPenetrate1);
                Global.ActionQueue.Add(sPenetrate1);

                // а вторая - навстречу первой до полного проникновения
                var aPenetrate2 = f2.ShiftTo(-shiftX, -shiftY);
                aPenetrate2.StartCondition = () => sMove1.IsFinished && sMove2.IsFinished;
                var sPenetrate2 = new ActionSequence(aPenetrate2);
                Global.ActionQueue.Add(sPenetrate2);

                // сплющиваем сбоку бутерброд
                var res = FormationFactory.CreateFormation(
                    () => f1.Rect.Left, () => f1.Rect.Top,
                    () => f1.Rect.Right - compactX, () => f1.Rect.Bottom - compactY);

                var sShift = new ActionSequence(res.ActionList.ToArray());
                sShift.First().StartCondition = () => sPenetrate2.IsFinished && sPenetrate1.IsFinished;
                foreach (var action in sShift)
                {
                    action.Urgent = true;
                }
                sShift.Add(res.Formation.ShiftTo(shiftX, shiftY));
                Global.ActionQueue.Add(sShift);

                // сплющиваем сбоку бутерброд
                //                res = FormationFactory.CreateFormation(
                //                    () => Math.Min(f1.Rect.Left, f2.Rect.Left),
                //                    () => Math.Min(f1.Rect.Top, f2.Rect.Top),
                //                    () => Math.Max(f1.Rect.Right, f2.Rect.Right),
                //                    () => Math.Max(f1.Rect.Bottom, f2.Rect.Bottom));
                //
                //                var sCompact = new ActionSequence(res.ActionList.ToArray());
                //                sCompact.First().StartCondition = () => sShift.IsFinished;
                //                sCompact.Add(res.Formation.ScaleCenter(0.5));
                //                sCompact.Add(res.Formation.ShiftTo(10,10));
                //                Global.ActionQueue.Add(sCompact);

            }


            // todo: убрать после экспериментов
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
