using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MyFormation : Formation
    {
        public readonly List<MyFormation> Children;

        public MyFormation()
        {
            WaitUntilIndex = -1;
            Alive = false;
            Children = new List<MyFormation>();
            BusyCondition = () => Global.World.TickIndex < WaitUntilIndex || !IsStanding;
        }


        public Action ExecutingAction { get; set; }
        public ActionSequence ExecutingSequence { get; set; }

        public int WaitUntilIndex { get; set; }

        public Func<bool> BusyCondition { get; set; }
        public bool Busy => BusyCondition != null && BusyCondition();
        public bool Alive { get; set; }


        public bool IsEnemyNear()
        {
            var near = Global.EnemyVehicles.Values
                .Count(
                    enemy => Global.MyVehicles.Values.Where(v => v.Type != VehicleType.Fighter)
                        .Any(i => i.SqrDistance(enemy) < i.VisionRange * i.VisionRange));
            return near > 20;
        }

        public Action GetSelectionAction()
        {
            var action = new Action(this) {ActionType = ActionType.ClearAndSelect};
            if (GroupIndex > 0)
            {
                action.Group = GroupIndex;
            }
            else
            {
                action.VehicleType = Type;
                action.GetRight = () => Global.World.Width;
                action.GetBottom = () => Global.World.Height;
            }
            return action;
        }

        public override void Update(IEnumerable<VehicleUpdate> updates = null)
        {
            base.Update(updates);
            if (Alive && !Vehicles.Any())
            {
                Alive = false;
            }
        }

        public override string ToString()
        {
            return $"{Type}, Busy:{Busy}, Rect:{Rect}";
        }
    }
}