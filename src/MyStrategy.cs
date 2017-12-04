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
                Global.EnemyFormations.Count > 0)
            {
                foreach (var formation in Global.MyFormations.Values.Where(f => f.Alive && f.Vehicles.Any()))
                {
                    if (TacticalActions.OccupyFacilities(formation))
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