using KemoCard.Scripts.Map;
using StaticClass;
using System;
using System.Collections.Generic;

namespace KemoCard.Scripts
{
    public class MapGeneration
    {
        public MapData Data { get; set; } = new();
        public Dictionary<RoomType, double> RANDOM_ROOM_TYPE_WEIGHTS { get; set; } = new()
        {
            {RoomType.Monster,0.0 },
            {RoomType.Event,0.0 },
            {RoomType.Shop,0.0 },
        };

        public double RANDOM_ROOM_TYPE_TOTAL_WEIGHT { get; set; } = 0.0;
        public List<List<Room>> MapData { get; set; } = new();
        public int FloorsClimbed { get; set; } = 0;
        public Room LastRoom { get; set; }
        public bool IsStillRunning { get; set; } = false;

        public void GenerateMap(MapData data = null)
        {
            if (data != null)
            {
                Data = data;
            }
            //StaticInstance.MainRoot.MapView.CameraEdgeY = StaticInstance.playerData.gsd.MapGenerator.Data.Y_DISTANCE * (StaticInstance.playerData.gsd.MapGenerator.Data.FLOORS - 1);
            MapData = GenerateGrid();
            var StartPoints = GetRandomStartingPoints();
            //GD.Print(string.Join(",", StartPoints));
            StartPoints.ForEach(StartPoint =>
            {
                int CurrentJ = StartPoint;
                for (int i = 0; i < Data.FLOORS - 1; i++)
                {
                    CurrentJ = SetUpConnection(i, CurrentJ);
                }
            });
            //int j = 0;
            //foreach (var floor in MapData)
            //{
            //    GD.Print($"floor {j}");
            //    var UsedRooms = floor.FindAll(room => room.NextRooms.Count > 0);
            //    GD.Print(string.Join(",", UsedRooms));
            //    j += 1;
            //}
            SetUpBossRoom();
            SetUpRandomRoomWeight();
            SetUpRoomTypes();

            var major = StaticInstance.playerData.gsd.MajorRole;
            major.TempDeck = new();
            major.CurrHealth = major.CurrHpLimit;
            major.CurrMagic = major.CurrMpLimit;
            major.RunSymbol.Clear();
            major.FightSymbol.Clear();
            IsStillRunning = true;
            Data.Rules?.Invoke(MapData);
            MainScene MainScene = (MainScene)StaticInstance.windowMgr.GetSceneByName("MainScene");
            //MainScene?.MapView.GenerateNewMap(Data);
            MainScene?.UpdateView();
            MainScene?.MapView.UnlockFloor(0);

            //int i = 0;
            //MapData.ForEach(Rooms =>
            //{
            //    GD.Print($"floor {i}:\t[{string.Join(",", Rooms)}]");
            //    i += 1;
            //});
        }

        private List<List<Room>> GenerateGrid()
        {
            List<List<Room>> Result = new((int)Data.FLOORS);
            for (int i = 0; i < Data.FLOORS; i++)
            {
                List<Room> AdjacentRooms = new((int)Data.MAP_WIDTH);
                for (int j = 0; j < Data.MAP_WIDTH; j++)
                {
                    Room CurrentRoom = new();
                    Random r = new();
                    CurrentRoom.X = r.NextSingle() * Data.PLACEMENT_RANDOMNESS + Data.X_DISTANCE * j;
                    CurrentRoom.Y = r.NextSingle() * Data.PLACEMENT_RANDOMNESS - Data.Y_DISTANCE * i;
                    CurrentRoom.Row = i;
                    CurrentRoom.Col = j;
                    CurrentRoom.NextRooms = new();
                    if (i == Data.FLOORS - 1)
                    {
                        CurrentRoom.Y = (i + 1) * -Data.Y_DISTANCE;
                    }
                    AdjacentRooms.Add(CurrentRoom);
                }
                Result.Add(AdjacentRooms);
            }
            return Result;
        }

        private List<int> GetRandomStartingPoints()
        {
            List<int> YCoors = null;
            int UniquePoints = 0;
            while (UniquePoints < 2)
            {
                UniquePoints = 0;
                YCoors = new();

                for (int i = 0; i < Data.PATHS; i++)
                {
                    Random r = new();
                    int StartingPoint = r.Next((int)Data.MAP_WIDTH);
                    if (!YCoors.Contains(StartingPoint))
                    {
                        UniquePoints += 1;
                    }
                    YCoors.Add(StartingPoint);
                }
            }
            return YCoors;
        }

        private int SetUpConnection(int i, int j)
        {
            Room NextRoom = null;
            Room CurrentRoom = MapData[i][j];
            while (NextRoom == null || WouldCrossExistingPath(i, j, NextRoom))
            {
                Random r = new();
                int RandomJ = (int)Math.Clamp(r.Next(j - 1, j + 2), 0, Data.MAP_WIDTH - 1);
                NextRoom = MapData[i + 1][RandomJ];
            }
            CurrentRoom.NextRooms.Add(NextRoom.Row * 100 + NextRoom.Col);
            return NextRoom.Col;
        }

        private bool WouldCrossExistingPath(int i, int j, Room room)
        {
            Room LeftNeighbour = null, RightNeighbour = null;
            if (j > 0)
                LeftNeighbour = MapData[i][j - 1];
            if (j < Data.MAP_WIDTH - 1)
                RightNeighbour = MapData[i][j + 1];
            if (RightNeighbour != null && room.Col > j)
            {
                foreach (var NextRoom in RightNeighbour.NextRooms)
                {
                    if (NextRoom % 100 < room.Col) return true;
                }
            }
            if (LeftNeighbour != null && room.Col < j)
            {
                foreach (var NextRoom in LeftNeighbour.NextRooms)
                {
                    if (NextRoom % 100 > room.Col) return true;
                }
            }
            return false;
        }

        private void SetUpBossRoom()
        {
            int Middle = (int)Math.Floor(Data.MAP_WIDTH * 0.5);
            Room BossRoom = MapData[(int)Data.FLOORS - 1][Middle];
            for (int j = 0; j < Data.MAP_WIDTH; j++)
            {
                var CurrentRoom = MapData[(int)Data.FLOORS - 2][j];
                if (CurrentRoom.NextRooms != null && CurrentRoom.NextRooms.Count > 0)
                {
                    CurrentRoom.NextRooms = new()
                    {
                        BossRoom.Row * 100 + BossRoom.Col
                    };
                }
            }
            BossRoom.Type = RoomType.Boss;
            List<string> plist = new();
            foreach (var kvp in Datas.Ins.PresetPool)
            {
                if (kvp.Value.is_boss && kvp.Value.tier >= Data.MinTier && kvp.Value.tier <= Data.MaxTier) plist.Add(kvp.Key);
            }
            Random r = new();
            BossRoom.PresetId = plist[r.Next(plist.Count)];
        }

        private void SetUpRandomRoomWeight()
        {
            RANDOM_ROOM_TYPE_WEIGHTS[RoomType.Monster] = Data.MONSTER_ROOM_WEIGHT;
            RANDOM_ROOM_TYPE_WEIGHTS[RoomType.Event] = Data.MONSTER_ROOM_WEIGHT + Data.EVENT_ROOM_WEIGHT;
            RANDOM_ROOM_TYPE_WEIGHTS[RoomType.Shop] = Data.SHOP_ROOM_WEIGHT + RANDOM_ROOM_TYPE_WEIGHTS[RoomType.Event];

            RANDOM_ROOM_TYPE_TOTAL_WEIGHT = RANDOM_ROOM_TYPE_WEIGHTS[RoomType.Shop];
        }

        private void SetUpRoomTypes()
        {
            foreach (var Room in MapData[0])
            {
                if (Room.NextRooms != null && Room.NextRooms.Count > 0)
                {
                    Room.Type = RoomType.Monster;
                    var random = Math.Round(StaticUtils.GenerateRandomValue(0, Data.PresetPool.Count - 1, Data.PresetPool.Count / (Data.FLOORS - Room.Row), OffsetRange));
                    Room.PresetId = Data.PresetPool[(int)random];
                }
            }

            foreach (var Room in MapData[(int)Math.Floor(Data.FLOORS / 2d)])
            {
                if (Room.NextRooms != null && Room.NextRooms.Count > 0)
                {
                    Room.Type = RoomType.Treasure;
                }
            }

            foreach (var Room in MapData[(int)Data.FLOORS - 2])
            {
                if (Room.NextRooms != null && Room.NextRooms.Count > 0)
                {
                    Room.Type = RoomType.Treasure;
                }
            }

            foreach (var floor in MapData)
            {
                foreach (var Room in floor)
                {
                    foreach (var NextRoom in Room.NextRooms)
                    {
                        var r = StaticInstance.playerData.gsd.MapGenerator.MapData[NextRoom / 100][NextRoom % 100];
                        if (r.Type == RoomType.None || (r.PresetId == null && r.Type == RoomType.Monster))
                        {
                            SetRoomRandomly(r);
                        }
                    }
                }
            }
        }

        private static readonly int OffsetRange = 5;
        private void SetRoomRandomly(Room room)
        {
            bool ConsecutiveShop = true;
            bool NoShopUnderFloor4 = true;
            RoomType TypeCandidate = RoomType.None;
            while (ConsecutiveShop || NoShopUnderFloor4)
            {
                TypeCandidate = GetRandomRoomTypeByWeight();
                bool isShop = TypeCandidate == RoomType.Shop;
                bool hasShopParent = RoomHasParentOfType(room, RoomType.Shop);
                ConsecutiveShop = isShop && hasShopParent;
                NoShopUnderFloor4 = room.Row <= 4 && TypeCandidate == RoomType.Shop;
            }
            room.Type = TypeCandidate;
            if (room.Type == RoomType.Monster)
            {
                var random = Math.Round(StaticUtils.GenerateRandomValue(0, Data.PresetPool.Count - 1, Data.PresetPool.Count / (Data.FLOORS - room.Row), OffsetRange));
                room.PresetId = Data.PresetPool[(int)random];
            }
        }

        private bool RoomHasParentOfType(Room room, RoomType roomType)
        {
            List<Room> parents = new();
            var idx = room.Row * 100 + room.Col;
            // 左下父节点
            if (room.Col > 0 && room.Row > 0)
            {
                var parent_candidate = MapData[room.Row - 1][room.Col - 1];
                if (parent_candidate.NextRooms.Contains(idx))
                    parents.Add(parent_candidate);
            }
            // 下父节点
            if (room.Row > 0)
            {
                var parent_candidate = MapData[room.Row - 1][room.Col];
                if (parent_candidate.NextRooms.Contains(idx))
                    parents.Add(parent_candidate);
            }
            // 右下父节点
            if (room.Col < Data.MAP_WIDTH - 1 && room.Row > 0)
            {
                var parent_candidate = MapData[room.Row - 1][room.Col + 1];
                if (parent_candidate.NextRooms.Contains(idx))
                    parents.Add(parent_candidate);
            }
            foreach (var parent in parents)
            {
                if (parent.Type == roomType) return true;
            }
            return false;
        }

        private RoomType GetRandomRoomTypeByWeight()
        {
            Random r = new();
            var roll = r.NextDouble() * RANDOM_ROOM_TYPE_TOTAL_WEIGHT;
            foreach (var type in RANDOM_ROOM_TYPE_WEIGHTS.Keys)
            {
                if (RANDOM_ROOM_TYPE_WEIGHTS[type] > roll) return type;
            }
            return RoomType.Monster;
        }

        public override string ToString()
        {
            string res = "";
            int i = 0;
            MapData.ForEach(Rooms =>
            {
                res += $"floor {i}:\t[{string.Join(",", Rooms)}]\n";
                i += 1;
            });
            return res;
        }
    }
}
