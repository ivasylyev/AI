using System;
using System.Collections.Generic;
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

        public static FormationResult CreateFormation(double left, double top, double right, double bottom)
        {
            return CreateFormation(() => left, () => top, () => right, () => bottom);
        }

        public static FormationResult CreateFormation(Func<double> left, Func<double> top, 
            Func<double> right,Func<double> bottom)
        {
            var formation = new Formation
            {
                GroupIndex = GetFreeFormationIndex(),
                Alive = false
            };


            var list = SelectAndAssignVehicles(formation, left, top, right, bottom, formation.GroupIndex);

            Global.Formations.Add(formation.GroupIndex, formation);

            return new FormationResult
            {
                Formation = formation,
                GroupIndex = formation.GroupIndex,
                ActionList = list
            };
        }

        private static List<Action> SelectAndAssignVehicles(Formation formation,
            Func<double> left, Func<double> top, Func<double> right, Func<double> bottom, int groupIndex)
        {
            var selectAction = new Action
            {
                ActionType = ActionType.ClearAndSelect,
                GetLeft = left,
                GetTop = top,
                GetRight = right,
                GetBottom = bottom
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
                Group = groupIndex
            };
            assignAction.Callback = () =>
            {
                formation.Alive = true;
                assignAction.Status = ActionStatus.Finished;
            };

            return new List<Action> {selectAction, assignAction};
        }


        public static int GetFreeFormationIndex()
        {
            return Enumerable.Range(1, 100).Except(Global.Formations.Keys).FirstOrDefault();
        }
    }
}