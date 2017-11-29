﻿using System;
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

        public bool HasActionsFor(Formation formation)
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
                        executingAction.Status = ActionStatus.Finished;
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
                    sequence = _internalQueue.FirstOrDefault(s =>s.ReadyToStart);
                    if (sequence != null)
                    {
                        _wait = -1;

                        var action = sequence.GetPendingAction();
                        if (action.Formation != null && Global.SelectedFormation != action.Formation)
                        {
                            Global.SelectedFormation = action.Formation;

                            action = action.Formation.GetSelectionAction();
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
                    move.Action = action.Action;
                    move.X = action.X - action.GetDeltaX();
                    move.Y = action.Y - action.GetDeltaY();
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

                    if (action.Formation != null)
                    {
                        action.Formation.WaitUntilIndex = Global.World.TickIndex + action.MinimumDuration;
                        if (action.Action == ActionType.ClearAndSelect)
                        {
                            Global.SelectedFormation = action.Formation;
                        }
                    }
                }

                action.Status = ActionStatus.Executing;
                action.Callback?.Invoke();
                Console.WriteLine($"Action:{action}");
            }
            finally
            {
                if (!sequence.Any() || sequence.All(a => a.Status == ActionStatus.Finished))
                {
                    _internalQueue.Remove(sequence);
                }
            }
        }
    }
}