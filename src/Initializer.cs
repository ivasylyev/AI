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

            Global.Map.Update();
            Global.DetailMap.Update();

            UpdateMyFormations();

            UpdateEnemyFormations();

            UpdateFacilies();


            Global.ActionQueue.Update();
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

        private static void UpdateMyFormations()
        {
            if (Global.World.TickIndex == 0)
            {
                CreateAndScale(VehicleType.Tank);
                CreateAndScale(VehicleType.Ifv);
                CreateAndScale(VehicleType.Arrv);
                CreateAndScale(VehicleType.Helicopter);
                CreateAndScale(VehicleType.Fighter);
            }
            foreach (var formation in Global.MyFormations.Values)
            {
                formation.Update(Global.World.VehicleUpdates);
            }
        }

        private static void CreateAndScale(VehicleType vehicleType)
        {
            var factor = 0.5;
            var result = FormationFactory.CreateMyFormation(vehicleType);

            var scaleAction = result.Formation.ScaleCenter(factor);
            scaleAction.StartCondition = () =>
            {
                return result.ActionList.Last(a => a.ActionType == ActionType.Assign).Status ==
                       ActionStatus.Finished;
            };
            result.ActionList.Add(scaleAction);

            Global.ActionQueue.Add(new ActionSequence(result.ActionList.ToArray()));
        }

        private static void UpdateEnemyFormations()
        {
            // будем пересоздавать вражеские формации не каждый тик, а то посядем по быстродействию.
            if (Global.World.TickIndex % 10 == 0)
            {
                var allEnemyTiles = new HashSet<Tile>();
                var formations = new List<List<Tile>>();

                foreach (var tile in Global.DetailMap.Tiles)
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
            foreach (var formation in Global.EnemyFormations)
            {
                formation.Update(Global.World.VehicleUpdates);
            }
        }

        private static void AddToEnemyFormations(HashSet<Tile> allEnemyTiles, List<Tile> formationTiles,
            Tile currentTile)
        {
            if (allEnemyTiles.Contains(currentTile))
            {
                return;
            }

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
                var id = facility.SelectedAsTargetForGroup;
                if (id.HasValue)
                {
                    if (!Global.MyFormations.ContainsKey(id.Value))
                    {
                        facility.SelectedAsTargetForGroup = null;
                    }
                    else
                    {
                        var myFormation = Global.MyFormations[id.Value];
                        if (!myFormation.Alive || !myFormation.Vehicles.Any())
                        {
                            facility.SelectedAsTargetForGroup = null;
                        }
                    }
                }
            }
        }
    }
}