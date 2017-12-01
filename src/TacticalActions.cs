﻿using System;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class TacticalActions
    {
        public static Formation CreateAirFormation()
        {
            const double eps = 10D;
            const double deltaShift = 5.1D;
            const double commonCoordinate = 250D;
            const double nearCoordinate = 60D;
            const double farCoordinate = 200D;
            const double factor = 1.7D;
            const double vehicleSize = 4D;


            var fighters = Global.MyFighters;
            var helicopters = Global.MyHelicopters;

            var isVertical = Math.Abs(fighters.Rect.Left - helicopters.Rect.Left) < eps;

            Formation f1;
            Formation f2;
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
            var res = FormationFactory.CreateFormation(
                () => Math.Min(f1.Rect.Left, f2.Rect.Left),
                () => Math.Min(f1.Rect.Top, f2.Rect.Top),
                () => Math.Max(f1.Rect.Right, f2.Rect.Right) - compactX,
                () => Math.Max(f1.Rect.Bottom, f2.Rect.Bottom) - compactY);

            var sShift = new ActionSequence(res.ActionList.ToArray());
            sShift.First().StartCondition = () => sPenetrate2.IsFinished && sPenetrate1.IsFinished;

            sShift.Add(res.Formation.ShiftTo(shiftX, shiftY));
            Global.ActionQueue.Add(sShift);

            //   компактизируем  
            res = FormationFactory.CreateFormation(
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

        public static void SplitFormation(Formation formation, int runFromCenter = 0)
        {
            var child = FormationFactory.CreateFormation(
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

            child = FormationFactory.CreateFormation(
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

            child = FormationFactory.CreateFormation(
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
            child = FormationFactory.CreateFormation(
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
    }
}