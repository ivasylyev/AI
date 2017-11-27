using System;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class VehicleWrapper
    {
        private const int HistoryLength = 10;
        private readonly Point[] _history = new Point[HistoryLength];
        public long Id { get; set; }
        public VehicleType Type { get; set; }
        public long TickIndex { get; private set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Durability { get; set; }
        public long PlayerId { get; set; }

        public Point Speed => new Point(SpeedX, SpeedY);
        public double SpeedX => (_history[0].X - _history[HistoryLength - 1].X) / HistoryLength;
        public double SpeedY => (_history[0].Y - _history[HistoryLength - 1].Y) / HistoryLength;

        public bool IsStanding => SpeedX < 0.001 && SpeedY < 0.001;


        public double VisionRange { get; set; }

        public static implicit operator VehicleWrapper(Vehicle vehicle)
        {
            var wrapper = new VehicleWrapper
            {
                Id = vehicle.Id,
                Type = vehicle.Type,
                X = vehicle.X,
                Y = vehicle.Y,
                Durability = vehicle.Durability,
                PlayerId = vehicle.PlayerId,
                VisionRange = vehicle.VisionRange
            };
            for (var i = 0; i < HistoryLength; i++)
            {
                wrapper._history[i] = new Point(0, 0);
            }
            wrapper.Update();
            return wrapper;
        }


        public void Update(VehicleUpdate vehicleUpdate)
        {
            X = vehicleUpdate.X;
            Y = vehicleUpdate.Y;
            Durability = vehicleUpdate.Durability;

            Update();
        }

        public void Update()
        {
            ShiftHistory(_history, X, Y);
            TickIndex = Global.World.TickIndex;
        }

        public double SqrDistance(VehicleWrapper to)
        {
            var x = to.X - X;
            var y = to.Y - Y;

            return (x * x + y * y);
        }

        public double Distance(VehicleWrapper to)
        {
            return Math.Sqrt(SqrDistance(to));
        }

        public bool IsInside(Rect rect)
        {
            return rect.IsInside(X, Y);
        }

        private void ShiftHistory(Point[] history, double newX, double newY)
        {
            for (var i = history.Length - 1; i > 0; i--)
            {
                history[i] = history[i - 1];
            }
            history[0] = new Point(newX, newY);
        }

        public override string ToString()
        {
            return $"({X};{Y}) {Type}; Speed:{Speed}";
        }
    }
}