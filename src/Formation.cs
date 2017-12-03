using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Formation
    {
        public Formation()
        {
            Vehicles = new Dictionary<long, VehicleWrapper>();
        }

        public Dictionary<long, VehicleWrapper> Vehicles { get; }

        public VehicleType Type { get; set; }
        public int GroupIndex { get; set; }

        public Rect Rect { get; private set; }
        public Point MassCenter { get; private set; }
        public Point Center => Rect.Center;

        public double Density
        {
            get
            {
                if (Vehicles.Count == 0)
                {
                    return 0;
                }
                var radius =
                    Rect.Center.Distance(Vehicles.Values.OrderBy(i => Rect.Center.SqrDistance(i)).Last());
                var sq = Math.PI * radius * radius;
                return Vehicles.Count / sq;
            }
        }

        public Point AvgSpeed => new Point(AvgSpeedX, AvgSpeedY);

        public double AvgSpeedX
        {
            get
            {
                if (Vehicles.Count == 0)
                {
                    return 0;
                }
                return Vehicles.Values.Average(v => v.SpeedX);
            }
        }

        public double AvgSpeedY
        {
            get
            {
                if (Vehicles.Count == 0)
                {
                    return 0;
                }
                return Vehicles.Values.Average(v => v.SpeedY);
            }
        }

        public bool IsStanding => !Vehicles.Any() || Vehicles.Values.All(v => v.IsStanding);

        public bool IsAllAeral => Vehicles.Values.All(v => v.IsAerial);
        public bool IsAllGround => Vehicles.Values.All(v => !v.IsAerial);
        public bool IsMixed => !IsAllAeral && !IsAllGround;

        public virtual void Update(IEnumerable<VehicleUpdate> updates = null)
        {
            if (updates != null)
            {
                foreach (var update in updates)
                {
                    if (update.Durability <= 0)
                    {
                        Vehicles.Remove(update.Id);
                    }
                }
            }
            Rect = Vehicles.Any() ? new Rect(Vehicles.Values) : new Rect(0, 0, 0, 0);
            MassCenter = Vehicles.Any()
                ? new Point(Vehicles.Values.Average(i => i.X), Vehicles.Values.Average(i => i.Y))
                : Point.Zero;
        }


        public override string ToString()
        {
            return $"{Type}, Rect:{Rect}";
        }
    }
}