using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class ActionQueueProcessor
    {
        public static void ProcessQueue()
        {
            if (Global.ActionQueue.Any() && Global.Me.RemainingActionCooldownTicks == 0)
            {
                var move = Global.ActionQueue.Dequeue();
                Global.Move.Action = move.Action;
                Global.Move.X = move.X;
                Global.Move.Y = move.Y;
                Global.Move.Angle = move.Angle;
                Global.Move.Group = move.Group;
                Global.Move.Left = move.Left;
                Global.Move.Top = move.Top;
                Global.Move.Right = move.Right;
                Global.Move.Bottom = move.Bottom;
                Global.Move.VehicleType = move.VehicleType;
                Global.Move.MaxSpeed = move.MaxSpeed;
                Global.Move.MaxAngularSpeed = move.MaxAngularSpeed;
                Global.Move.Factor = move.Factor;
                Global.Move.FacilityId = move.FacilityId;
                Global.Move.VehicleId = move.VehicleId;
            }
        }
    }
}