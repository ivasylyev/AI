using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed class MyNewStrategy 
    {
        public void Move(Player me, World world, Game game, Move move)
        {
            //try
            {
                Initializer.Init(me, world, game, move);

                TacticalActions.NuclearStrike();

                TacticalActions.RunAwayFromNuclearStrike();


               

                AttackOrDefenceOrOccupy();


                SetupProduction();

                AssignNewGroups();

                Anticollision();

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
            if (Global.World.TickIndex > 120 && Global.World.TickIndex % 30 == 0)
            {
                TacticalActions.Anticollision();
            }
        }

        static VehicleType neededType = VehicleType.Fighter;

        private void SetupProduction()
        {
            if (Global.World.TickIndex % 10 == 0)
            {
                foreach (var facility in Global.NotMyFacilities)
                {
                    facility.InitialTickIndex = Global.World.TickIndex;
                }
                foreach (var facility in Global.MyFacilities)
                {
                    TacticalActions.SetupProductionIfNeed(facility);
                }
            }
        }

        private void AssignNewGroups()
        {
            if (Global.World.TickIndex % 30 == 0)
            {
                TacticalActions.CreateProducedFormation();
            }
        }

        private void AttackOrDefenceOrOccupy()
        {
            if (Global.World.TickIndex % 60 == 0 )
            {
                foreach (var formation in Global.MyFormations.Values.Where(f => f.Alive && f.Vehicles.Any()))
                {
                   
                    if (TacticalActions.OccupyFacilities(formation, false))
                    {
                        continue;
                    }

                    if (Global.EnemyFormations.Count > 0 && TacticalActions.Attack(formation))
                    {
                        continue;
                    }
                    var i = 0;
                }
            }
        }
    }
}