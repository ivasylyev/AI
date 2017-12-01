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

        public static Formation SelectedFormation;
        public static Formation MyAirFormation;

        public static Formation MyFighters => Formations[-(int) VehicleType.Fighter];
        public static Formation MyHelicopters => Formations[-(int) VehicleType.Helicopter];

        public static Formation MyArrvs => Formations[-(int) VehicleType.Arrv];
        public static Formation MyIfvs => Formations[-(int) VehicleType.Ifv];
        public static Formation MyTanks => Formations[-(int) VehicleType.Tank];

        public static readonly Dictionary<long, VehicleWrapper> AllVehicles = new Dictionary<long, VehicleWrapper>();
        public static readonly Dictionary<long, VehicleWrapper> MyVehicles = new Dictionary<long, VehicleWrapper>();
        public static readonly Dictionary<long, VehicleWrapper> EnemyVehicles = new Dictionary<long, VehicleWrapper>();

        public static readonly Dictionary<int, Formation> Formations = new Dictionary<int, Formation>();

        public static readonly ActionQueue ActionQueue = new ActionQueue();

        public static readonly Random Random = new Random();
    }
}