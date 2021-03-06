﻿using System.Collections.Generic;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Rect
    {
        public Point LeftTop = new Point(int.MaxValue, int.MaxValue);

        public Point RightBottom = new Point(0, 0);

        public double Left => LeftTop.X;
        public double Top => LeftTop.Y;


        public double Right => RightBottom.X;
        public double Bottom => RightBottom.Y;


        public Point Center => new Point((Right + Left) / 2, (Bottom + Top) / 2);
        public double SqrDiameter => (Right - Left) * (Right - Left) + (Bottom - Top) * (Bottom - Top);

        public Rect(double left, double top, double right, double bottom)
        {
            LeftTop = new Point(left, top);
            RightBottom = new Point(right, bottom);
        }
        public Rect(Point leftTop, Point rightBottom)
        {
            LeftTop = leftTop;
            RightBottom = rightBottom;
        }

        public Rect(IEnumerable<VehicleWrapper> vehicles)
        {
            foreach (var vehicle in vehicles)
            {
                if (vehicle.X < Left)
                {
                    LeftTop.X = vehicle.X;
                }
                if (vehicle.Y < Top)
                {
                    LeftTop.Y = vehicle.Y;
                }
                if (vehicle.X > Right)
                {
                    RightBottom.X = vehicle.X;
                }
                if (vehicle.Y > Bottom)
                {
                    RightBottom.Y = vehicle.Y;
                }
            }
        }

        public bool IsInside(double x, double y)
        {
            return x >= Left && y >= Top && x <= Right && y <= Bottom;
        }
        public bool IsInside(Point p)
        {
            return p.X >= Left && p.Y >= Top && p.X <= Right && p.Y <= Bottom;
        }

        public override string ToString()
        {
            return $"({Left:000.0}; {Top:000.0}) ({Right:000.0}; {Bottom:000.0}); Center ({Center.X:000.0}; {Center.Y:000.0})";
        }
    }
}