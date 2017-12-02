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

                Anticollision();

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
            }
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
    }
}