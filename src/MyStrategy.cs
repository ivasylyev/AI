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

                Anticollision();

                AttackTest();


//                OccupyFacilities();
//
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
                var formations = TacticalActions.CreateProducedFormation();

                foreach (var formation in formations)
                {
                    MakeAttackOrder(formation);
                }
            }
        }

        private void AttackTest()
        {
            if (Global.World.TickIndex % 120 == 0)
            {
                foreach (var formation in Global.MyFormations.Values)
                {
                    // todo: давать правильную команду 
                    MakeAttackOrder(formation);
                }
            }
        }

        private static void MakeAttackOrder(MyFormation formation)
        {
            if (formation.Alive && !formation.Busy)
            {
                var enemy = Global.EnemyFormations.OrderBy(f => f.Center.SqrDistance(formation.Center))
                    .FirstOrDefault();
                var pointToMove = enemy == null
                    ? Point.EndOfWorld / 2
                    : enemy.Center;

                var distance = pointToMove.Distance(formation.Center);
                if (distance > 200)
                {
                    pointToMove = (formation.Center * 3 + pointToMove) / 4;
                }
                else if (distance > 100)
                {
                    pointToMove = (formation.Center + pointToMove) / 2;
                }

                ActionSequence sequence;
                var actionMove = formation.MoveCenterTo(pointToMove);
                if (formation.Density < 0.015 || distance > 200)
                {
                    var actionScale = formation.ScaleCenter(0.1);
                    sequence = new ActionSequence(actionScale, actionMove);
                }
                else
                {
                    sequence = new ActionSequence(actionMove);
                }


                Global.ActionQueue.Add(sequence);
            }
        }

        private void FollowTest()
        {
//            if (Global.World.TickIndex % 120 == 0)
//            {
//                if (Global.MyAirFormation.Alive)
//                {
//                    var action = Global.MyArrvs.MoveCenterTo(Global.MyAirFormation.Center);
//                    var sequence = new ActionSequence(action);
//                    Global.ActionQueue.Add(sequence);
//                }
//            }
        }
    }
}