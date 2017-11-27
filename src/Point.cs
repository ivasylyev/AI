using System;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Point
    {
        public double X;

        public double Y;

        public static Point Zero => new Point(0, 0);

        public static Point Infinity => new Point(Global.World.Width, Global.World.Height);

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double Distance(double x, double y)
        {
            return Math.Sqrt(SqrDistance(x, y));
        }

        public double SqrDistance(double x, double y)
        {
            var dx = X - x;
            var dy = Y - y;

            return dx * dx + dy * dy;
        }


        public double Distance(Point other)
        {
            return Distance(other.X, other.Y);
        }

        public double SqrDistance(Point other)
        {
            return SqrDistance(other.X, other.Y);
        }


        public double Distance(VehicleWrapper unit)
        {
            return Distance(unit.X, unit.Y);
        }

        public double SqrDistance(VehicleWrapper unit)
        {
            return SqrDistance(unit.X, unit.Y);
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }


        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public override string ToString()
        {
            return $"({X}; {Y})";
        }
    }
}