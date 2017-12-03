using System;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Action
    {
        public const int DefaultMinimalDuration = 10;

        public ActionType? ActionType { get; set; }

        public int Group { get; set; }

        public double Angle { get; set; }

        public double Factor { get; set; }

        public double MaxSpeed { get; set; }
        public double MaxAngularSpeed { get; set; }

        public VehicleType? VehicleType { get; set; }

        public long FacilityId { get; set; } = -1L;
        public long VehicleId { get; set; } = -1L;

        public bool Urgent { get; set; }
        public bool Interruptable { get; set; } = true;
        public bool IsAnticollision { get; set; }
        
        public int AbortAtWorldTick { get; set; } = -1;
        public ActionStatus Status { get; set; } = ActionStatus.Pending;

        public Func<bool> StartCondition;
        public Func<bool> FinishCondition;
        public Func<double> GetX = () => 0;
        public Func<double> GetY = () => 0;

        public Func<double> GetLeft = () => 0;
        public Func<double> GetTop = () => 0;
        public Func<double> GetRight = () => 0;
        public Func<double> GetBottom = () => 0;

        public Move ExecutingMove { get; set; }

        public System.Action Callback;

        public MyFormation Formation { get; set; }

        public int MinimumDuration { get; set; }

        public bool ReadyToStart => StartCondition == null || StartCondition();
        public bool ReadyToFinish => FinishCondition == null || FinishCondition();

        public Action(int minimumDuration = DefaultMinimalDuration)
        {
            MinimumDuration = minimumDuration;
        }

        public Action(MyFormation formation, int minimumDuration = DefaultMinimalDuration)
        {
            Formation = formation;
            MinimumDuration = minimumDuration;
            if (Formation != null)
            {
                FinishCondition = () => !Formation.Busy;
                StartCondition = () => !Formation.Busy && Formation.Vehicles.Count > 0;
            }
        }

        public Action Clone()
        {
            Action clone = new Action(Formation, MinimumDuration)
            {
                ActionType = ActionType,
                Group = Group,
                Angle = Angle,
                Factor = Factor,
                MaxSpeed = MaxSpeed,
                MaxAngularSpeed = MaxAngularSpeed,
                VehicleType = VehicleType,
                VehicleId = VehicleId,
                FacilityId = FacilityId,
                Urgent = Urgent,
                Status = ActionStatus.Pending,
                GetX = GetX,
                GetY = GetY,
                GetLeft = GetLeft,
                GetTop = GetTop,
                GetRight = GetRight,
                GetBottom = GetBottom,
                Callback = Callback
            };
            return clone;
        }

        public void Abort()
        {
            Status = ActionStatus.Aborted;
        }

        public override string ToString()
        {
            return
                $"Tick:{Global.World.TickIndex},{ActionType},{Formation?.Type}, Rect:({GetLeft():###.#};{GetTop():###.#}  {GetRight():###.#};{GetBottom():###.#}), X;Y:({GetX():###.#};{GetY():###.#}), Group:{Group} Status:{Status}";
        }
    }
}