using System;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Point
    {
        public double X;

        public double Y;

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Point Zero => new Point(0, 0);
        public static Point EndOfWorld => new Point(Global.World.Width, Global.World.Height);

        public static Point Infinity => new Point(Global.World.Width, Global.World.Height);

        public Point Normalized()
        {
            return this / Length();
        }

        public double SqrLength()
        {
            return SqrDistance(Zero);
        }

        public double Length()
        {
            return Distance(Zero);
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

        public static double operator *(Point a, Point b)
        {
            return a.X * b.X + a.Y * b.Y;
        }


        public static Point operator *(Point a, double factor)
        {
            return new Point(factor * a.X, factor * a.Y);
        }

        public static Point operator *(double factor, Point a)
        {
            return a * factor;
        }

        public static Point operator /(Point a, double factor)
        {
            if (factor < 0.00001)
            {
                return Zero;
            }
            return new Point(a.X / factor, a.Y / factor);
        }


        public override string ToString()
        {
            return $"({X:000.0}; {Y:000.0})";
        }
    }
}