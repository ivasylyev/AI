using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed class MyOldStrategy 
    {
        private static readonly Dictionary<long, VehicleWrapper> AllVehicles = new Dictionary<long, VehicleWrapper>();
        private static readonly Dictionary<long, VehicleWrapper> MyVehicles = new Dictionary<long, VehicleWrapper>();
        private static readonly Dictionary<long, VehicleWrapper> EnemyVehicles = new Dictionary<long, VehicleWrapper>();
        private static readonly Queue<Move> ActionQueue = new Queue<Move>();

        private static Player _me;
        private static World _world;
        private static Game _game;
        private static Move _move;

        private static int _standingCount;
        private static bool IsStandingLongTime => _standingCount > 10;
        private static bool IsStandingVeryLongTime => _standingCount > 70;

        private static double _myMinX;
        private static double _myMaxX;
        private static double _myAvgX;
        private static double _myMinY;
        private static double _myMaxY;
        private static double _myAvgY;

        private static double _enemyAvgX;
        private static double _enemyAvgY;

        private static bool _allScaled;
        private static bool _horisontalMoved;
        private static bool _verticalScaled;
        private static bool _verticalShiftedCenter;
        private static bool _verticalMoved;

        private static bool _finalShifted;
        private static bool _finalScaled;
        private static bool _ready;

        private static List<double> _scalingVehicleY;
        private static long? _kamikadzeId;
        private static readonly List<long> _kamikadzeIdHistory = new List<long>();

        private static readonly bool[,] _vehicleMatrix = new bool[3, 3];

        private static int _runAwayTickCount;
        private static bool _hasRunAway;
        private readonly Random _random = new Random();


        public void Move(Player me, World world, Game game, Move move)
        {
            try
            {
                Init(me, world, game, move);
                //  SelectionTest();

                RunAwayFromNuclearStrike();

                KamikadzeMove();
                NuclearStrike2();

                MixVehicles();
                RotateAndMove();

                ProcessQueue();
            }
            catch (Exception ex)
            {
                var exception = ex;
            }
        }

        private void SelectionTest()
        {
            if (_world.TickIndex > 100 && _world.TickIndex % 100 == 0)
            {
                SelectAllVehiclesExceptKamikadze();
                RotateAllVehicles(Math.PI / 2);
            }
        }


        private void MixVehicles()
        {
            var factor = 1.5;
            var betweenSquares = 74;
            if (_world.TickIndex == 0)
            {
                SelectVehicles(0, 82, 230, 156);
                MoveVehicles(3, 0, 10);
            }
            else if (_world.TickIndex > 20 && !_allScaled)
            {
                SelectAllVehicles();
                Scale(0, 0, factor);


                _allScaled = true;
                _standingCount = 0;
            }
            else if (_allScaled && !_verticalMoved && IsStandingLongTime)
            {
                SelectVehicles(0, factor * 156, factor * 230, factor * 230);
                MoveVehicles(0, -factor * betweenSquares, 10);

                SelectVehicles(0, 0, factor * 230, factor * 82);
                MoveVehicles(0, factor * betweenSquares, 10);

                _verticalMoved = true;
                _standingCount = 0;
            }
            else if (_verticalMoved && !_verticalScaled && IsStandingLongTime)
            {
                _scalingVehicleY = MyVehicles.Values
                                             .Where(v => v.Type != VehicleType.Fighter &&
                                                         v.Type != VehicleType.Helicopter)
                                             .Select(v => (double)(int)v.Y).Distinct().OrderBy(y => y).ToList();
                if (_scalingVehicleY.Count > 20)
                {
                    // ���-�� ����� �� ���
                    _verticalShiftedCenter = true;
                    _horisontalMoved = true;
                }
                else
                {
                    for (var i = 0; i < _scalingVehicleY.Count; i++)
                    {
                        SelectVehicles(0, _scalingVehicleY[i], _world.Width / 2, _scalingVehicleY[i] + 2);
                        MoveVehicles(0, 4.5 * (i - _scalingVehicleY.Count / 2), 10);
                    }

                    _standingCount = 0;
                }

                _verticalScaled = true;
            }

            else if (_verticalScaled && !_verticalShiftedCenter && IsStandingLongTime)
            {
                SelectVehicles(factor * 82, 0, factor * 156, factor * 230);
                MoveVehicles(0, factor * 3, 10);

                _verticalShiftedCenter = true;
                _standingCount = 0;
            }
            else if (_verticalShiftedCenter && !_horisontalMoved && IsStandingVeryLongTime)
            {
                SelectVehicles(factor * 156, 0, factor * 230, factor * 230);
                MoveVehicles(-factor * betweenSquares, 0, 10);

                SelectVehicles(0, 0, factor * 82, factor * 230);
                MoveVehicles(factor * betweenSquares, 0, 10);

                _horisontalMoved = true;
                _standingCount = 0;
            }
            else if (_horisontalMoved && !_finalShifted && IsStandingVeryLongTime)
            {
                var height = (_myMaxY - _myMinY);
                var middle = _myMinY + height / 2;
                SelectVehicles(_myMinX, _myMinY, _myMaxX, middle - 4);
                MoveVehicles(0, height / 2, 10);

                SelectVehicles(_myMinX, middle + 4, _myMaxX, _myMaxY);
                MoveVehicles(0, -height / 2, 10);

                _finalShifted = true;
            }
            else if (_finalShifted && !_finalScaled && IsStandingLongTime)
            {
                SelectAllVehiclesExceptKamikadze();
                ScaleVehicles(AggregateOperation.Average, 0.2);

                _finalScaled = true;
            }
            else if (_finalScaled && IsStandingLongTime)
            {
                _ready = true;
            }
        }


        private void RunAwayFromNuclearStrike()
        {
            var enemy = _world.GetOpponentPlayer();

            if (_ready && !_hasRunAway && MyVehicles.Values.Any() && enemy.NextNuclearStrikeTickIndex - _world.TickIndex == 29)
            {
                SelectAllVehiclesExceptKamikadze();
                Scale(enemy.NextNuclearStrikeX, enemy.NextNuclearStrikeY, 3);
                _hasRunAway = true;
                _runAwayTickCount = 0;
            }
            else if (_hasRunAway && _runAwayTickCount == 40)
            {
                _hasRunAway = false;
                SelectAllVehiclesExceptKamikadze();
                ScaleVehicles(AggregateOperation.Average, 0.1);
            }
            _runAwayTickCount++;
        }

        private void KamikadzeMove()
        {
            //_ready &&
            if (!IsNear() && _world.TickIndex % 30 == 0 && MyVehicles.Any() && EnemyVehicles.Any())
            {
                if (!_kamikadzeId.HasValue || _world.TickIndex % 180 == 0)
                {
                    var kamikadze =
                        MyVehicles.Values.Where(v => v.Type == VehicleType.Fighter && v.Durability > 50)
                                  .OrderBy(v => (v.X - _enemyAvgX) * (v.X - _enemyAvgX) + (v.Y - _enemyAvgY) * (v.Y - _enemyAvgY))
                                  .FirstOrDefault();
                    _kamikadzeId = kamikadze?.Id;
                    if (_kamikadzeId.HasValue && !_kamikadzeIdHistory.Contains(_kamikadzeId.Value))
                    {
                        _kamikadzeIdHistory.Add(_kamikadzeId.Value);
                    }
                }
                if (_kamikadzeId.HasValue)
                {
                    if (MyVehicles.ContainsKey(_kamikadzeId.Value))
                    {
                        var kamikadze = MyVehicles[_kamikadzeId.Value];

                        var nearEnemyes = EnemyVehicles.Values
                                                       .OrderBy(enemy => SqrDistance(kamikadze, enemy))
                                                       .ToList();
                        if (nearEnemyes.Any())
                        {
                            var nearestEnemy = nearEnemyes[0];
                            if (nearEnemyes.Count > 10)
                            {
                                if (SqrDistance(nearEnemyes[0], nearEnemyes[10]) > 10000 && SqrDistance(nearEnemyes[5], nearEnemyes[10]) < 2500)
                                {
                                    nearestEnemy = nearEnemyes[10];
                                }
                            }

                            var vectorX = nearestEnemy.X - kamikadze.X;
                            var vectorY = nearestEnemy.Y - kamikadze.Y;
                            var length = Math.Sqrt(vectorY * vectorY + vectorX * vectorX);
                            if (length > 0.01)
                            {
                                SelectVehicles(kamikadze.X - .01, kamikadze.Y - .01, kamikadze.X + .01, kamikadze.Y + .01);

                                var isStanding = kamikadze.SpeedX < 0.01 && kamikadze.SpeedY < 0.01;

                                var newX = isStanding
                                               ? kamikadze.X + 5 * _random.NextDouble()
                                               : vectorX * (1 - Math.Sign(vectorX) * _game.TacticalNuclearStrikeRadius / length);
                                var newY = isStanding
                                               ? kamikadze.Y + 5 * _random.NextDouble()
                                               : vectorY * (1 - Math.Sign(vectorX) * _game.TacticalNuclearStrikeRadius / length);

                                var move = new Move
                                {
                                    Action = ActionType.Move,
                                    X = newX,
                                    Y = newY,
                                    VehicleId = kamikadze.Id,
                                    MaxSpeed = 10
                                };
                                ActionQueue.Enqueue(move);
                            }
                        }
                    }
                }
            }
        }


        private void RotateAndMove()
        {
            var timeToMix = 1350;
            var timeToMixAndMove = 1700;

            var index = _world.TickIndex % 120;
            if (EnemyVehicles.Values.Any() && MyVehicles.Values.Any() && _ready && _runAwayTickCount > 80)
            {
                if (_world.TickIndex > timeToMix && _world.TickIndex < timeToMixAndMove && IsStandingLongTime)
                {
                    if (index == 0)
                    {
                        SelectAllVehiclesExceptKamikadze();
                        RotateAllVehicles(Math.PI / 2);
                    }

                    if (index == 100)
                    {
                        SelectAllVehiclesExceptKamikadze();
                        ScaleVehicles(AggregateOperation.Average, 0.9);
                    }
                }
                else if (_world.TickIndex > timeToMixAndMove)
                {
                    if (index == 0)
                    {
                        if (!IsNear())
                        {
                            SelectAllVehiclesExceptKamikadze();
                            RotateAllVehicles(Math.PI / 6);
                        }
                    }

                    if (index == 40)
                    {
                        if (!IsNear() || EnemyVehicles.Count < MyVehicles.Count)
                        {
                            SelectAllVehiclesExceptKamikadze();
                            ScaleVehicles(AggregateOperation.Average, 0.2);
                        }
                    }
                    if (index == 80)
                    {
                        SelectAllVehiclesExceptKamikadze();

                        MoveVehicles(_enemyAvgX - _myAvgX, _enemyAvgY - _myAvgY);
                    }
                }
            }
        }

        private bool IsNear()
        {
            var near = EnemyVehicles.Values
                                    .Count(
                                        enemy =>
                                        MyVehicles.Values.Where(v => v.Type != VehicleType.Fighter)
                                                  .Any(i => SqrDistance(i, enemy) < i.VisionRange * i.VisionRange));
            return near > 20;
        }

        private void SelectAllVehicles()
        {
            var move = new Move
            {
                Action = ActionType.ClearAndSelect,
                Right = _world.Width,
                Bottom = _world.Height
            };
            ActionQueue.Enqueue(move);
        }


        private void SelectAllVehiclesExceptKamikadze()
        {
            var move = new Move
            {
                Action = ActionType.ClearAndSelect,
                Left = _myMinX,
                Top = _myMinY,
                Right = _myMaxX,
                Bottom = _myMaxY
            };
            ActionQueue.Enqueue(move);
            //
            //            if (_kamikadzeId.HasValue && MyVehicles.ContainsKey(_kamikadzeId.Value))
            //            {
            //                var kamikadze = MyVehicles[_kamikadzeId.Value];
            //
            //                DeselectVehicles(kamikadze.X - 1, kamikadze.Y - 1, kamikadze.X + 1, kamikadze.Y + 1);
            //            }
        }


        private void SelectVehicles(double left, double top, double right, double bottom)
        {
            var move = new Move
            {
                Action = ActionType.ClearAndSelect,
                Left = left,
                Top = top,
                Right = right,
                Bottom = bottom
            };
            ActionQueue.Enqueue(move);
        }

        private void DeselectVehicles(double left, double top, double right, double bottom)
        {
            var move = new Move
            {
                Action = ActionType.Deselect,
                Left = left,
                Top = top,
                Right = right,
                Bottom = bottom
            };
            ActionQueue.Enqueue(move);
        }

        private static void Scale(double x, double y, double factor)
        {
            var move = new Move
            {
                Action = ActionType.Scale,
                Factor = factor,
                X = x,
                Y = y
            };
            ActionQueue.Enqueue(move);
        }

        private void RotateAllVehicles(double angle)
        {
            var move = new Move
            {
                Action = ActionType.Rotate,
                Angle = angle,
                X = _myAvgX,
                Y = _myAvgY
            };
            ActionQueue.Enqueue(move);
        }

        private void ScaleVehicles(AggregateOperation operation, double factor)
        {
            double x;
            double y;

            switch (operation)
            {
                case AggregateOperation.Average:
                    x = _myAvgX;
                    y = _myAvgY;
                    break;
                case AggregateOperation.Min:
                    x = _myMinX;
                    y = _myMinY;
                    break;
                case AggregateOperation.Max:
                    x = _myMaxX;
                    y = _myMaxY;
                    break;
                default:
                    x = _world.Width / 2D;
                    y = _world.Height / 2D;
                    break;
            }


            var move = new Move
            {
                Action = ActionType.Scale,
                Factor = factor,
                X = x,
                Y = y
            };
            ActionQueue.Enqueue(move);
        }


        private void MoveVehicles(double x, double y, double maxSpeed = 0.2)
        {
            var move = new Move
            {
                Action = ActionType.Move,
                X = x,
                Y = y,
                MaxSpeed = maxSpeed
            };
            ActionQueue.Enqueue(move);
        }


        private void Init(Player me, World world, Game game, Move move)
        {
            _me = me;
            _world = world;
            _game = game;
            _move = move;

            foreach (var newVehicle in _world.NewVehicles)
            {
                AllVehicles.Add(newVehicle.Id, newVehicle);
                if (newVehicle.PlayerId == _me.Id)
                {
                    MyVehicles.Add(newVehicle.Id, newVehicle);
                }
                else
                {
                    EnemyVehicles.Add(newVehicle.Id, newVehicle);
                }
            }
            var isStanding = true;
            foreach (var vehicleUpdate in _world.VehicleUpdates)
            {
                var vehickeId = vehicleUpdate.Id;
                if (vehicleUpdate.Durability > 0)
                {
                    VehicleWrapper wrapper;
                    if (MyVehicles.TryGetValue(vehickeId, out wrapper))
                    {
                        if ((Math.Abs(wrapper.X - vehicleUpdate.X) > 0.01 || Math.Abs(wrapper.Y - vehicleUpdate.Y) > 0.01)
                            && !_kamikadzeIdHistory.Contains(wrapper.Id))
                        {
                            isStanding = false;
                        }
                        wrapper.Update(vehicleUpdate);
                    }
                    else if (EnemyVehicles.TryGetValue(vehickeId, out wrapper))
                    {
                        wrapper.Update(vehicleUpdate);
                    }
                    AllVehicles[vehickeId].Update(vehicleUpdate);
                }
                else
                {
                    AllVehicles.Remove(vehickeId);
                    MyVehicles.Remove(vehickeId);
                    EnemyVehicles.Remove(vehickeId);
                }
            }
            if (_kamikadzeId.HasValue && !MyVehicles.ContainsKey(_kamikadzeId.Value))
            {
                _kamikadzeId = null;
            }
            if (isStanding)
            {
                _standingCount++;
            }
            if (MyVehicles.Any())
            {
                _myMinX = MyVehicles.Values.Where(v => !_kamikadzeIdHistory.Contains(v.Id)).Min(v => v.X);
                _myMaxX = MyVehicles.Values.Where(v => !_kamikadzeIdHistory.Contains(v.Id)).Max(v => v.X);
                _myAvgX = MyVehicles.Values.Where(v => !_kamikadzeIdHistory.Contains(v.Id)).Average(v => v.X);
                _myMinY = MyVehicles.Values.Where(v => !_kamikadzeIdHistory.Contains(v.Id)).Min(v => v.Y);
                _myMaxY = MyVehicles.Values.Where(v => !_kamikadzeIdHistory.Contains(v.Id)).Max(v => v.Y);
                _myAvgY = MyVehicles.Values.Where(v => !_kamikadzeIdHistory.Contains(v.Id)).Average(v => v.Y);
            }
            if (EnemyVehicles.Any())
            {
                _enemyAvgX = EnemyVehicles.Values.Average(v => v.X);
                _enemyAvgY = EnemyVehicles.Values.Average(v => v.Y);
            }
        }

        private void ProcessQueue()
        {
            if (ActionQueue.Any() && _me.RemainingActionCooldownTicks == 0)
            {
                var move = ActionQueue.Dequeue();
                _move.Action = move.Action;
                _move.X = move.X;
                _move.Y = move.Y;
                _move.Angle = move.Angle;
                _move.Group = move.Group;
                _move.Left = move.Left;
                _move.Top = move.Top;
                _move.Right = move.Right;
                _move.Bottom = move.Bottom;
                _move.VehicleType = move.VehicleType;
                _move.MaxSpeed = move.MaxSpeed;
                _move.MaxAngularSpeed = move.MaxAngularSpeed;
                _move.Factor = move.Factor;
                _move.FacilityId = move.FacilityId;
                _move.VehicleId = move.VehicleId;
            }
        }

        private void NuclearStrike()
        {
            if (_me.NextNuclearStrikeTickIndex == -1 && _world.TickIndex % 60 == 0 && ActionQueue.All(a => a.Action != ActionType.TacticalNuclearStrike))
            {
                Move possibleStrike = null;
                var affected = 0;
                var sqrNuclearRadius = _game.TacticalNuclearStrikeRadius * _game.TacticalNuclearStrikeRadius;
                foreach (var enemy in EnemyVehicles.Values)
                {
                    var visors = MyVehicles.Values
                                           .Where(i => SqrDistance(i, enemy) < 0.6 * i.VisionRange * i.VisionRange)
                                           .OrderByDescending(i => i.Durability)
                                           .ToList();

                    if (visors.Count == 0)
                    {
                        continue;
                    }

                    var affectedVehicles = AllVehicles.Values
                                                      .Where(i => SqrDistance(i, enemy) < sqrNuclearRadius)
                                                      .ToList();

                    var enemyAffectedCount = affectedVehicles.Count(i => i.PlayerId != _me.Id);
                    if (enemyAffectedCount <= Math.Min(30, EnemyVehicles.Count / 2 - 10))
                    {
                        continue;
                    }

                    var myAffectedCount = affectedVehicles.Count(i => i.PlayerId == _me.Id);

                    if (myAffectedCount < 0.8 * enemyAffectedCount)
                    {
                        if (affected < enemyAffectedCount - myAffectedCount)
                        {
                            affected = enemyAffectedCount - myAffectedCount;
                            possibleStrike = new Move
                            {
                                Action = ActionType.TacticalNuclearStrike,
                                X = enemy.X,
                                Y = enemy.Y,
                                VehicleId = visors[0].Id
                            };
                        }
                    }
                }
                if (possibleStrike != null)
                {
                    //     var dequeue = ActionQueue.Dequeue();
                    ActionQueue.Enqueue(possibleStrike);
                }
            }
        }


        private void NuclearStrike2()
        {
            if (_me.RemainingNuclearStrikeCooldownTicks == 0 &&
                _me.NextNuclearStrikeTickIndex == -1 &&
                _world.TickIndex % 59 == 0 &&
                ActionQueue.All(a => a.Action != ActionType.TacticalNuclearStrike))
            {
                VehicleWrapper possibleTarget = null;
                double bestDeltaDamage = Math.Min(30 * 100, EnemyVehicles.Values.Sum(i => i.Durability) / 5);

                var sqrNuclearRadius = _game.TacticalNuclearStrikeRadius * _game.TacticalNuclearStrikeRadius;

                const double visionRangeThreshold = 0.7;
                foreach (var enemy in EnemyVehicles.Values)
                {
                    var visors = MyVehicles.Values
                                           .Where(i => SqrDistance(i, enemy) < visionRangeThreshold * i.VisionRange * i.VisionRange)
                                           .ToList();

                    if (visors.Count == 0)
                    {
                        continue;
                    }


                    var allDamaged = AllVehicles.Values
                                                .Where(i => SqrDistance(i, enemy) < sqrNuclearRadius)
                                                .ToList();

                    var enemyDamaged = allDamaged.Where(i => i.PlayerId != _me.Id).ToList();
                    var myDamaged = allDamaged.Where(i => i.PlayerId == _me.Id).ToList();

                    if (enemyDamaged.Count <= Math.Min(30, EnemyVehicles.Count / 2 - 10))
                    {
                        continue;
                    }

                    var myDamage = myDamaged
                        .Sum(i => Math.Min(i.Durability, _game.MaxTacticalNuclearStrikeDamage * (1 - Math.Sqrt(SqrDistance(i, enemy) / sqrNuclearRadius))));
                    var enemyDamage = enemyDamaged
                        .Sum(i => Math.Min(i.Durability, _game.MaxTacticalNuclearStrikeDamage * (1 - Math.Sqrt(SqrDistance(i, enemy) / sqrNuclearRadius))));

                    if (enemyDamage - myDamage > bestDeltaDamage)
                    {
                        bestDeltaDamage = enemyDamage - myDamage;
                        possibleTarget = enemy;
                    }
                }

                if (possibleTarget == null)
                {
                    return;
                }
                var visor = MyVehicles.Values
                                      .Where(i => SqrDistance(i, possibleTarget) < visionRangeThreshold * i.VisionRange * i.VisionRange)
                                      .OrderBy(i => SqrDistance(i, possibleTarget)).First();
                ActionQueue.Enqueue(new Move
                {
                    Action = ActionType.TacticalNuclearStrike,
                    X = possibleTarget.X + 30 * possibleTarget.SpeedX,
                    Y = possibleTarget.Y + 30 * possibleTarget.SpeedY,
                    VehicleId = visor.Id
                });
            }
        }

        private double SqrDistance(VehicleWrapper from, VehicleWrapper to)
        {
            var x = to.X - from.X;
            var y = to.Y - from.Y;

            return (x * x + y * y);
        }


        public enum AggregateOperation
        {
            Min,
            Max,
            Average
        }

        public class VehicleWrapper
        {
            private const int HistoryLength = 10;
            private readonly double[] historyX = new double[HistoryLength];
            private readonly double[] historyY = new double[HistoryLength];
            public long Id { get; set; }
            public VehicleType Type { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public int Durability { get; set; }
            public long PlayerId { get; set; }

            public double SpeedX => (historyX[0] - historyX[HistoryLength - 1]) / HistoryLength;
            public double SpeedY => (historyY[0] - historyY[HistoryLength - 1]) / HistoryLength;


            public double VisionRange { get; set; }

            public static implicit operator VehicleWrapper(Vehicle vehicle)
            {
                return new VehicleWrapper
                {
                    Id = vehicle.Id,
                    Type = vehicle.Type,
                    X = vehicle.X,
                    Y = vehicle.Y,
                    Durability = vehicle.Durability,
                    PlayerId = vehicle.PlayerId,
                    VisionRange = vehicle.VisionRange
                };
            }


            public void Update(VehicleUpdate vehicleUpdate)
            {
                X = vehicleUpdate.X;
                Y = vehicleUpdate.Y;
                Durability = vehicleUpdate.Durability;

                ShiftHistory(historyX, X);
                ShiftHistory(historyY, Y);
            }

            private void ShiftHistory(double[] history, double newVal)
            {
                for (var i = history.Length - 1; i > 0; i--)
                {
                    history[i] = history[i - 1];
                }
                history[0] = newVal;
            }

            public override string ToString()
            {
                return $"({X};{Y}){Type}";
            }
        }
    }
}