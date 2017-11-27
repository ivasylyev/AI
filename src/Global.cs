using System;
using System.Collections.Generic;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class Global
    {
        public static Player Me;
        public static World World;
        public static Game Game;
        public static Move Move;

        public static readonly Dictionary<long, VehicleWrapper> AllVehicles = new Dictionary<long, VehicleWrapper>();
        public static readonly Dictionary<long, VehicleWrapper> MyVehicles = new Dictionary<long, VehicleWrapper>();
        public static readonly Dictionary<long, VehicleWrapper> EnemyVehicles = new Dictionary<long, VehicleWrapper>();

        public static readonly Dictionary<long, Formation> Formations = new Dictionary<long, Formation>();


        public static readonly Queue<Move> ActionQueue = new Queue<Move>();

        public static double factor = 1.5;
        public static double betweenSquares = 74;

        public static readonly Random Random = new Random();
    }
}