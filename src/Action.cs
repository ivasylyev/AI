using System;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Action : Move
    {
        public const int DefaultMinimalDuration = 10;

        public Action(int minimumDuration = DefaultMinimalDuration)
        {
            MinimumDuration = minimumDuration;
        }

        public Action(Formation formation, int minimumDuration = DefaultMinimalDuration)
        {
            Formation = formation;
            MinimumDuration = minimumDuration;
            FinishCondition = () => !Formation.Busy;
            StartCondition = () => !Formation.Busy && Formation.Vehicles.Count > 0;
        }

        public bool Urgent { get; set; }
        public int WaitForWorldTick { get; set; }
        public ActionStatus Status { get; set; }

        public Func<bool> StartCondition;
        public Func<bool> FinishCondition;
        public Func<double> GetDeltaX = () => 0;
        public Func<double> GetDeltaY = () => 0;

        public System.Action Callback;

        public Formation Formation { get; set; }

        public int MinimumDuration { get; set; }

        public bool ReadyToStart => StartCondition == null || StartCondition();
        public bool ReadyToFinish => FinishCondition == null || FinishCondition();

        public override string ToString()
        {
            return $"{Action} Rect:({Left};{Top}  {Right};{Bottom}), XY:({X};{Y}), Group:{Group} Status:{Status}";
        }
    }
}