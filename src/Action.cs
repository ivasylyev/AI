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
        }

        public bool Urgent { get; set; }
        public int WaitForWorldTick { get; set; }
        public ActionStatus Status { get; set; }

        public Func<bool> Condition;
        public Func<double> GetDeltaX = () => 0;
        public Func<double> GetDeltaY = () => 0;

        public System.Action Callback;

        public Formation Formation { get; set; }

        public int MinimumDuration { get; set; }

        public bool Ready => Condition == null || Condition();

        public override string ToString()
        {
            return $"{Action} Rect:({Left};{Top}  {Right};{Bottom}), XY:({X};{Y}), Group:{Group} Status:{Status}";
        }
    }
}