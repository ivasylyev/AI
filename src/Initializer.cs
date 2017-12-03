using System.Collections.Generic;
using System.Linq;
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

            UpdateFacilies();

            Global.Map.Update();
            Global.DetailMap.Update();

            BuildEnemyFormationMap();

            UpdateFormations();


            Global.ActionQueue.Update();

            if (Global.World.TickIndex == 0)
            {
                // Global.MyAirFormation = TacticalActions.CreateAirFormation();
                TacticalActions.CompactGroundFormations();
            }
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
                Global.ActionQueue.Add(new ActionSequence(
                    FormationFactory.CreateMyFormation(VehicleType.Arrv).ActionList.ToArray()));
                Global.ActionQueue.Add(new ActionSequence(
                    FormationFactory.CreateMyFormation(VehicleType.Ifv).ActionList.ToArray()));
                Global.ActionQueue.Add(new ActionSequence(
                    FormationFactory.CreateMyFormation(VehicleType.Tank).ActionList.ToArray()));
                Global.ActionQueue.Add(new ActionSequence(
                    FormationFactory.CreateMyFormation(VehicleType.Fighter).ActionList.ToArray()));
                Global.ActionQueue.Add(new ActionSequence(
                    FormationFactory.CreateMyFormation(VehicleType.Helicopter).ActionList.ToArray()));
            }
            foreach (var formation in Global.MyFormations.Values)
            {
                formation.Update(Global.World.VehicleUpdates);
            }
        }

        private static void BuildEnemyFormationMap()
        {
            HashSet<Tile> allEnemyTiles = new HashSet<Tile>();
            List<List<Tile>> formations = new List<List<Tile>>();


            foreach (Tile tile in Global.DetailMap.Tiles)
            {
                if (tile.Enemies.Count != 0 && !allEnemyTiles.Contains(tile))
                {
                    var formationTiles = new List<Tile>();
                    AddToEnemyFormations(allEnemyTiles, formationTiles, tile);
                    formations.Add(formationTiles);
                }
            }

            Global.EnemyFormations.Clear();
            foreach (var formation in formations)
            {
                FormationFactory.CreateEnemyFormation(formation);
            }
        }

        private static void AddToEnemyFormations(HashSet<Tile> allEnemyTiles, List<Tile> formationTiles,
            Tile currentTile)
        {
            if (allEnemyTiles.Contains(currentTile))
                return;

            allEnemyTiles.Add(currentTile);
            if (currentTile.Enemies.Count != 0)
            {
                formationTiles.Add(currentTile);
                foreach (var neighbor in currentTile.Neighbors)
                {
                    AddToEnemyFormations(allEnemyTiles, formationTiles, neighbor);
                }
            }
        }


        private static void UpdateFacilies()
        {
            foreach (var facility in Global.MyFacilities)
            {
                facility.SelectedAsTargetForGroup = null;
            }
        }
    }
}