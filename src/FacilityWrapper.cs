using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model
{
    public partial class Facility
    {
        public bool IsMine => OwnerPlayerId == Global.World.GetMyPlayer().Id;
        public int? SelectedAsTargetForGroup { get; set; }

        public Rect Rect => new Rect(Left, Top, Left + Global.Game.FacilityWidth, Top + Global.Game.FacilityHeight);
        public Point Center => Rect.Center;

        public Tile BigTile => Global.Map[Center];

        public int NearGroundEnemyCount
        {
            get
            {
                
                Rect r = new Rect(Center.X-100, Center.Y-100, Center.X+100, Center.Y+100);
                return Global.EnemyVehicles.Values.Count(v => !v.IsAerial && v.IsInside(r));
            }
        }

    }
}