using System;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed class MyStrategy : IStrategy
    {
        public void Move(Player me, World world, Game game, Move move)
        {
            try
            {
                Initializer.Init(me, world, game, move);

                SelectionTest();

                Global.ActionQueue.Process();
            }
            catch (Exception ex)
            {
                var exception = ex;
            }
        }

        private bool _isFightersSplitted;
        private bool _isFightersScaled;

        private void SelectionTest()
        {
            if (Global.World.TickIndex == 1)
            {
                var fighters = Global.Formations[-(int) VehicleType.Fighter];
                fighters.MoveLeftTopTo(300, 300);
            }

            if (Global.World.TickIndex % 60 == 0)
            {
                var fighters = Global.Formations[-(int) VehicleType.Fighter];
                if (!fighters.Busy && !fighters.Children.Any())
                {
                    fighters.Split(100);
                    _isFightersSplitted = true;
                }
                if (!_isFightersScaled)
                {
                    if (fighters.Children.All(c => c.Alive && !c.Busy))
                    {
                        foreach (var child in fighters.Children)
                        {
                            child.ScaleLeftTop(3);
                        }
                        _isFightersScaled = true;
                    }
                }
            }
        }
    }
}