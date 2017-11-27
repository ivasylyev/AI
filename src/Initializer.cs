using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Initializer
    {
        public static void Init(Player me, World world, Game game, Move move)
        {
            InitGlobal(me, world, game, move);

            CreateVehicles();

            UpdateVehicles();

            UpdateFormations();
        }

        private static void InitGlobal(Player me, World world, Game game, Move move)
        {
            Global.Me = me;
            Global.World = world;
            Global.Game = game;
            Global.Move = move;
        }

        private static void CreateVehicles()
        {
            foreach (var newVehicle in Global.World.NewVehicles)
            {
                VehicleWrapper wrapper = newVehicle;
                Global.AllVehicles.Add(wrapper.Id, wrapper);
                if (wrapper.PlayerId == Global.Me.Id)
                {
                    Global.MyVehicles.Add(wrapper.Id, wrapper);
                }
                else
                {
                    Global.EnemyVehicles.Add(wrapper.Id, wrapper);
                }
            }
        }

        private static void UpdateVehicles()
        {
            foreach (var vehicleUpdate in Global.World.VehicleUpdates)
            {
                var vehickeId = vehicleUpdate.Id;
                if (vehicleUpdate.Durability > 0)
                {
                    Global.AllVehicles[vehickeId].Update(vehicleUpdate);
                }
                else
                {
                    Global.AllVehicles.Remove(vehickeId);
                    Global.MyVehicles.Remove(vehickeId);
                    Global.EnemyVehicles.Remove(vehickeId);
                }
            }
            foreach (var vehicle in Global.AllVehicles.Values)
            {
                if (vehicle.TickIndex != Global.World.TickIndex)
                {
                    vehicle.Update();
                }
            }
        }

        private static void UpdateFormations()
        {
            if (Global.World.TickIndex == 0)
            {
                FormationFactory.CreateFormation(VehicleType.Fighter);
                FormationFactory.CreateFormation(VehicleType.Helicopter);
                FormationFactory.CreateFormation(VehicleType.Arrv);
                FormationFactory.CreateFormation(VehicleType.Ifv);
                FormationFactory.CreateFormation(VehicleType.Tank);
            }
            foreach (var formation in Global.Formations.Values)
            {
                formation.Update(Global.World.VehicleUpdates);
            }
        }
    }
}