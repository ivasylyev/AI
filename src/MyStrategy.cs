using System;
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
                if (fighters.IsStanding && !_isFightersSplitted)
                {
                    var f1 = FormationFactory.CreateFormation(fighters.Rectangle.Left, fighters.Rectangle.Top,
                        fighters.MassCenter.X, fighters.MassCenter.Y, 1);
                    f1.Shift(-100, -100);

                    var f2 = FormationFactory.CreateFormation(fighters.MassCenter.X, fighters.Rectangle.Top,
                        fighters.Rectangle.Right, fighters.MassCenter.Y, 2);
                    f2.Shift(100, -100);

                    var f3 = FormationFactory.CreateFormation(fighters.Rectangle.Left, fighters.MassCenter.Y,
                        fighters.MassCenter.X, fighters.Rectangle.Bottom, 3);
                    f3.Shift(-100, 100);
                    var f4 = FormationFactory.CreateFormation(fighters.MassCenter.X, fighters.MassCenter.Y,
                        fighters.Rectangle.Right, fighters.Rectangle.Bottom, 4);
                    f4.Shift(100, 100);

                    _isFightersSplitted = true;
                }
                else if (fighters.IsStanding && _isFightersSplitted && !_isFightersScaled)
                {
                    var f1 = Global.Formations[1];
                    var f2 = Global.Formations[2];
                    var f3 = Global.Formations[3];
                    var f4 = Global.Formations[4];

                    f1.ScaleLeftTop(3);

                    f2.ScaleLeftTop(3);

                    f3.ScaleLeftTop(3);

                    f4.ScaleLeftTop(3);

                    _isFightersScaled = true;
                }
            }
        }
    }
}