using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class TacticalActions
    {
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

        public static void CreateProducedFormation()
        {
            foreach (var facility in Global.MyFacilities)
            {
                if (facility.Type == FacilityType.VehicleFactory)
                {
                    List<VehicleWrapper> createdVehicles = facility.CreatedVehicles;

                    if (createdVehicles.Count > 40 || facility.NearGroundEnemyCount > 10 && createdVehicles.Count > 20)
                    {
                        Rect createdRect = facility.CreatedVehiclesRect;
                        var allVehiclesInsideCount = Global.MyVehicles.Values.Count(v => v.IsInside(createdRect));
                        if (createdVehicles.Count == allVehiclesInsideCount)
                        {
                            var stopProduction = facility.StopProduction();
                            var result = FormationFactory.CreateMyFormation(createdRect.Left, createdRect.Top,
                                createdRect.Right, createdRect.Bottom);
                            result.ActionList.Insert(0, stopProduction);

                            foreach (var action in result.ActionList)
                            {
                                action.Priority = ActionPriority.High;
                            }
                            Global.ActionQueue.Add(new ActionSequence(result.ActionList.ToArray()));
                        }
                    }
                }
            }
        }

        public static void SetupProductionIfNeed(Facility facility)
        {
            if (facility.Type != FacilityType.VehicleFactory)
                return;

            var myFightersCount = Global.MyFighters.Vehicles.Count;
            var myHelisCount = Global.MyHelicopters.Vehicles.Count;

            var enemyFightersCount = Global.EnemyFighters.Count();
            var enemyHeliCount = Global.EnemyHelicopters.Count();

            VehicleType neededType;
            bool isGroundProducing = (facility.VehicleType == null ||
                                      facility.VehicleType == VehicleType.Tank ||
                                      facility.VehicleType == VehicleType.Ifv ||
                                      facility.VehicleType == VehicleType.Arrv);

//            if (myFightersCount < 0.8 * enemyFightersCount)
//            {
//                neededType = VehicleType.Fighter;
//                if (isGroundProducing)
//                {
//                    CreateProducedFormation();
//                }
//            }
//            else 
            if (myHelisCount < 0.8 * enemyHeliCount && enemyFightersCount < 30)
            {
                neededType = VehicleType.Helicopter;
                if (isGroundProducing)
                {
                    CreateProducedFormation();
                }
            }
            else
            {
                if (facility.VehicleType == null ||
                    facility.VehicleType == VehicleType.Fighter ||
                    facility.VehicleType == VehicleType.Helicopter)
                {
                    CreateProducedFormation();
                    facility.InitialTickIndex = 0;
                }
                facility.InitialTickIndex++;
                var tick = (Global.World.TickIndex - facility.InitialTickIndex) % 1200;

                neededType = tick < 1200 - 6 * Math.Min(150, enemyHeliCount)
                    ? VehicleType.Tank
                    : VehicleType.Ifv;
            }

            if (facility.VehicleType != neededType)
            {
                if (!Global.ActionQueue.HasActionsFor(facility))
                {
                    if (Global.MyVehicles.Values.All(v => v.Groups.Length == 0 || !v.IsInside(facility.Rect)))
                    {
                        var sequence = new ActionSequence(facility.SetupProduction(neededType));
                        Global.ActionQueue.Add(sequence);
                    }
                }
            }
        }

        public static bool OccupyFacilities(MyFormation formation, bool breakCurrentAction)
        {
            if (formation.Alive &&
                (!formation.Busy || breakCurrentAction) &&
                formation.AeralPercent < 0.1)
            {
                var targetFacility = formation.FacilityAsTarget == null
                    ? null
                    : Global.World.Facilities.FirstOrDefault(f => f.Id == formation.FacilityAsTarget.Value);

                Facility freeFacility = null;
                if (targetFacility != null && !targetFacility.IsMine)
                {
                    freeFacility = targetFacility;
                }
                else
                {
                    var planningToOccupy = Global.MyFormations.Values
                        .Where(f => f.FacilityAsTarget.HasValue)
                        .Select(f => f.FacilityAsTarget.Value).ToList();

                    freeFacility = Global.NotMyFacilities
                        .Where(f => !f.IsMine && !planningToOccupy.Contains(f.Id))
                        .OrderBy(f => f.Center.SqrDistance(formation.Center)).FirstOrDefault();
                }
                if (freeFacility != null)
                {
                    formation.FacilityAsTarget = freeFacility.Id;

                    var actionMove = formation.MoveCenterTo(freeFacility.Center);
                    var sequence = new ActionSequence(actionMove);
                    Global.ActionQueue.Add(sequence);
                    return true;
                }
            }
            return false;
        }


        public static bool Attack(MyFormation formation)
        {
            if (AirAttack(formation))
                return true;

            var targetFacility = formation.FacilityAsTarget == null
                ? null
                : Global.World.Facilities.FirstOrDefault(f => f.Id == formation.FacilityAsTarget.Value);

            if (formation.FacilityAsTarget == null)
            {
                OccupyFacilities(formation, true);

                targetFacility = formation.FacilityAsTarget == null
                    ? null
                    : Global.World.Facilities.FirstOrDefault(f => f.Id == formation.FacilityAsTarget.Value);
            }
            if (targetFacility != null)
            {
                if (targetFacility.IsMine)
                {
                    targetFacility = null;
                    formation.FacilityAsTarget = null;
                }
            }

            double maxAttackDistance = (targetFacility == null)
                ? Global.World.Width / 2
                : Math.Min(100, formation.Center.Distance(targetFacility.Center) / 2);


            var myDurability = Global.MyVehicles.Values.Sum(v => v.Durability);
            var enemyDurability = Global.EnemyVehicles.Values.Sum(v => v.Durability);

            var betterCoeff = Math.Max(1, myDurability / (enemyDurability + 1.0));
            var oneShotDanger = new Dictionary<EnemyFormation, double>();
            foreach (var enemy in Global.EnemyFormations)
            {
                var dangerForEnemy = formation.DangerFor(enemy);
                var dangerForMe = enemy.DangerFor(formation);
                if (enemy.Center.Distance(formation.Center) < maxAttackDistance)
                {
                    oneShotDanger.Add(enemy, (betterCoeff) * dangerForEnemy - dangerForMe);
                }
            }

            EnemyFormation target = null;
            if (oneShotDanger.Count > 0)
            {
                var targetPair = oneShotDanger
                    .OrderByDescending(kv => kv.Value)
                    .FirstOrDefault();

                if (targetPair.Value > 0)
                {
                    target = targetPair.Key;
                }
            }

            if (target != null)
            {
                MakeAttackOrder(formation, target, false);
                return true;
            }

            var runAwayDanger = new Dictionary<EnemyFormation, double>();
            foreach (var enemy in Global.EnemyFormations)
            {
                var dangerForEnemy = formation.DangerFor(enemy);
                var dangerForMe = enemy.DangerFor(formation);
                if (enemy.Center.Distance(formation.Center) < 150)
                {
                    runAwayDanger.Add(enemy,  (dangerForEnemy - dangerForMe)/(formation.Count+1.0));
                }
            }
            if (runAwayDanger.Count > 0)
            {
                var targetPair = runAwayDanger
                    .OrderBy(kv => kv.Value)
                    .FirstOrDefault();
                if (targetPair.Value < -10)
                {
                    var runAwayFromThis = targetPair.Key;
                    var direction = (formation.MassCenter - runAwayFromThis.MassCenter).Normalized();
                    formation.FacilityAsTarget = null;


                    Global.ActionQueue.AbortOldActionsFor(formation);
                    var actionMove = formation.ShiftTo(direction*100);
                    actionMove.Priority = ActionPriority.High;
                    actionMove.StartCondition = () => true;
                    actionMove.Interruptable = false;
                    var sequence = new ActionSequence(actionMove);
                    Global.ActionQueue.Add(sequence);

                }

            }


            if (targetFacility != null)
            {
                var actionMove = formation.MoveCenterTo(targetFacility.Center);
                var sequence = new ActionSequence(actionMove);
                Global.ActionQueue.Add(sequence);
            }
            return false;
        }

        private static bool AirAttack(MyFormation formation)
        {
            if (formation != Global.MyHelicopters && formation != Global.MyFighters)
                return false;
            if (formation.Durability / (formation.MaxDurability + 1) < 0.7)
            {
                if (MoveToAlly(formation, Global.MyArrvs))
                    return true;
            }
            var enemyFightersCount = Global.EnemyFighters.Count();
            if (formation == Global.MyHelicopters && enemyFightersCount > 30)
            {
                var enemyFighters = FormationFactory.CreateEnemyFormation(Global.EnemyFighters);
                if ((enemyFighters.Rect.RightBottom - enemyFighters.Rect.LeftTop).Length() < 300)
                {
                    MyFormation foundAllyGround = null;
                    if (Global.MyIfvs.Alive && Global.MyIfvs.Vehicles.Count > 30)
                    {
                        foundAllyGround = Global.MyIfvs;
                    }
                    else if (Global.MyArrvs.Alive && Global.MyArrvs.Vehicles.Count > 30)
                    {
                        foundAllyGround = Global.MyArrvs;
                    }
                    if (foundAllyGround != null)
                    {
                        var actionMove = formation.MoveCenterTo(foundAllyGround.Center);
                        AbortAndAddToExecutingSequence(formation, actionMove);
                        return true;
                    }
                }
            }
            if (formation == Global.MyFighters)
            {
                if (enemyFightersCount > 10)
                {
                    var enemy = FormationFactory.CreateEnemyFormation(Global.EnemyFighters);
                    var enemyLength = (enemy.Rect.RightBottom - enemy.Rect.LeftTop).Length();
                    if (enemyLength < 200)
                    {
                        if (AttackOrRunAway(formation, enemy))
                            return true;
                    }
                }
                if (Global.EnemyHelicopters.Count() > 10)
                {
                    var enemy = FormationFactory.CreateEnemyFormation(Global.EnemyHelicopters);
                    var enemyLength = (enemy.Rect.RightBottom - enemy.Rect.LeftTop).Length();
                    if (enemyLength < 300)
                    {
                        if (AttackOrRunAway(formation, enemy))
                            return true;
                    }
                }
            }
            return false;
        }

        private static bool AttackOrRunAway(MyFormation formation, EnemyFormation enemy)
        {
            foreach (var enemyFormation in Global.EnemyFormations)
            {
                if (enemyFormation.Rect.IsInside(enemy.Center))
                {
                    var dangerForEnemy = formation.DangerFor(enemy);
                    var dangerForMe = enemy.DangerFor(formation);
                    if (dangerForMe < dangerForEnemy * 1.5)
                    {
                        MakeAttackOrder(formation, enemy, true);
                        return true;
                    }
                    if (MoveToAlly(formation, Global.MyArrvs))
                        return true;
                    if (MoveToAlly(formation, Global.MyIfvs))
                        return true;
                    if (MoveToAlly(formation, Global.MyTanks))
                        return true;
                }
            }
            return false;
        }

        private static bool MoveToAlly(MyFormation formation, MyFormation allyFormation)
        {
            if (allyFormation.Alive && allyFormation.Vehicles.Count > 10)
            {
                var actionMove = formation.MoveCenterTo(allyFormation.Center);
                AbortAndAddToExecutingSequence(formation, actionMove);
                return true;
            }
            return false;
        }

        public static bool MakeAttackOrder(MyFormation me, EnemyFormation enemy, bool breakCurrentAction)
        {
            if (!(me.Alive && (!me.Busy || breakCurrentAction)))
                return false;


            var pointToMove = enemy == null
                ? Point.EndOfWorld / 2
                : enemy.MassCenter;


            var myMassCenter = me.MassCenter;
            var distance = pointToMove.Distance(myMassCenter);

            if (enemy != null)
            {
                var enemySpeedScalar = enemy.AvgSpeed.Length();
                var mySpeedScalar = me.MaxSpeed;

                var myDirection = (pointToMove - myMassCenter).Normalized();
                var enemyDirection = enemy.AvgSpeed.Normalized();

                var resultDirection =
                    (myDirection + enemySpeedScalar / mySpeedScalar * enemyDirection).Normalized();

                pointToMove = myMassCenter + distance * resultDirection;
            }

            if (distance > 100)
            {
                pointToMove = (me.Center + pointToMove) / 2;
            }

            if (me.ExecutingAction != null && me.ExecutingAction.ActionType == ActionType.Move)
            {
                var oldVector = new Point(me.ExecutingAction.GetX(), me.ExecutingAction.GetY());


                var newVector = pointToMove - me.Center;

                var scalar = oldVector.Normalized() * newVector.Normalized();
                if (me.Busy && (scalar > 0.95 || // около 18 градусов
                                (oldVector - newVector).Length() < 20))
                {
                    return false;
                }
            }

            var actionMove = me.MoveCenterTo(pointToMove);
            actionMove.MaxSpeed = me.MinSpeed;
            if (me.Density < 0.01 && distance > 200)
            {
                var actionScale = me.ScaleCenter(0.1);
                AbortAndAddToExecutingSequence(me, actionScale, actionMove);
            }
            else
            {
                AbortAndAddToExecutingSequence(me, actionMove);
            }
            me.FacilityAsTarget = null;
            return true;
        }


        public static void RunAwayFromNuclearStrike()
        {
            var enemy = Global.World.GetOpponentPlayer();
            var ahtung = enemy.NextNuclearStrikeTickIndex - Global.World.TickIndex == 29;
            if (ahtung)
            {
                var strikePoint = new Point(enemy.NextNuclearStrikeX, enemy.NextNuclearStrikeY);
                //          Point strikePoint = Global.MyFighters.Center;

                var affected = Global.MyFormations.Values
                    .Where(f => f.Alive &&
                                f.Vehicles.Any() &&
                                f.Rect.IsInside(strikePoint));
                foreach (var formation in affected)
                {
                    var scale1 = formation.ScalePoint(strikePoint, 10);
                    scale1.Interruptable = false;
                    scale1.Priority = ActionPriority.Urgent;
                    scale1.AbortAtWorldTick = Global.World.TickIndex + 30;

                    var scale2 = formation.ScalePoint(strikePoint, 0.1);
                    scale2.Interruptable = false;
                    scale2.Priority = ActionPriority.Urgent;
                    scale2.StartCondition = () => Global.World.TickIndex >= scale1.AbortAtWorldTick;
                    InsertToExecutingSequence(formation, scale1, scale2);
                }
            }
        }

        public static void NuclearStrike()
        {
            if (Global.Me.RemainingNuclearStrikeCooldownTicks == 0 &&
                Global.Me.NextNuclearStrikeTickIndex == -1 &&
                Global.World.TickIndex % 30 == 25 &&
                !Global.ActionQueue.HasActionsFor(ActionType.TacticalNuclearStrike))
            {
                VehicleWrapper possibleTarget = null;
                double bestDeltaDamage = Math.Min(30 * 100, Global.EnemyVehicles.Values.Sum(i => i.Durability) / 5);

                var sqrNuclearRadius =
                    Global.Game.TacticalNuclearStrikeRadius * Global.Game.TacticalNuclearStrikeRadius;

                const double visionRangeThreshold = 0.7;
                foreach (var enemy in Global.EnemyVehicles.Values)
                {
                    var visors = Global.MyVehicles.Values
                        .Where(i => i.SqrDistance(enemy) < visionRangeThreshold * i.VisionRange * i.VisionRange)
                        .ToList();
                    if (visors.Count == 0)
                    {
                        continue;
                    }

                    var allDamaged = Global.AllVehicles.Values
                        .Where(i => i.SqrDistance(enemy) < sqrNuclearRadius)
                        .ToList();

                    var enemyDamaged = allDamaged.Where(i => i.PlayerId != Global.Me.Id).ToList();
                    var myDamaged = allDamaged.Where(i => i.PlayerId == Global.Me.Id).ToList();

                    if (enemyDamaged.Count <= Math.Min(30, Global.EnemyVehicles.Count / 2 - 10))
                    {
                        continue;
                    }

                    var myDamage = myDamaged
                        .Sum(i => Math.Min(i.Durability,
                            Global.Game.MaxTacticalNuclearStrikeDamage *
                            (1 - Math.Sqrt(i.SqrDistance(enemy) / sqrNuclearRadius))));
                    var enemyDamage = enemyDamaged
                        .Sum(i => Math.Min(i.Durability,
                            Global.Game.MaxTacticalNuclearStrikeDamage *
                            (1 - Math.Sqrt(i.SqrDistance(enemy) / sqrNuclearRadius))));

                    if (enemyDamage - myDamage > bestDeltaDamage)
                    {
                        bestDeltaDamage = enemyDamage - myDamage;
                        possibleTarget = enemy;
                    }
                }

                if (possibleTarget == null)
                {
                    return;
                }
                var visor = Global.MyVehicles.Values
                    .Where(i => i.SqrDistance(possibleTarget) < visionRangeThreshold * i.VisionRange * i.VisionRange)
                    .OrderBy(i => i.SqrDistance(possibleTarget)).Last();

                var action = visor.TacticalNuclearStrike(possibleTarget);
                action.Interruptable = false;
                var newSequence = new ActionSequence(action);
                Global.ActionQueue.Add(newSequence);
            }
        }

        public static void InsertToExecutingSequence(MyFormation formation, params Action[] action)
        {
            if (formation != null && formation.Alive && action.Length > 0)
            {
                var executingAction = formation.ExecutingSequence?.GetExecutingAction();

                if (executingAction != null &&
                    executingAction.ActionType == ActionType.Move &&
                    executingAction.Interruptable)
                {
                    action.First().StartCondition = () => true;
                    var continueAction = executingAction.Clone();
                    continueAction.StartCondition = () =>
                        action.Last().Status == ActionStatus.Finished ||
                        action.Last().Status == ActionStatus.Aborted;

                    Global.ActionQueue.AbortOldActionsFor(formation);
                    formation.ExecutingSequence.AddRange(action);
                    formation.ExecutingSequence.Add(continueAction);
                }
                if (executingAction == null)
                {
                    action.First().StartCondition = () => true;
                    var newSequence = new ActionSequence(action);
                    Global.ActionQueue.Add(newSequence);
                }
            }
        }

        public static void AbortAndAddToExecutingSequence(MyFormation formation, params Action[] action)
        {
            if (formation != null && formation.Alive && action.Length > 0)
            {
                var executingAction = formation.ExecutingSequence?.GetExecutingAction();

                if (executingAction != null &&
                    executingAction.ActionType == ActionType.Move &&
                    executingAction.Interruptable)
                {
                    action.First().StartCondition = () => true;
                    Global.ActionQueue.AbortOldActionsFor(formation);
                    formation.ExecutingSequence.AddRange(action);
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
            if (Global.JoinFormation1 != null && Global.JoinFormation1.Alive && Global.JoinFormation1.Count > 0 &&
                Global.JoinFormation2 != null && Global.JoinFormation2.Alive && Global.JoinFormation2.Count > 0)
            {
                var result = FormationFactory.CreateMyFormation(Global.JoinFormation1, Global.JoinFormation2);

                Global.JoinFormation1 = null;
                Global.JoinFormation2 = null;

                Global.ActionQueue.Add(new ActionSequence(result.ActionList.ToArray()));
                return;
            }
            var processedKeys = new List<int>();
            foreach (var key1 in Global.MyFormations.Keys)
            {
                var f1 = Global.MyFormations[key1];
                if (f1.Alive && !Global.IgnoreCollisionGroupIndexes.Contains(key1))
                {
                    foreach (var facility in Global.MyFacilities.Where(f =>
                        f.Type == FacilityType.VehicleFactory && f.VehicleType.HasValue))
                    {
                        var facRect = facility.CreatedVehiclesRect;

                        if (f1.IsAllAeral && facility.CreatedVehicles.All(v => v.IsAerial) ||
                            f1.IsAllGround && facility.CreatedVehicles.All(v => !v.IsAerial))
                        {
                            var distBetweenCenters = f1.Rect.Center.SqrDistance(facRect.Center);
                            if (distBetweenCenters < (f1.Rect.SqrDiameter + facRect.SqrDiameter) / 2)
                            {
                                if (!processedKeys.Contains(key1))
                                {
                                    processedKeys.Add(key1);
                                    var delta = f1.Center - facRect.Center;
                                    var normalizedSpeed = f1.AvgSpeed.Normalized();
                                    var rotated = new Point(normalizedSpeed.Y, -normalizedSpeed.X);
                                    delta =  delta.Normalized();

                                    CompactAndContinue(f1, 50*(delta + rotated/2));
                                }
                            }
                        }
                    }

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

                                if (Global.MyFormations.Count > 15 &&
                                    f1.Count < 50 &&
                                    f2.Count < 50 &&
                                    !processedKeys.Contains(key1) &&
                                    !processedKeys.Contains(key2))
                                {
                                    processedKeys.Add(key1);
                                    processedKeys.Add(key2);
                                    Global.JoinFormation1 = f1;
                                    Global.JoinFormation2 = f2;
                                    return;
                                }
                                else
                                {
                                    if (!processedKeys.Contains(key1))
                                    {
                                        processedKeys.Add(key1);
                                        var delta1 = 20 * (delta.Normalized() + f1.AvgSpeed.Normalized()/2);

                                        CompactAndContinue(f1, delta1);
                                    }
                                    if (!processedKeys.Contains(key2))
                                    {
                                        processedKeys.Add(key2);
                                        var delta2 = 20 * (delta.Normalized() + f2.AvgSpeed.Normalized()/2);
                                        CompactAndContinue(f2, -1 * delta2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CompactAndContinue(MyFormation form, Point delta)
        {
            const double densistyLimit = 0.01;
            var move = form.ShiftTo(delta);

            //     move.Interruptable = false;
            move.IsAnticollision = true;
            if (form.Density < densistyLimit && form.Alive && form.Count > 0)
            {
                var scale1 = form.ScalePoint(form.MassCenter, 0.1);
                //     scale1.Interruptable = false;
                scale1.IsAnticollision = true;
                scale1.AbortAtWorldTick = Global.World.TickIndex +
                                          (int) (Math.Sqrt(form.Rect.SqrDiameter) / (4 * form.MaxSpeed));

                var rotate = form.RotateCenter(Math.PI / 2);
                //   rotate.Interruptable = false;
                rotate.StartCondition = () => Global.World.TickIndex >= scale1.AbortAtWorldTick;
                rotate.AbortAtWorldTick = scale1.AbortAtWorldTick + 10;

                move.StartCondition = () => Global.World.TickIndex >= rotate.AbortAtWorldTick;
                InsertToExecutingSequence(form, scale1, rotate, move);
            }
            else
            {
                InsertToExecutingSequence(form, move);
            }
        }


        public static double DangerFor(this Formation source, Formation dest)
        {
            var destAeralPercent = dest.AeralPercent;
            var oneUnitAeralAttack = Math.Max(0, destAeralPercent * (source.AvgAerialDamage - dest.AvgAerialDefence));
            var oneUnitGroundAttack =
                Math.Max(0, (1 - destAeralPercent) * (source.AvgGroundDamage - dest.AvgGroundDefence));
            var oneUnitAttack = oneUnitGroundAttack + oneUnitAeralAttack;
            var totalAttack = Math.Min(source.Count * oneUnitAttack, dest.Durability);
           // var danger = totalAttack / (dest.Durability + 1);
            // var danger = Math.Max(0, oneUnitAttack / (dest.Durability + 1));
            return totalAttack;
        }

        public static void FinalSpread()
        {
            if (Global.World.TickIndex > 10000 &&
                Global.World.TickIndex % 1000 == 0 &&
                Global.MyVehicles.Count > 10 * Global.EnemyVehicles.Count &&
                Global.World.GetMyPlayer().Score < Global.World.GetOpponentPlayer().Score)
            {
                Global.ActionQueue.Clear();
                foreach (var formation in Global.MyFormations.Values)
                {
                    var scale1 = formation.ScaleCenter(10);
                    var scale2 = formation.ScaleCenter(0.1);
                    var sequence = new ActionSequence(scale1, scale2);
                    Global.ActionQueue.Add(sequence);
                }
            }
        }
    }
}