using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class TacticalActions
    {
        public static void CompactGroundFormations()
        {
//            var factor = 0.5;
//            Global.ActionQueue.Add(new ActionSequence(Global.MyTanks.ScaleCenter(factor)));
//            Global.ActionQueue.Add(new ActionSequence(Global.MyArrvs.ScaleCenter(factor)));
//            Global.ActionQueue.Add(new ActionSequence(Global.MyIfvs.ScaleCenter(factor)));
        }

        public static MyFormation CreateAirFormation()
        {
            const double eps = 10D;
            const double deltaShift = 5.1D;
            const double commonCoordinate = 350D;
            const double nearCoordinate = 60D;
            const double farCoordinate = 200D;
            const double factor = 1.7D;
            const double vehicleSize = 4D;


            var fighters = Global.MyFighters;
            var helicopters = Global.MyHelicopters;
            Global.IgnoreCollisionGroupIndexes.Add(fighters.GroupIndex);
            Global.IgnoreCollisionGroupIndexes.Add(helicopters.GroupIndex);

            var isVertical = Math.Abs(fighters.Rect.Left - helicopters.Rect.Left) < eps;

            MyFormation f1;
            MyFormation f2;
            double f1MoveX;
            double f1MoveY;
            double f2MoveX;
            double f2MoveY;
            double shiftX;
            double shiftY;
            double compactX;
            double compactY;
            double angle;

            if (isVertical)
            {
                f1MoveX = commonCoordinate + deltaShift;
                f1MoveY = nearCoordinate;
                f2MoveX = commonCoordinate;
                f2MoveY = farCoordinate;
                shiftX = 0;
                shiftY = (farCoordinate - nearCoordinate) / 2;
                compactX = 0;
                compactY = 3 * vehicleSize;
                angle = -Math.PI / 4;
            }

            else
            {
                f1MoveX = nearCoordinate;
                f1MoveY = commonCoordinate + deltaShift;
                f2MoveX = farCoordinate;
                f2MoveY = commonCoordinate;
                shiftX = (farCoordinate - nearCoordinate) / 2;
                shiftY = 0;
                compactX = 3 * vehicleSize;
                compactY = 0;
                angle = Math.PI / 4;
            }
            if (isVertical && fighters.Rect.Top < helicopters.Rect.Top ||
                !isVertical && fighters.Rect.Left < helicopters.Rect.Left)
            {
                f1 = fighters;
                f2 = helicopters;
            }
            else
            {
                f1 = helicopters;
                f2 = fighters;
            }
            // двигаем первую формацию налево или вниз, а потом - масштабируем ее
            var sMove1 = new ActionSequence(
                f1.MoveLeftTopTo(f1MoveX, f1MoveY),
                f1.ScaleLeftTop(factor)
            );
            Global.ActionQueue.Add(sMove1);

            // двигаем вторую формацию налево или вниз, а потом - масштабируем ее
            var sMove2 = new ActionSequence(
                f2.MoveLeftTopTo(f2MoveX, f2MoveY),
                f2.ScaleLeftTop(factor)
            );
            Global.ActionQueue.Add(sMove2);

            // после того, как обе формации отмасштабированы, первая формация движеться наствречу второй
            var aPenetrate1 = f1.ShiftTo(shiftX, shiftY);
            aPenetrate1.StartCondition = () => sMove1.IsFinished && sMove2.IsFinished;
            var sPenetrate1 = new ActionSequence(aPenetrate1);
            Global.ActionQueue.Add(sPenetrate1);

            // а вторая - навстречу первой до полного проникновения
            var aPenetrate2 = f2.ShiftTo(-shiftX, -shiftY);
            aPenetrate2.StartCondition = () => sMove1.IsFinished && sMove2.IsFinished;
            var sPenetrate2 = new ActionSequence(aPenetrate2);
            Global.ActionQueue.Add(sPenetrate2);

            // сплющиваем сбоку бутерброд
            var res = FormationFactory.CreateMyFormation(
                () => Math.Min(f1.Rect.Left, f2.Rect.Left),
                () => Math.Min(f1.Rect.Top, f2.Rect.Top),
                () => Math.Max(f1.Rect.Right, f2.Rect.Right) - compactX,
                () => Math.Max(f1.Rect.Bottom, f2.Rect.Bottom) - compactY);

            Global.IgnoreCollisionGroupIndexes.Add(res.GroupIndex);

            var sShift = new ActionSequence(res.ActionList.ToArray());
            sShift.First().StartCondition = () => sPenetrate2.IsFinished && sPenetrate1.IsFinished;

            sShift.Add(res.Formation.ShiftTo(shiftX, shiftY));
            Global.ActionQueue.Add(sShift);

            //   компактизируем  
            res = FormationFactory.CreateMyFormation(
                () => Math.Min(f1.Rect.Left, f2.Rect.Left),
                () => Math.Min(f1.Rect.Top, f2.Rect.Top),
                () => Math.Max(f1.Rect.Right, f2.Rect.Right),
                () => Math.Max(f1.Rect.Bottom, f2.Rect.Bottom));

            var sCompact = new ActionSequence(res.ActionList.ToArray());
            sCompact.First().StartCondition = () => sShift.IsFinished;
            sCompact.Add(res.Formation.ScaleCenter(1.2));
            sCompact.Add(res.Formation.RotateCenter(angle));
            sCompact.Add(res.Formation.ScaleCenter(0.5));
            sCompact.Add(res.Formation.ShiftTo(10, 10));
            Global.ActionQueue.Add(sCompact);

            return res.Formation;
        }

        public static void SplitFormation(MyFormation formation, int runFromCenter = 0)
        {
            var child = FormationFactory.CreateMyFormation(
                () => formation.Rect.Left,
                () => formation.Rect.Top,
                () => formation.MassCenter.X,
                () => formation.MassCenter.Y);
            formation.Children.Add(child.Formation);
            var sequence = new ActionSequence(child.ActionList.ToArray());
            if (runFromCenter > 0)
            {
                sequence.Add(child.Formation.ShiftTo(-runFromCenter, -runFromCenter));
            }

            child = FormationFactory.CreateMyFormation(
                () => formation.MassCenter.X,
                () => formation.Rect.Top,
                () => formation.Rect.Right,
                () => formation.MassCenter.Y);
            sequence.AddRange(child.ActionList);
            formation.Children.Add(child.Formation);
            if (runFromCenter > 0)
            {
                sequence.Add(child.Formation.ShiftTo(runFromCenter, -runFromCenter));
            }

            child = FormationFactory.CreateMyFormation(
                () => formation.Rect.Left,
                () => formation.MassCenter.Y,
                () => formation.MassCenter.X,
                () => formation.Rect.Bottom);
            sequence.AddRange(child.ActionList);
            formation.Children.Add(child.Formation);
            if (runFromCenter > 0)
            {
                sequence.Add(child.Formation.ShiftTo(-runFromCenter, runFromCenter));
            }
            child = FormationFactory.CreateMyFormation(
                () => formation.MassCenter.X,
                () => formation.MassCenter.Y,
                () => formation.Rect.Right,
                () => formation.Rect.Bottom);
            sequence.AddRange(child.ActionList);
            formation.Children.Add(child.Formation);
            if (runFromCenter > 0)
            {
                sequence.Add(child.Formation.ShiftTo(runFromCenter, runFromCenter));
            }
            Global.ActionQueue.Add(sequence);
        }

        public static IEnumerable<MyFormation> CreateProducedFormation()
        {
            foreach (var facility in Global.MyFacilities)
            {
                if (facility.Type == FacilityType.VehicleFactory)
                {
                    var createdVehicles = Global.MyVehicles.Values
                        .Where(v => v.Groups.Length == 0 && v.IsInside(facility.Rect) && v.IsStanding)
                        .ToList();

                    if (createdVehicles.Count > 30)

                    {
                        var result = FormationFactory.CreateMyFormation(facility.Rect.Left, facility.Rect.Top,
                            facility.Rect.Right, facility.Rect.Bottom);

                        Global.ActionQueue.Add(new ActionSequence(result.ActionList.ToArray()));
                        yield return result.Formation;
                    }
                }
            }
        }

        public static void SetupProductionIfNeed(Facility facility, VehicleType neededType)
        {
            if (facility.Type == FacilityType.VehicleFactory && facility.VehicleType != neededType)
            {
                if (!Global.ActionQueue.HasActionsFor(facility))
                {
                    var sequence = new ActionSequence(facility.SetupProduction(neededType));
                    Global.ActionQueue.Add(sequence);
                }
            }
        }

        public static void OccupyFacilities(MyFormation formation)
        {
            if (formation.Alive && !formation.Busy)
            {
                var freeFacility = Global.World.Facilities
                    .OrderBy(f => f.Center.SqrDistance(formation.Center))
                    .FirstOrDefault(f => !f.IsMine && !f.SelectedAsTargetForGroup.HasValue);

                if (freeFacility != null)
                {
                    freeFacility.SelectedAsTargetForGroup = formation.GroupIndex;

                    var actionMove = formation.MoveCenterTo(freeFacility.Center);
                    var sequence = new ActionSequence(actionMove);
                    Global.ActionQueue.Add(sequence);
                }
            }
        }

        public static void PauseExecuteAndContinue(MyFormation formation, params Action[] action)
        {
            if (formation != null && formation.Alive && action.Length > 0)
            {
                var executingAction = formation.ExecutingSequence?.GetExecutingAction();
                if (executingAction != null && executingAction.ActionType == ActionType.Move &&
                    executingAction.Interruptable)
                {
                    action.First().StartCondition = () => true;
                    formation.ExecutingSequence.AddRange(action);

                    var continueAction = executingAction.Clone();
                    continueAction.StartCondition = () =>
                        action.Last().Status == ActionStatus.Finished ||
                        action.Last().Status == ActionStatus.Aborted;
                    formation.ExecutingSequence.Add(continueAction);
                    executingAction.Abort();
                }
                if (executingAction == null)
                {
                    var newSequence = new ActionSequence(action);
                    Global.ActionQueue.Add(newSequence);
                }
            }
        }

        public static void Anticollision()
        {
            List<int> processedKeys = new List<int>();
            foreach (var key1 in Global.MyFormations.Keys)
            {
                var f1 = Global.MyFormations[key1];
                if (f1.Alive && !Global.IgnoreCollisionGroupIndexes.Contains(key1))
                {
                    foreach (var key2 in Global.MyFormations.Keys.Where(k => k != key1))
                    {
                        var f2 = Global.MyFormations[key2];
                        if (f2.Alive &&
                            !Global.IgnoreCollisionGroupIndexes.Contains(key2) &&
                            (f1.IsMixed || f2.IsMixed || f1.IsAllAeral == f2.IsAllAeral))
                        {
                            var distBetweenCenters = f1.Rect.Center.SqrDistance(f2.Rect.Center);
                            if (distBetweenCenters < (f1.Rect.SqrDiameter + f2.Rect.SqrDiameter) / 2)
                            {
                                var delta = f1.Center - f2.Center;

                                if (!processedKeys.Contains(key1))
                                {
                                    var delta1 = 20 * (delta.Normalized() + f1.AvgSpeed.Normalized());
                                    var move1 = f1.ShiftTo(delta1.X, delta1.Y);
                                    move1.Interruptable = false;

                                    PauseExecuteAndContinue(f1, move1);
                                }
                                if (!processedKeys.Contains(key2))
                                {
                                    var delta2 = 20 * (delta.Normalized() + f2.AvgSpeed.Normalized());
                                    var move2 = f2.ShiftTo(-delta2.X, -delta2.Y);
                                    move2.Interruptable = false;

                                    PauseExecuteAndContinue(f2, move2);
                                }
                                processedKeys.Add(key1);
                                processedKeys.Add(key2);
                            }
                        }
                    }
                }
            }
        }
    }
}