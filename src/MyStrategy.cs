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
            if (Global.World.TickIndex % 120 == 0)
            {
                foreach (var formation in Global.MyFormations.Values)
                {
                    if (formation.Alive && formation.IsAllAeral)
                    {
                        Dictionary<Formation, double> dangerForEnemy = new Dictionary<Formation, double>();
                        Dictionary<Formation, double> dangerForMe = new Dictionary<Formation, double>();
                        foreach (var enemy in Global.EnemyFormations)
                        {
                            dangerForEnemy.Add(enemy, formation.DangerFor(enemy));
                            dangerForMe.Add(enemy, formation.DangerFor(enemy));
                        }
                        // todo: давать правильную команду 
                        EnemyFormation enemy1 = Global.EnemyFormations
                            .OrderBy(f => f.Center.SqrDistance(formation.Center))
                            .FirstOrDefault();


                        TacticalActions.MakeAttackOrder(formation, enemy1, false);
                    }
                }
            }
        }
    }
}