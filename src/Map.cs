using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Map
    {
        private readonly Tile[,] _tiles;
        private readonly int _length;

        public int TilesPerSide { get; }

        public Tile this[int x, int y] => _tiles[x, y];
        public Tile this[Point center] => _tiles[GetTileX(center.X), GetTileY(center.Y)];

        public Tile[,] Tiles => _tiles;

        public Map(int tilesPerSide)
        {
            TilesPerSide = tilesPerSide;
            _length = (int) Math.Round(Global.World.Width / tilesPerSide);

            _tiles = new Tile[tilesPerSide, tilesPerSide];

            for (int x = 0; x < tilesPerSide; x++)
            {
                for (int y = 0; y < tilesPerSide; y++)
                {
                    _tiles[x, y] = new Tile(x, y, _length);
                }
            }
            foreach (var tile in _tiles)
            {
                tile.SetupNeighbors(this);
            }
        }

        public void Update()
        {
            foreach (var tile in _tiles)
            {
                tile.Clear();
            }
            foreach (VehicleWrapper vehicle in Global.EnemyVehicles.Values)
            {
                var tileX = GetTileX(vehicle.X);
                var tileY = GetTileY(vehicle.Y);
                _tiles[tileX, tileY].AddEnemy(vehicle);
            }
        }

        public IReadOnlyCollection<VehicleWrapper> GetSpotters()
        {
            var result = new List<VehicleWrapper>();
            foreach (Tile tile in _tiles)
            {
                if (tile.Spotters.Count == 0)
                {
                    continue;
                }
                result.AddRange(tile.Spotters);
            }
            return result;
        }


        private int GetTileY(double y)
        {
            return (int) Math.Floor(y / _length);
        }

        private int GetTileX(double x)
        {
            return (int) Math.Floor(x / _length);
        }
    }
}