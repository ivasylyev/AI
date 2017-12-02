using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class ActionQueue
    {
        private static int _wait = -1;
        private readonly List<ActionSequence> _internalQueue = new List<ActionSequence>();

        public void Add(ActionSequence actions)
        {
            _internalQueue.Add(actions);

            foreach (var action in actions)
            {
                action.Status = ActionStatus.Pending;
            }
        }


        public int WaitTicks(int ticks)
        {
            var action = new Action {WaitForWorldTick = Global.World.TickIndex + ticks};
            Add(new ActionSequence(action));
            return action.WaitForWorldTick;
        }

        public bool HasActionsFor(MyFormation formation)
        {
            return _internalQueue.Any(sequence => sequence.Any(a => a.Formation == formation));
        }

        public void Update()
        {
            foreach (var sequence in _internalQueue)
            {
                var executingAction = sequence.GetExecutingAction();
                if (executingAction != null)
                {
                    if (executingAction.ReadyToFinish)
                    {
                        executingAction.Status = ActionStatus.Finished;
                    }
                }
            }
        }

        public void Process()
        {
            if (_internalQueue.Any() && Global.Me.RemainingActionCooldownTicks == 0)
            {
                var sequence = _internalQueue.FirstOrDefault(s => s.Urgent && s.ReadyToStart);
                if (sequence != null)
                {
                    var action = sequence.GetPendingAction();
                    Execute(sequence, action, Global.Move);
                    return;
                }

                if (Global.World.TickIndex >= _wait)
                {
                    sequence = _internalQueue.Where(s => s.ReadyToStart).ToList().FirstOrDefault();
                    if (sequence != null)
                    {
                        _wait = -1;

                        var action = sequence.GetPendingAction();
                        if (action.Formation != null)
                        {
                            if (Global.SelectedFormation != action.Formation)
                            {
                                action.Urgent = true;
                                action = action.Formation.GetSelectionAction();
                            }
                            else
                            {
                                action.Formation.WaitUntilIndex = Global.World.TickIndex + action.MinimumDuration;
                            }
                        }
                        Execute(sequence, action, Global.Move);
                    }
                }
            }
        }


        private void Execute(ActionSequence sequence, Action action, Move move)
        {
            if (action == null)
            {
                return;
            }

            try
            {
                if (action.WaitForWorldTick > 0)
                {
                    _wait = action.WaitForWorldTick;
                }
                else
                {
                    move.Action = action.ActionType;
                    move.X = action.GetX();
                    move.Y = action.GetY();
                    move.Group = action.Group;
                    move.Angle = action.Angle;
                    move.Left = action.GetLeft();
                    move.Top = action.GetTop();
                    move.Right = action.GetRight();
                    move.Bottom = action.GetBottom();
                    move.VehicleType = action.VehicleType;
                    move.MaxSpeed = action.MaxSpeed;
                    move.MaxAngularSpeed = action.MaxAngularSpeed;
                    move.Factor = action.Factor;
                    move.FacilityId = action.FacilityId;
                    move.VehicleId = action.VehicleId;

                    if (action.Formation != null)
                    {
                        action.Formation.ExecutingAction = action;
                        action.Formation.ExecutingSequence = sequence;
                        if (action.ActionType == ActionType.ClearAndSelect)
                        {
                            Global.SelectedFormation = action.Formation;
                        }
                    }
                    action.ExecutingMove = move;
                }

                action.Status = ActionStatus.Executing;
                action.Callback?.Invoke();
                Console.WriteLine($"Action:{action}");
            }
            finally
            {
                if (sequence.IsFinished)
                {
                    _internalQueue.Remove(sequence);
                }
            }
        }
    }
}