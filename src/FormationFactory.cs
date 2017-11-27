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
            return formation;
        }

        public static Formation CreateFormation(double left, double top, double right, double bottom, int groupIndex)
        {
            var formation = new Formation
            {
                GroupIndex = groupIndex
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

            SelectVehicles(left, top, right, bottom);
            AssignVehicles(groupIndex);

            Global.Formations.Add(formation.GroupIndex, formation);

            return formation;
        }

        private static void SelectVehicles(double left, double top, double right, double bottom)
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

        private static void AssignVehicles(int groupIndex)
        {
            Global.ActionQueue.Add(new Action
            {
                Action = ActionType.Assign,
                Group = groupIndex
            });
        }
    }
}