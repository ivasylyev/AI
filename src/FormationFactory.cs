using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class FormationFactory
    {
        public static Formation CreateFormation(VehicleType type)
        {
            var formation = new Formation
            {
                GroupIndex = -(int) type,
                Type = type
            };

            foreach (var keyVal in Global.MyVehicles)
            {
                if (keyVal.Value.Type == type)
                {
                    formation.Vehicles.Add(keyVal.Key, keyVal.Value);
                }
            }

            formation.Update();

            Global.Formations.Add(formation.GroupIndex, formation);
            formation.Alive = true;
            return formation;
        }

        public static Formation CreateFormation(double left, double top, double right, double bottom)
        {
            var formation = new Formation
            {
                GroupIndex = GetFreeFormationIndex()
            };

            var rect = new Rect(left, top, right, bottom);

            foreach (var keyVal in Global.MyVehicles)
            {
                if (keyVal.Value.IsInside(rect))
                {
                    formation.Vehicles.Add(keyVal.Key, keyVal.Value);
                }
            }

            formation.Update();

            SelectVehicles(formation, left, top, right, bottom);
            AssignVehicles(formation, formation.GroupIndex);

            Global.Formations.Add(formation.GroupIndex, formation);

            return formation;
        }

        private static void SelectVehicles(Formation formation, double left, double top, double right, double bottom)
        {
            var move = new Action
            {
                Action = ActionType.ClearAndSelect,
                Left = left,
                Top = top,
                Right = right,
                Bottom = bottom
            };
            Global.ActionQueue.Add(move);
        }

        private static void AssignVehicles(Formation formation, int groupIndex)
        {
            Global.ActionQueue.Add(new Action
            {
                Action = ActionType.Assign,
                Group = groupIndex,
                Callback = () => { formation.Alive = true; }
            });
        }

        public static int GetFreeFormationIndex()
        {
            return Enumerable.Range(1, 100).Except(Global.Formations.Keys).FirstOrDefault();
        }
    }
}