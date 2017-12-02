using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed class MyStrategy : IStrategy
    {
        public void Move(Player me, World world, Game game, Move move)
        {
            //try
            {
                Initializer.Init(me, world, game, move);

                OccupyFacilities();

                SetupProduction();

                AssignNewGroups();
                //SelectionTest2();

                LongMoveTest();

                Global.ActionQueue.Process();
            }
//            catch (Exception ex)
//            {
//                var exception = ex;
//            }
        }


        private void LongMoveTest()
        {
            if (Global.World.TickIndex > 50 && Global.World.TickIndex % 20 == 0)
            {
                List<int> processedKeys = new List<int>();
                foreach (var key1 in Global.MyFormations.Keys)
                {
                    var f1 = Global.MyFormations[key1];
                    if (f1.Alive && !Global.IgnoreCollisionGroupIndexes.Contains(key1))
                    {
                        foreach (var key2 in Global.MyFormations.Keys.Where(k => k != key1))
                        {
                            var f2 = Global.MyFormations[key2];
                            if (f2.Alive && 
                                !Global.IgnoreCollisionGroupIndexes.Contains(key2) &&
                                (f1.IsMixed || f2.IsMixed || f1.IsAllAeral ==f2.IsAllAeral)
                                )
                            {
                                var distBetweenCenters = f1.Rect.Center.SqrDistance(f2.Rect.Center);
                                if (distBetweenCenters < (f1.Rect.SqrDiameter + f2.Rect.SqrDiameter)/2)
                                {
                                    var deltaX = f1.Center.X - f2.Center.X;
                                    var deltaY = f1.Center.Y - f2.Center.Y;
                                    double l = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                                    deltaX = 20 * deltaX / l;
                                    deltaY = 20 * deltaY / l;

                                    if (!processedKeys.Contains(key1))
                                    {
                                        var move1 = f1.ShiftTo(deltaX, deltaY);
                                        TacticalActions.PauseExecuteAndContinue(f1, move1);
                                    }
                                    if (!processedKeys.Contains(key2))
                                    {
                                        var move2 = f2.ShiftTo(-deltaX, -deltaY);
                                        TacticalActions.PauseExecuteAndContinue(f2, move2);
                                    }
                                    processedKeys.Add(key1);
                                    processedKeys.Add(key2);
                                }
                            }
                        }
                    }
                }


                //                TacticalActions.PauseExecuteAndContinue(formation, actionMove, actionScale);
            }
        }


        private void OccupyFacilities()
        {
            if (Global.World.TickIndex % 10 == 0)
            {
                foreach (var key1 in Global.MyFormations.Keys)
                {
                    foreach (var key2 in Global.MyFormations.Keys.Where(k => k != key1))
                    {
                        var f1 = Global.MyFormations[key1];
                        var f2 = Global.MyFormations[key2];
                        var distBetweenCenters = f1.Rect.Center.SqrDistance(f2.Rect.Center);
                        if (distBetweenCenters < (f1.Rect.SqrDiameter + f1.Rect.SqrDiameter) / 2)
                        {
                        }
                    }
                }
            }

            TacticalActions.OccupyFacilities(Global.MyIfvs);
        }


        private void SetupProduction()
        {
            if (Global.World.TickIndex % 10 == 0)
            {
                var myFightersCount = Global.MyFighters.Vehicles.Count;
                var enemyAirCount = Global.EnemyFighters.Vehicles.Count + Global.EnemyHelicopters.Vehicles.Count;
                var neededType = myFightersCount < enemyAirCount ? VehicleType.Fighter : VehicleType.Helicopter;

                foreach (var facility in Global.MyFacilities)
                {
                    TacticalActions.SetupProductionIfNeed(facility, neededType);
                }
            }
        }

        private void AssignNewGroups()
        {
            if (Global.World.TickIndex % 120 == 0)
            {
                var formations = TacticalActions.CreateProducedFormation();

                foreach (var formation in formations)
                {
                    // todo: давать правильную команду 
                    var action = formation.MoveCenterTo(Global.EnemyArrvs.Center.X, Global.EnemyArrvs.Center.Y);
                    var sequence = new ActionSequence(action);
                    Global.ActionQueue.Add(sequence);
                }
            }
        }


        private void SelectionTest2()
        {
            // todo: надо бы делить все по тикам, чтобы не накладывалось
            //if (Global.World.TickIndex % 60 == 0 && Global.MyAirFormation.Alive)
            if (Global.World.TickIndex % 60 == 0)
            {
                var type = VehicleType.Helicopter;
                var enemyX = Global.EnemyVehicles.Values.Where(v => v.Type == type).Average(v => v.X);
                var enemyY = Global.EnemyVehicles.Values.Where(v => v.Type == type).Average(v => v.Y);


                // todo: надо бы предсказывать, куда двигаться
                var formation = Global.MyFighters;
                var actionMove = formation.MoveCenterTo(formation.Center.X / 2 + enemyX, enemyY);

                if (formation.ExecutingSequence == null)
                {
                    var actionCompact = formation.ScaleCenter(0.1);
                    var sequence = new ActionSequence(actionCompact, actionMove);
                    Global.ActionQueue.Add(sequence);
                }
                else
                {
                    var enemyIsNear = formation.Rect.Center.SqrDistance(enemyX, enemyY) < 300 * 300;

                    /*
                    var actionMove2 = formation.MoveCenterTo(600, 600);
                    actionMove2.StartCondition = () => true;
                    Global.MyFighters.ExecutingSequence.Add(actionMove2);
                    */
                    if (enemyIsNear || Global.World.TickIndex % 180 == 0)
                    {
                        /*
                        formation.ExecutingSequence.AbortExecutingAction();

                        //actionMove.MaxSpeed = 0.5;
                        if (enemyIsNear)
                        {
                            //var actionMove2 = formation.MoveCenterTo((9*formation.Center.X + enemyX)/10, (9*formation.Center.Y+ enemyY)/10);
                            var actionMove2 = formation.MoveCenterTo(600, 600);
                            actionMove2.StartCondition = () => true;
                            Global.MyFighters.ExecutingSequence.Add(actionMove2);
                        }
                        else
                        {
                            actionMove.StartCondition = () => true;
                        }
                        formation.ExecutingSequence.Add(actionMove);
                        */
                    }
                }


                //  TacticalActions.SplitFormation(Global.MyIfvs, 50);
            }


            if (Global.World.TickIndex == 10)
            {
                //  TacticalActions.SplitFormation(Global.MyIfvs, 50);
            }
//            if (Global.MyAirFormation.Alive)
//            {
//            }
            // todo: убрать после экспериментов

            //            if (Global.World.TickIndex % 10 == 0)
            //            {
            //                foreach (var key1 in Global.MyFormations.Keys)
            //                {
            //                    foreach (var key2 in Global.MyFormations.Keys.Where(k=>k!=key1))
            //                    {
            //                        var f1 = Global.MyFormations[key1];
            //                        var f2 = Global.MyFormations[key2];
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