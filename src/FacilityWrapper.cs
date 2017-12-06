using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model
{
    public partial class Facility
    {
        public bool IsMine => OwnerPlayerId == Global.World.GetMyPlayer().Id;
        public int? SelectedAsTargetForGroup { get; set; }
        public int InitialTickIndex { get; set; }

        public Rect Rect => new Rect(Left, Top, Left + Global.Game.FacilityWidth, Top + Global.Game.FacilityHeight);
        public Point Center => Rect.Center;

        public Tile BigTile => Global.Map[Center];

        public int NearGroundEnemyCount
        {
            get
            {
                Rect r = new Rect(Center.X - 100, Center.Y - 100, Center.X + 100, Center.Y + 100);
                return Global.EnemyVehicles.Values.Count(v => !v.IsAerial && v.IsInside(r));
            }
        }

        public List<VehicleWrapper> CreatedVehicles
        {
            get
            {
                return Global.MyVehicles.Values
                    .Where(v => v.Groups.Length == 0 && v.IsInside(Rect) && v.IsStanding)
                    .ToList();
            }
        }

        public Rect CreatedVehiclesRect
        {
            get
            {
                List<VehicleWrapper> createdVehicles = CreatedVehicles;
                if (createdVehicles.Any())
                {
                    Rect createdRect = new Rect(createdVehicles.Min(v => v.X),
                        createdVehicles.Min(v => v.Y),
                        createdVehicles.Max(v => v.X),
                        createdVehicles.Max(v => v.Y));

                    return createdRect;
                }
                return new Rect(0,0,0,0);
            }
        }
    }
}