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
        public int WaitForWorldTick { get; set; }
        public ActionStatus Status { get; set; } = ActionStatus.Pending;

        public Func<bool> StartCondition;
        public Func<bool> FinishCondition;
        public Func<double> GetX = () => 0;
        public Func<double> GetY = () => 0;

        public Func<double> GetLeft = () => 0;
        public Func<double> GetTop = () => 0;
        public Func<double> GetRight = () => 0;
        public Func<double> GetBottom = () => 0;

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
            FinishCondition = () => !Formation.Busy;
            StartCondition = () => !Formation.Busy && Formation.Vehicles.Count > 0;
        }

        public void Abort()
        {
            Status = ActionStatus.Aborted;
        }

        public override string ToString()
        {
            return
                $"Tick:{Global.World.TickIndex},{ActionType} Rect:({GetLeft()};{GetTop()}  {GetRight()};{GetBottom()}), X;Y:({GetX()};{GetY()}), Group:{Group} Status:{Status}";
        }
    }
}