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
            BusyCondition = () => Global.World.TickIndex < WaitUntilIndex || !IsStanding  ;
        }

        public VehicleType Type { get; set; }

        public int GroupIndex { get; set; }

        public Dictionary<long, VehicleWrapper> Vehicles { get; }

        public Rect Rectangle { get; set; }

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
                    Rectangle.Center.Distance(Vehicles.Values.OrderBy(i => Rectangle.Center.Distance(i)).Last());
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
            Rectangle = new Rect(Vehicles.Values);
            MassCenter = Vehicles.Any()
                ? new Point(Vehicles.Values.Average(i => i.X), Vehicles.Values.Average(i => i.Y))
                : Point.Zero;
        }


        public ActionSequence Split(int runFromCenter = 0)
        {
            var child = FormationFactory.CreateFormation(Rectangle.Left, Rectangle.Top, MassCenter.X, MassCenter.Y);
            Children.Add(child.Formation);
            var sequence = new ActionSequence(child.ActionList.ToArray());
            if (runFromCenter > 0)
            {
                sequence.Add(child.Formation.GetShiftAction(-runFromCenter, -runFromCenter));
            }

            child = FormationFactory.CreateFormation(MassCenter.X, Rectangle.Top, Rectangle.Right, MassCenter.Y);
            sequence.AddRange(child.ActionList);
            Children.Add(child.Formation);
            if (runFromCenter > 0)
            {
                sequence.Add(child.Formation.GetShiftAction(runFromCenter, -runFromCenter));
            }

            child = FormationFactory.CreateFormation(Rectangle.Left, MassCenter.Y, MassCenter.X, Rectangle.Bottom);
            sequence.AddRange(child.ActionList);
            Children.Add(child.Formation);
            if (runFromCenter > 0)
            {
                sequence.Add(child.Formation.GetShiftAction(-runFromCenter, runFromCenter));
            }
            child = FormationFactory.CreateFormation(MassCenter.X, MassCenter.Y, Rectangle.Right, Rectangle.Bottom);
            sequence.AddRange(child.ActionList);
            Children.Add(child.Formation);
            if (runFromCenter > 0)
            {
                sequence.Add(child.Formation.GetShiftAction(runFromCenter, runFromCenter));
            }
            return sequence;
        }

        public Action MoveCenterTo(double x, double y, double maxSpeed = 10)
        {
            return GetShiftAction(x - Rectangle.Center.X, y - Rectangle.Center.Y);
        }

        public Action MoveLeftTopTo(double x, double y, double maxSpeed = 10)
        {
            return GetShiftAction(x - Rectangle.Left, y - Rectangle.Top);
        }

        public Action GetShiftAction(double x, double y, double maxSpeed = 10)
        {
            var actualX = MassCenter.X;
            var actualY = MassCenter.Y;
            var action = new Action(this)
            {
                Action = ActionType.Move,
                X = x,
                Y = y,
                GetDeltaX = () => MassCenter.X - actualX,
                GetDeltaY = () => MassCenter.Y - actualY,
                MaxSpeed = maxSpeed
            };
            return action;
        }

        public Action ScaleLeftTop(double factor)
        {
            var actualX = MassCenter.X;
            var actualY = MassCenter.Y;
            var action = new Action(this)
            {
                Action = ActionType.Scale,
                Factor = factor,
                X = Rectangle.Left,
                Y = Rectangle.Top,
                GetDeltaX = () => MassCenter.X - actualX,
                GetDeltaY = () => MassCenter.Y - actualY
            };
            return action;
        }


        public Action GetSelectionAction()
        {
            var move = new Action(this) {Action = ActionType.ClearAndSelect};
            if (GroupIndex > 0)
            {
                move.Group = GroupIndex;
            }
            else
            {
                move.VehicleType = Type;
                move.Right = Global.World.Width;
                move.Bottom = Global.World.Height;
            }
            return move;
        }
    }
}