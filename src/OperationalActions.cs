using System;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class OperationalActions
    {
        public static Action TacticalNuclearStrike(this VehicleWrapper visitor, VehicleWrapper target)
        {
            var action = new Action
            {
                ActionType = ActionType.TacticalNuclearStrike,
                GetX = () => target.X + 30 * target.SpeedX,
                GetY = () => target.Y + 30 * target.SpeedY,
                VehicleId = visitor.Id,
                Urgent = true,
            };
            return action;
        }

        public static Action SetupProduction(this Facility facility, VehicleType type)
        {
            var action = new Action
            {
                ActionType = ActionType.SetupVehicleProduction,
                FacilityId = facility.Id,
                VehicleType = type
            };
            return action;
        }

        public static Action MoveCenterTo(this MyFormation formation, Point point, double maxSpeed = 10)
        {
            return MoveCenterTo(formation, point.X, point.Y);
        }


        public static Action MoveCenterTo(this MyFormation formation, double x, double y, double maxSpeed = 10)
        {
            var action = new Action(formation)
            {
                ActionType = ActionType.Move,
                GetX = () => x - formation.MassCenter.X,
                GetY = () => y - formation.MassCenter.Y,
                MaxSpeed = maxSpeed
            };
            return action;
        }

        public static Action MoveLeftTopTo(this MyFormation formation, Point point, double maxSpeed = 10)
        {
            return MoveLeftTopTo(formation, point.X, point.Y);
        }

        public static Action MoveLeftTopTo(this MyFormation formation, double x, double y, double maxSpeed = 10)
        {
            var action = new Action(formation)
            {
                GetX = () => x - formation.Rect.Left,
                GetY = () => y - formation.Rect.Top,
                ActionType = ActionType.Move,
                MaxSpeed = maxSpeed
            };
            return action;
        }

        public static Action ShiftTo(this MyFormation formation, double x, double y, double maxSpeed = 10)
        {
            var action = new Action(formation)
            {
                ActionType = ActionType.Move,
                GetX = () => x,
                GetY = () => y,
                MaxSpeed = maxSpeed
            };
            return action;
        }

        public static Action ScaleCenter(this MyFormation formation, double factor)
        {
            var action = new Action(formation)
            {
                ActionType = ActionType.Scale,
                Factor = factor,
                GetX = () => formation.MassCenter.X,
                GetY = () => formation.MassCenter.Y
            };
            return action;
        }

        public static Action ScalePoint(this MyFormation formation, Point point, double factor)
        {
            var action = new Action(formation)
            {
                ActionType = ActionType.Scale,
                Factor = factor,
                GetX = () => point.X,
                GetY = () => point.Y
            };
            return action;
        }

        public static Action ScaleLeftTop(this MyFormation formation, double factor)
        {
            var action = new Action(formation)
            {
                ActionType = ActionType.Scale,
                Factor = factor,
                GetX = () => formation.Rect.Left,
                GetY = () => formation.Rect.Top
            };
            return action;
        }

        public static Action RotateCenter(this MyFormation formation, double angle)
        {
            var action = new Action(formation)
            {
                ActionType = ActionType.Rotate,
                Angle = angle,
                GetX = () => formation.MassCenter.X,
                GetY = () => formation.MassCenter.Y
            };
            return action;
        }
    }
}