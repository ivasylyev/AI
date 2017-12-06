using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class FormationFactory
    {
        public static EnemyFormation CreateEnemyFormation(IEnumerable<VehicleWrapper> vehicles)
        {
            var formation = new EnemyFormation();
            foreach (var v in vehicles)
            {
                formation.Vehicles.Add(v.Id, v);
            }

            formation.Update();
            Global.EnemyFormations.Add(formation);
            return formation;
        }

        public static EnemyFormation CreateEnemyFormation(IEnumerable<Tile> tiles)
        {
            var vehicles = GetVehiclesFromTiles(tiles);
            return CreateEnemyFormation(vehicles);
        }

        private static IEnumerable<VehicleWrapper> GetVehiclesFromTiles(IEnumerable<Tile> tiles)
        {
            foreach (var tile in tiles)
            {
                foreach (var enemy in tile.Enemies)
                {
                    yield return enemy;
                }
            }
        }

        public static FormationResult CreateMyFormation(VehicleType type)
        {
            var formation = new MyFormation
            {
                GroupIndex = 1 + (int) type,
                Alive = false,
                Type = type
            };

            var left = Global.MyVehicles.Values.Where(v => v.Type == type).Min(v => v.X);
            var top = Global.MyVehicles.Values.Where(v => v.Type == type).Min(v => v.Y);
            var right = Global.MyVehicles.Values.Where(v => v.Type == type).Max(v => v.X);
            var bottom = Global.MyVehicles.Values.Where(v => v.Type == type).Max(v => v.Y);

            var list = SelectAndAssignVehicles(formation, () => left, () => top, () => right, () => bottom,
                formation.GroupIndex);

            Global.MyFormations.Add(formation.GroupIndex, formation);

            return new FormationResult
            {
                Formation = formation,
                GroupIndex = formation.GroupIndex,
                ActionList = list
            };
        }

        public static FormationResult CreateMyFormation(double left, double top, double right, double bottom)
        {
            return CreateMyFormation(() => left, () => top, () => right, () => bottom);
        }

        public static FormationResult CreateMyFormation(Func<double> left, Func<double> top,
            Func<double> right, Func<double> bottom)
        {
            var formation = new MyFormation
            {
                GroupIndex = GetMyFreeFormationIndex(),
                Alive = false
            };


            var list = SelectAndAssignVehicles(formation, left, top, right, bottom, formation.GroupIndex);

            Global.MyFormations.Add(formation.GroupIndex, formation);

            return new FormationResult
            {
                Formation = formation,
                GroupIndex = formation.GroupIndex,
                ActionList = list
            };
        }

        private static List<Action> SelectAndAssignVehicles(MyFormation formation,
            Func<double> left, Func<double> top, Func<double> right, Func<double> bottom, int groupIndex)
        {
            var selectAction = new Action
            {
                ActionType = ActionType.ClearAndSelect,
                GetLeft = left,
                GetTop = top,
                GetRight = right,
                GetBottom = bottom,
            };
            selectAction.Callback = () =>
            {
                var rect = new Rect(left(), top(), right(), bottom());

                foreach (var keyVal in Global.MyVehicles)
                {
                    if (keyVal.Value.IsInside(rect))
                    {
                        formation.Vehicles.Add(keyVal.Key, keyVal.Value);
                    }
                }

                formation.Update();

                selectAction.Status = ActionStatus.Finished;
            };

            var assignAction = new Action
            {
                ActionType = ActionType.Assign,
                Group = groupIndex,
           //     IsUrgent = true
            };
            assignAction.Callback = () =>
            {
                formation.Alive = true;
                assignAction.Status = ActionStatus.Finished;
            };

            return new List<Action> {selectAction, assignAction};
        }


        public static int GetMyFreeFormationIndex()
        {
            return Enumerable.Range(1, 100).Except(Global.MyFormations.Keys).FirstOrDefault();
        }
    }
}