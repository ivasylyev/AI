using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Formation
    {
        public readonly List<Formation> Children;

        public Formation()
        {
            WaitUntilIndex = -1;
            Alive = false;
            Vehicles = new Dictionary<long, VehicleWrapper>();
            Children = new List<Formation>();
            BusyCondition = () => { return Global.World.TickIndex < WaitUntilIndex || !IsStanding; };
        }

        public VehicleType Type { get; set; }

        public int GroupIndex { get; set; }

        public Dictionary<long, VehicleWrapper> Vehicles { get; }

        public Rect Rect { get; set; }

        public Point MassCenter { get; set; }

        public int WaitUntilIndex { get; set; }

        public Func<bool> BusyCondition { get; set; }

        public bool Busy => BusyCondition != null && BusyCondition();
        public bool Alive { get; set; }

        public double Density
        {
            get
            {
                if (Vehicles.Count == 0)
                {
                    return 0;
                }
                var radius =
                    Rect.Center.Distance(Vehicles.Values.OrderBy(i => Rect.Center.Distance(i)).Last());
                var sq = Math.PI * radius * radius;
                return Vehicles.Count / sq;
            }
        }

        public Point AvgSpeed => new Point(AvgSpeedX, AvgSpeedY);

        public double AvgSpeedX
        {
            get
            {
                if (Vehicles.Count == 0)
                {
                    return 0;
                }
                return Vehicles.Values.Average(v => v.SpeedX);
            }
        }

        public double AvgSpeedY
        {
            get
            {
                if (Vehicles.Count == 0)
                {
                    return 0;
                }
                return Vehicles.Values.Average(v => v.SpeedY);
            }
        }

        public bool IsStanding => !Vehicles.Any() || Vehicles.Values.All(v => v.IsStanding);

        public bool IsNear()
        {
            var near = Global.EnemyVehicles.Values
                .Count(
                    enemy => Global.MyVehicles.Values.Where(v => v.Type != VehicleType.Fighter)
                        .Any(i => i.SqrDistance(enemy) < i.VisionRange * i.VisionRange));
            return near > 20;
        }

        public void Update(IEnumerable<VehicleUpdate> updates = null)
        {
            if (updates != null)
            {
                foreach (var update in updates)
                {
                    if (update.Durability <= 0)
                    {
                        Vehicles.Remove(update.Id);
                    }
                }
            }
            Rect = new Rect(Vehicles.Values);
            MassCenter = Vehicles.Any()
                ? new Point(Vehicles.Values.Average(i => i.X), Vehicles.Values.Average(i => i.Y))
                : Point.Zero;
        }


        public ActionSequence Split(int runFromCenter = 0)
        {
            var child = FormationFactory.CreateFormation(Rect.Left, Rect.Top, MassCenter.X, MassCenter.Y);
            Children.Add(child.Formation);
            var sequence = new ActionSequence(child.ActionList.ToArray());
            if (runFromCenter > 0)
            {
                sequence.Add(child.Formation.ShiftTo(-runFromCenter, -runFromCenter));
            }

            child = FormationFactory.CreateFormation(MassCenter.X, Rect.Top, Rect.Right, MassCenter.Y);
            sequence.AddRange(child.ActionList);
            Children.Add(child.Formation);
            if (runFromCenter > 0)
            {
                sequence.Add(child.Formation.ShiftTo(runFromCenter, -runFromCenter));
            }

            child = FormationFactory.CreateFormation(Rect.Left, MassCenter.Y, MassCenter.X, Rect.Bottom);
            sequence.AddRange(child.ActionList);
            Children.Add(child.Formation);
            if (runFromCenter > 0)
            {
                sequence.Add(child.Formation.ShiftTo(-runFromCenter, runFromCenter));
            }
            child = FormationFactory.CreateFormation(MassCenter.X, MassCenter.Y, Rect.Right, Rect.Bottom);
            sequence.AddRange(child.ActionList);
            Children.Add(child.Formation);
            if (runFromCenter > 0)
            {
                sequence.Add(child.Formation.ShiftTo(runFromCenter, runFromCenter));
            }
            return sequence;
        }

        public Action MoveCenterTo(double x, double y, double maxSpeed = 10)
        {
            var action = new Action(this)
            {
                ActionType = ActionType.Move,
                GetX = () => x - Rect.Left,
                GetY = () => y - Rect.Top,
                MaxSpeed = maxSpeed
            };
            return action;
        }

        public Action MoveLeftTopTo(double x, double y, double maxSpeed = 10)
        {
            var action = new Action(this)
            {
                ActionType = ActionType.Move,
                GetX = () => x - MassCenter.X,
                GetY = () => y - MassCenter.Y,
                MaxSpeed = maxSpeed
            };
            return action;
        }

        public Action ShiftTo(double x, double y, double maxSpeed = 10)
        {
            var action = new Action(this)
            {
                ActionType = ActionType.Move,
                GetX = () => x,
                GetY = () => y,
                MaxSpeed = maxSpeed
            };
            return action;
        }

        public Action ScaleCenter(double factor)
        {
            var action = new Action(this)
            {
                ActionType = ActionType.Scale,
                Factor = factor,
                GetX = () => MassCenter.X,
                GetY = () => MassCenter.Y,
            };
            return action;
        }
        public Action ScaleLeftTop(double factor)
        {
            var action = new Action(this)
            {
                ActionType = ActionType.Scale,
                Factor = factor,
                GetX = () => Rect.Left,
                GetY = () => Rect.Top,
            };
            return action;
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
                action.GetRight = ()=> Global.World.Width;
                action.GetBottom  = () => Global.World.Height;
            }
            return action;
        }

        public override string ToString()
        {
            return $"{Type}, Busy:{Busy}, Rect:{Rect}";
        }
    }
}