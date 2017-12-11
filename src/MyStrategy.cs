using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MyStrategy :IStrategy
    {
        readonly MyOldStrategy _oldStrategy = new MyOldStrategy();
        readonly MyNewStrategy _newStrategy = new MyNewStrategy();
        public void Move(Player me, World world, Game game, Move move)
        {
            if (world.Facilities.Any())
            {
                _newStrategy.Move(me,world,game,move);
            }
            else
            {
                _oldStrategy.Move(me, world, game, move);
            }

        }
    }
}