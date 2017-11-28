using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class ActionQueue
    {
        private readonly List<Action> _internalQueue = new List<Action>();

        private static int _wait = -1;

        public void Add(Action action)
        {
            _internalQueue.Add(action);
        }


        public int WaitTicks(int ticks)
        {
            var action = new Action {WaitForWorldTick = Global.World.TickIndex + ticks};
            Add(action);
            return action.WaitForWorldTick;
        }

        public bool HasActionsFor(Formation formation)
        {
            return _internalQueue.Any(a => a.Formation == formation);
        }

        public void Process()
        {
            // Если в очереди етсь приказы и мы имеем право действовать
            if (_internalQueue.Any() && Global.Me.RemainingActionCooldownTicks == 0)
            {
                // Если существуют срочные приказы они выполняются без задержек
                var action = _internalQueue.FirstOrDefault(i => i.Urgent && i.Ready);
                if (action != null)
                {
                    Execute(action, Global.Move);
                    return;
                }

                // Если метка ожидания пройдена 
                if (Global.World.TickIndex >= _wait)
                {
                    // Ищется готовое действие заданное не для группы либо для непустой группы готовой действовать
                    action = _internalQueue.FirstOrDefault(i =>
                        (i.Ready && (i.Formation == null || (!i.Formation.Busy && i.Formation.Vehicles.Count > 0))));
                    if (action != null)
                    {
                        _wait = -1;

                        // Если текущая выделенная формация не совпадает с той для которой выполняется действие,
                        // то надо вместо заданного приказа выполнить приказ на выделение
                        if (action.Formation != null && Global.SelectedFormation != action.Formation)
                        {
                            Global.SelectedFormation = action.Formation;
                            action = action.Formation.GetSelectionAction();
                        }
                        Execute(action, Global.Move);
                    }
                }
            }
        }


        private void Execute(Action action, Move move)
        {
            if (action == null) return;

            try
            {
                if (action.WaitForWorldTick > 0)
                {
                    _wait = action.WaitForWorldTick;
                }
                else
                {
                    move.Action = action.Action;
                    move.X = action.X + action.GetDeltaX();
                    move.Y = action.Y + action.GetDeltaY();
                    move.Group = action.Group;
                    move.Angle = action.Angle;
                    move.Left = action.Left;
                    move.Top = action.Top;
                    move.Right = action.Right;
                    move.Bottom = action.Bottom;
                    move.VehicleType = action.VehicleType;
                    move.MaxSpeed = action.MaxSpeed;
                    move.MaxAngularSpeed = action.MaxAngularSpeed;
                    move.Factor = action.Factor;
                    move.FacilityId = action.FacilityId;
                    move.VehicleId = action.VehicleId;

                    // Если приказ пришел от группы, то надо сдвинуть таймер группы на заданное число тиков
                    if (action.Formation != null)
                    {
                        action.Formation.WaitUntilIndex = Global.World.TickIndex + action.MinimumDuration;
                        if (action.Action == ActionType.ClearAndSelect)
                            Global.SelectedFormation = action.Formation;
                    }
                }

                action.Callback?.Invoke();
                Console.WriteLine($"Action:{action}");
            }
            finally
            {
                _internalQueue.Remove(action);
            }
        }
    }
}