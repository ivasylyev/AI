using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class ActionQueue
    {
        private readonly List<ActionSequence> _internalQueue = new List<ActionSequence>();

        public void Add(ActionSequence sequence)
        {
            var moveActions = sequence.Where(a => a.ActionType == ActionType.Move && a.Formation != null).ToList();
            if (moveActions.Count == 1)
            {
                var action = moveActions.First();
                if (action.IsAnticollision)
                {
                    if (_internalQueue.Any(s => s.Any(a => a.IsAnticollision &&
                                                           a.Formation == action.Formation &&
                                                           (a.Status == ActionStatus.Pending ||
                                                           a.Status == ActionStatus.Executing))))
                    {
                        action.Status = ActionStatus.Aborted;
                        return;
                    }
                }
                Func<Action, bool> replacePredicate = a => a.Interruptable &&
                                                    a.ActionType == ActionType.Move &&
                                                    a.Formation == action.Formation &&
                                                           (a.Status == ActionStatus.Pending ||
                                                            a.Status == ActionStatus.Executing);
                var oldSequence = _internalQueue.LastOrDefault(s => s.Any(replacePredicate));
                var actionToReplace = oldSequence?.LastOrDefault(replacePredicate);
                actionToReplace?.Abort();
            }
            _internalQueue.Add(sequence);

            foreach (var action in sequence)
            {
                action.Status = ActionStatus.Pending;
            }
        }

        public void Clear()
        {
            _internalQueue.Clear();
        }

        public bool HasActionsFor(ActionType type)
        {
            return _internalQueue.Any(sequence => sequence.Any(a => a.ActionType == type));
        }
        public bool HasActionsFor(MyFormation formation)
        {
            return _internalQueue.Any(sequence => sequence.Any(a => a.Formation == formation));
        }

        public bool HasActionsFor(Facility facility)
        {
            return _internalQueue.Any(sequence => sequence.Any(a => a.FacilityId == facility.Id));
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
            _internalQueue.RemoveAll(s => s.IsFinished);
            foreach (var sequence in _internalQueue)
            {
                foreach (var action in sequence)
                {
                    if (Global.World.TickIndex == action.AbortAtWorldTick)
                    {
                        action.Abort();
                    }
                }
            }
            if (_internalQueue.Any() && Global.Me.RemainingActionCooldownTicks == 0)
            {
                var sequence = _internalQueue.FirstOrDefault(s => s.Urgent && s.ReadyToStart) ??
                               _internalQueue.FirstOrDefault(s => s.ReadyToStart);
                if (sequence != null)
                {
                    var action = sequence.GetPendingAction();
                    action = GetActionOrSelectedActionIfNeeded(action);
                    Execute(sequence, action, Global.Move);
                }
            }
        }

        private static Action GetActionOrSelectedActionIfNeeded(Action action)
        {
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
            return action;
        }


        private void Execute(ActionSequence sequence, Action action, Move move)
        {
            if (action == null)
            {
                return;
            }


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


            action.Status = ActionStatus.Executing;
            action.Callback?.Invoke();
            Console.WriteLine($"Action:{action}");
        }
    }
}