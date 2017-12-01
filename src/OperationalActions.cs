namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class OperationalActions
    {
        public static Action MoveCenterTo(this Formation formation, double x, double y, double maxSpeed = 10)
        {
            var action = new Action(formation)
            {
                ActionType = Model.ActionType.Move,
                GetX = () => x - formation.Rect.Left,
                GetY = () => y - formation.Rect.Top,
                MaxSpeed = maxSpeed
            };
            return action;
        }

        public static Action MoveLeftTopTo(this Formation formation, double x, double y, double maxSpeed = 10)
        {
            var action = new Action(formation)
            {
                ActionType = Model.ActionType.Move,
                GetX = () => x - formation.MassCenter.X,
                GetY = () => y - formation.MassCenter.Y,
                MaxSpeed = maxSpeed
            };
            return action;
        }

        public static Action ShiftTo(this Formation formation, double x, double y, double maxSpeed = 10)
        {
            var action = new Action(formation)
            {
                ActionType = Model.ActionType.Move,
                GetX = () => x,
                GetY = () => y,
                MaxSpeed = maxSpeed
            };
            return action;
        }

        public static Action ScaleCenter(this Formation formation, double factor)
        {
            var action = new Action(formation)
            {
                ActionType = Model.ActionType.Scale,
                Factor = factor,
                GetX = () => formation.MassCenter.X,
                GetY = () => formation.MassCenter.Y,
            };
            return action;
        }

        public static Action ScaleLeftTop(this Formation formation, double factor)
        {
            var action = new Action(formation)
            {
                ActionType = Model.ActionType.Scale,
                Factor = factor,
                GetX = () => formation.Rect.Left,
                GetY = () => formation.Rect.Top,
            };
            return action;
        }

        public static Action RotateCenter(this Formation formation, double angle)
        {
            var action = new Action(formation)
            {
                ActionType = Model.ActionType.Rotate,
                Angle = angle,
                GetX = () => formation.MassCenter.X,
                GetY = () => formation.MassCenter.Y,
            };
            return action;
        }
    }
}