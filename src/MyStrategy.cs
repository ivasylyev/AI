using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
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

                AttackTest();


                OccupyFacilities();

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


        private void OccupyFacilities()
        {
            if (Global.World.TickIndex > 120 && Global.World.TickIndex % 60 == 0)
            {
                TacticalActions.OccupyFacilities(Global.MyIfvs);
                TacticalActions.OccupyFacilities(Global.MyTanks);
                TacticalActions.OccupyFacilities(Global.MyArrvs);
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

        private void AttackTest()
        {
            if (Global.World.TickIndex % 120 == 0 && Global.EnemyFormations.Count > 0)
            {
                foreach (var formation in Global.MyFormations.Values)
                {
                    if (formation.Alive && formation.IsAllAeral)
                    {
                        Dictionary<EnemyFormation, double> oneUnitDanger = new Dictionary<EnemyFormation, double>();
                        Dictionary<EnemyFormation, double> wholeDanger = new Dictionary<EnemyFormation, double>();
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
}