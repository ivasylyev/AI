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

        public bool Urgent;

        public int WaitForWorldTick;

        public Func<bool> Condition;

        public System.Action Callback;

        public Formation Formation;

        public int MinimumDuration;

        public bool Ready => Condition == null || Condition();
    }
}