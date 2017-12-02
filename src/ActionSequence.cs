using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class ActionSequence : List<Action>
    {
        public ActionSequence(params Action[] action)
        {
            AddRange(action);
        }

        public bool Urgent
        {
            get
            {
                var pendingAction = GetPendingAction();
                var executingAction = GetExecutingAction();
                if (pendingAction == null || executingAction != null)
                {
                    return false;
                }
                return pendingAction.Urgent;
            }
        }

        public bool ReadyToStart
        {
            get
            {
                var pendingAction = GetPendingAction();
                var executingAction = GetExecutingAction();
                if (pendingAction == null || executingAction != null)
                {
                    return false;
                }
                return pendingAction.ReadyToStart;
            }
        }

        public ActionStatus Status
        {
            get
            {
                if (!this.Any() || this.All(a => a.Status == ActionStatus.Finished))
                {
                    return ActionStatus.Finished;
                }
                if (this.Any(a => a.Status == ActionStatus.Executing))
                {
                    return ActionStatus.Executing;
                }
                if (this.Any(a => a.Status == ActionStatus.Pending))
                {
                    return ActionStatus.Pending;
                }
                if (this.Any(a => a.Status == ActionStatus.Aborted))
                {
                    return ActionStatus.Aborted;
                }
                return ActionStatus.Undefined;
            }
        }

        public bool IsFinished => Status == ActionStatus.Finished || Status == ActionStatus.Aborted;

        public void Add(ActionSequence sequence)
        {
            AddRange(sequence);
        }

        public void AbortExecutingAction()
        {
            GetExecutingAction()?.Abort();
        }

        public void AbortAll()
        {
            foreach (var action in this)
            {
                action.Abort();
            }
        }

        public Action GetExecutingAction()
        {
            return this.FirstOrDefault(a => a.Status == ActionStatus.Executing);
        }

        public Action GetPendingAction()
        {
            return this.FirstOrDefault(a => a.Status == ActionStatus.Pending);
        }
    }
}