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

                TacticalActions.FinalSpread();

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

        static VehicleType neededType = VehicleType.Fighter;

        private void SetupProduction()
        {
            if (Global.World.TickIndex % 10 == 0)
            {
                var myFightersCount = Global.MyFighters.Vehicles.Count;
                var myHelisCount = Global.MyHelicopters.Vehicles.Count;

                var enemyFightersCount = Global.EnemyFighters.Count();
                var enemyHeliCount = Global.EnemyHelicopters.Count();


                if (myFightersCount < enemyFightersCount + enemyHeliCount)
                {
                    neededType = VehicleType.Fighter;
                }
                else if (myHelisCount < enemyHeliCount)
                {
                    neededType = VehicleType.Helicopter;
                }
                else
                {
                    if (Global.World.TickIndex % 1800 == 0)
                    {
                        neededType = VehicleType.Tank;
                    }
                    if (Global.World.TickIndex % 1800 == 600)
                    {
                        neededType = VehicleType.Arrv;
                    }
                    if (Global.World.TickIndex % 1800 == 1200)
                    {
                        neededType = VehicleType.Ifv;
                    }
                }

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
                Global.EnemyFormations.Count > 0)
            {
                foreach (var formation in Global.MyFormations.Values.Where(f => f.Alive && f.Vehicles.Any()))
                {
                    if (TacticalActions.OccupyFacilities(formation, false))
                    {
                        continue;
                    }
                    if (TacticalActions.Attack(formation))
                    {
                        continue;
                    }
                    var i = 0;
                }
            }
        }
    }
}