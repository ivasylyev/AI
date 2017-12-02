namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model
{
    public partial class Facility
    {
        public bool IsMine => OwnerPlayerId == Global.World.GetMyPlayer().Id;
        public int? SelectedAsTargetForGroup { get; set; }

        public Rect Rect => new Rect(Left, Top, Left + Global.Game.FacilityWidth, Top + Global.Game.FacilityHeight);
        public Point Center => Rect.Center;
    }
}