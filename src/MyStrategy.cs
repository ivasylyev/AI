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

                TacticalActions.NuclearStrike();

                TacticalActions.RunAwayFromNuclearStrike();

                Anticollision();

                AttackOrDefenceOrOccupy();


                SetupProduction();

                AssignNewGroups();

                Global.ActionQueue.Process();
            }
//            catch (Exception ex)
//            {
//                var exception = ex;
//            }
        }


        private void Anticollision()
        {
            if (Global.World.TickIndex > 120 && Global.World.TickIndex % 20 == 0)
            {
                TacticalActions.Anticollision();
            }
        }


        private void SetupProduction()
        {
            if (Global.World.TickIndex % 10 == 0)
            {
                var myFightersCount = Global.MyFighters.Vehicles.Count;
                var enemyAirCount = Global.EnemyFighters.Count() + Global.EnemyHelicopters.Count();
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
                TacticalActions.CreateProducedFormation();
            }
        }

        private void AttackOrDefenceOrOccupy()
        {
            if (Global.World.TickIndex % 60 == 0 &&
                Global.EnemyFormations.Count > 0 )
            {
                foreach (var formation in Global.MyFormations.Values.Where(f => f.Alive && f.Vehicles.Any()))
                {
                    if (TacticalActions.OccupyFacilities(formation))
                    {
                        continue;
                    }
                    if (formation == Global.MyHelicopters && Global.EnemyFighters.Count() > 30)
                    {
                        MyFormation foundAllyGround = null;
                        if (Global.MyIfvs.Alive && Global.MyIfvs.Vehicles.Count > 30)
                        {
                            foundAllyGround = Global.MyIfvs;
                        }
                        else if (Global.MyArrvs.Alive && Global.MyArrvs.Vehicles.Count > 30)
                        {
                            foundAllyGround = Global.MyArrvs;
                        }
                        if (foundAllyGround != null)
                        {
                            var actionMove = formation.MoveCenterTo(Global.MyIfvs.Center);
                            var sequence = new ActionSequence(actionMove);
                            Global.ActionQueue.Add(sequence);
                            continue;
                        }
                    }
                    if (formation == Global.MyFighters)
                    {
                        if (Global.EnemyFighters.Count() > 10)
                        {
                            var enemy = FormationFactory.CreateEnemyFormation(Global.EnemyFighters);
                            TacticalActions.MakeAttackOrder(formation, enemy, false);
                            continue;
                        }
                        if (Global.EnemyHelicopters.Count() > 10)
                        {
                            var enemy = FormationFactory.CreateEnemyFormation(Global.EnemyHelicopters);
                            TacticalActions.MakeAttackOrder(formation, enemy, false);
                            continue;
                        }
                        if (Global.MyArrvs.Alive && Global.MyArrvs.Vehicles.Count > 30)
                        {
                            var actionMove = formation.MoveCenterTo(Global.MyArrvs.Center);
                            var sequence = new ActionSequence(actionMove);
                            Global.ActionQueue.Add(sequence);
                            continue;
                        }
                    }


                    var oneUnitDanger = new Dictionary<EnemyFormation, double>();
                    var wholeDanger = new Dictionary<EnemyFormation, double>();
                    foreach (var enemy in Global.EnemyFormations)
                    {
                        var dangerForEnemy = formation.DangerFor(enemy);
                        var dangerForMe = enemy.DangerFor(formation);
                        oneUnitDanger.Add(enemy, dangerForEnemy - dangerForMe);

                        wholeDanger.Add(enemy, dangerForEnemy * formation.Count - dangerForMe * enemy.Count);
                    }


                    // todo: давать правильную команду 

                    // выбирать также по расстоянию
                    EnemyFormation target = null;
                    var targetPair = wholeDanger.OrderByDescending(kv => kv.Value).First();
                    if (targetPair.Value > 0)
                    {
                        target = targetPair.Key;
                    }
                    if (target == null)
                    {
                        targetPair = oneUnitDanger.OrderByDescending(kv => kv.Value).First();
                        if (targetPair.Value > 0)
                        {
                            target = targetPair.Key;
                        }
                    }
                    if (target != null)
                    {
                        TacticalActions.MakeAttackOrder(formation, target, false);
                    }
                }
            }
        }
    }
}