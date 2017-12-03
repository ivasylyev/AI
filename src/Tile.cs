using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Tile
    {
         
        public static readonly Tile Empty = new Tile(0, 0, 0);

        private readonly int _x;
        private readonly int _y;
        private readonly List<Tile> _neighbors;
        private List<VehicleWrapper> _enemiesAround;

        public Rect TileFrame { get; }
        public List<VehicleWrapper> Enemies { get; }

        public Tile NW { get; private set; }
        public Tile N { get; private set; }
        public Tile NE { get; private set; }

        public Tile W { get; private set; }
        public Tile E { get; private set; }

        public Tile SW { get; private set; }
        public Tile S { get; private set; }
        public Tile SE { get; private set; }

        public List<Tile> Neighbors => _neighbors;

        public Tile(int x, int y, int length)
        {
            _x = x;
            _y = y;
            TileFrame = new Rect(new Point(x * length, y * length), new Point((x + 1) * length, (y + 1) * length));
            Enemies = new List<VehicleWrapper>();
            _neighbors = new List<Tile>(8);
        }

        public IList<VehicleWrapper> Spotters
        {
            get
            {
                if (Enemies.Count == 1
                    && Enemies[0].Type == VehicleType.Fighter
                    && _neighbors.Sum(x => x.Enemies.Count) < 5)
                {
                    return Enemies;
                }
                return Array.Empty<VehicleWrapper>();
            }
        }

        public int EnemiesAroundCount => Enemies.Count + _neighbors.Sum(x => x.Enemies.Count);

        public List<VehicleWrapper> EnemiesAround
        {
            get
            {
                if (_enemiesAround == null)
                {
                    _enemiesAround = new List<VehicleWrapper>(EnemiesAroundCount);
                    _enemiesAround.AddRange(Enemies);
                    foreach (Tile neighbor in _neighbors)
                    {
                        _enemiesAround.AddRange(neighbor.Enemies);
                    }
                }
                return _enemiesAround;
            }
        }

        public Point Center { get; private set; }

        public void SetupNeighbors(Map map)
        {
            // -1-1   0-1 +1-1
            // -1 0  ---- +1 0
            // -1+1   0+1 +1+1
            _neighbors.Add(NW = GetTile(map, -1, -1));
            _neighbors.Add(N = GetTile(map, 0, -1));
            _neighbors.Add(NE = GetTile(map, +1, -1));

            _neighbors.Add(W = GetTile(map, -1, 0));
            _neighbors.Add(E = GetTile(map, +1, 0));

            _neighbors.Add(SW = GetTile(map, -1, +1));
            _neighbors.Add(S = GetTile(map, 0, +1));
            _neighbors.Add(SE = GetTile(map, +1, +1));
        }

        public bool Includes(Point point)
        {
            return TileFrame.IsInside(point);
        }

        public void Clear()
        {
            Enemies.Clear();
            _enemiesAround = null;
        }

        public void AddEnemy(VehicleWrapper vehicle)
        {
            Enemies.Add(vehicle);
        }

        private Tile GetTile(Map map, int xOffset, int yOffset)
        {
            var x = _x + xOffset;
            var y = _y + yOffset;
            if (x < 0 || y < 0 || x >= map.TilesPerSide || y >= map.TilesPerSide)
            {
                return Empty;
            }
            return map[x, y];
        }

        public override string ToString()
        {
            return $"{TileFrame}, Enemies:{Enemies.Count}";
        }
    }
}
