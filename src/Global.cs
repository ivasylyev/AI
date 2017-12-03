using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class Global
    {
        private static Map _map;
        private static Map _detailMap;

        public static Player Me;
        public static World World;
        public static Game Game;
        public static Move Move;

        public static readonly Dictionary<int, MyFormation> MyFormations = new Dictionary<int, MyFormation>();
        public static readonly List<int> IgnoreCollisionGroupIndexes = new List<int>();
        public static readonly List<EnemyFormation> EnemyFormations = new List<EnemyFormation>();

        public static readonly Dictionary<long, VehicleWrapper> AllVehicles = new Dictionary<long, VehicleWrapper>();
        public static readonly Dictionary<long, VehicleWrapper> MyVehicles = new Dictionary<long, VehicleWrapper>();
        public static readonly Dictionary<long, VehicleWrapper> EnemyVehicles = new Dictionary<long, VehicleWrapper>();


        public static IEnumerable<Facility> MyFacilities => World?.Facilities.Where(f => f.IsMine);

        public static MyFormation MyFighters => MyFormations[1 + (int) VehicleType.Fighter];
        public static MyFormation MyHelicopters => MyFormations[1 + (int) VehicleType.Helicopter];
        public static MyFormation MyArrvs => MyFormations[1 + (int) VehicleType.Arrv];
        public static MyFormation MyIfvs => MyFormations[1 + (int) VehicleType.Ifv];
        public static MyFormation MyTanks => MyFormations[1 + (int) VehicleType.Tank];

        public static IEnumerable<VehicleWrapper> EnemyFighters =>
            EnemyVehicles.Values.Where(v => v.Type == VehicleType.Fighter);

        public static IEnumerable<VehicleWrapper> EnemyHelicopters =>
            EnemyVehicles.Values.Where(v => v.Type == VehicleType.Helicopter);

        public static IEnumerable<VehicleWrapper> EnemyArrvs =>
            EnemyVehicles.Values.Where(v => v.Type == VehicleType.Arrv);

        public static IEnumerable<VehicleWrapper> EnemyIfvs =>
            EnemyVehicles.Values.Where(v => v.Type == VehicleType.Ifv);

        public static IEnumerable<VehicleWrapper> EnemyTanks =>
            EnemyVehicles.Values.Where(v => v.Type == VehicleType.Tank);

        public static MyFormation SelectedFormation;
        public static MyFormation MyAirFormation;

        public static Map Map => _map ?? (_map = new Map(tilesPerSide: 16));
        public static Map DetailMap => _detailMap ?? (_detailMap = new Map(tilesPerSide: 128));


        public static readonly ActionQueue ActionQueue = new ActionQueue();

        public static readonly Random Random = new Random();
    }
}