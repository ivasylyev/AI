﻿using System;
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
            var formation = Global.MyFighters;
            if (Global.World.TickIndex == 10)
            {
                ActionSequence sequence = new ActionSequence(formation.MoveCenterTo(300, 300));
                Global.ActionQueue.Add(sequence);
            }
            if (Global.World.TickIndex == 70)
            {
                var actionMove = formation.MoveCenterTo(formation.Center.X + 50, formation.Center.Y - 50);
                TacticalActions.PauseExecuteAndContinue(formation, actionMove);
            }
        }


        private void OccupyFacilities()
        {
            if (Global.World.TickIndex % 60 == 0)
            {
                TacticalActions.OccupyFacilities(Global.MyIfvs);
            }
        }


        private void SetupProduction()
        {
            if (Global.World.TickIndex % 10 == 0)
            {
                var myFightersCount = Global.MyFighters.Vehicles.Count;
                var enemyAirCount = Global.EnemyFighters.Vehicles.Count + Global.EnemyHelicopters.Vehicles.Count;
                VehicleType neededType = myFightersCount < enemyAirCount ? VehicleType.Fighter : VehicleType.Helicopter;

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
                var actionMove = formation.MoveCenterTo((formation.Center.X) / 2 + enemyX, enemyY);

                if (formation.ExecutingSequence == null)
                {
                    var actionCompact = formation.ScaleCenter(0.1);
                    ActionSequence sequence = new ActionSequence(actionCompact, actionMove);
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