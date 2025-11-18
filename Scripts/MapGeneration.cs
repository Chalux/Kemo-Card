using System;
using System.Collections.Generic;
using System.Linq;
using KemoCard.Pages;
using KemoCard.Scripts.Map;

namespace KemoCard.Scripts
{
    public class MapGeneration
    {
        public MapData Data { get; private set; } = new();

        private Dictionary<RoomType, double> RandomRoomTypeWeights { get; } = new()
        {
            { RoomType.Monster, 0.0 },
            { RoomType.Event, 0.0 },
            { RoomType.Shop, 0.0 },
        };

        private double RandomRoomTypeTotalWeight { get; set; }
        public List<List<Room>> MapData { get; private set; } = [];
        public int FloorsClimbed { get; set; }
        public Room LastRoom { get; set; }
        public bool IsStillRunning { get; private set; }

        public void GenerateMap(MapData data = null, bool runStartAction = false)
        {
            if (data != null)
            {
                Data = data;
            }

            //StaticInstance.MainRoot.MapView.CameraEdgeY = StaticInstance.playerData.gsd.MapGenerator.Data.Y_DISTANCE * (StaticInstance.playerData.gsd.MapGenerator.Data.FLOORS - 1);
            MapData = GenerateGrid();
            var startPoints = GetRandomStartingPoints();
            //GD.Print(string.Join(",", StartPoints));
            startPoints.ForEach(startPoint =>
            {
                var currentJ = startPoint;
                for (var i = 0; i < Data.Floors - 1; i++)
                {
                    currentJ = SetUpConnection(i, currentJ);
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

            var major = StaticInstance.PlayerData.Gsd.MajorRole;
            major.TempDeck = [];
            major.CurrHealth = major.CurrHpLimit;
            major.CurrMagic = major.CurrMpLimit;
            major.RunSymbol.Clear();
            major.FightSymbol.Clear();
            IsStillRunning = true;
            Data.Rules?.Invoke(MapData);
            var mainScene = (MainScene)StaticInstance.WindowMgr.GetSceneByName("MainScene");
            //MainScene?.MapView.GenerateNewMap(Data);
            mainScene?.UpdateView();
            mainScene?.MapView.UnlockFloor(0);

            if (runStartAction)
            {
                Data.MapStartAction?.Invoke();
            }

            //int i = 0;
            //MapData.ForEach(Rooms =>
            //{
            //    GD.Print($"floor {i}:\t[{string.Join(",", Rooms)}]");
            //    i += 1;
            //});
        }

        private List<List<Room>> GenerateGrid()
        {
            List<List<Room>> result = new((int)Data.Floors);
            for (var i = 0; i < Data.Floors; i++)
            {
                List<Room> adjacentRooms = new((int)Data.MapWidth);
                for (var j = 0; j < Data.MapWidth; j++)
                {
                    Room currentRoom = new();
                    Random r = new();
                    currentRoom.X = r.NextSingle() * Data.PlacementRandomness + Data.XDistance * j;
                    currentRoom.Y = r.NextSingle() * Data.PlacementRandomness - Data.YDistance * i;
                    currentRoom.Row = i;
                    currentRoom.Col = j;
                    currentRoom.NextRooms = [];
                    if (i == Data.Floors - 1)
                    {
                        currentRoom.Y = (i + 1) * -Data.YDistance;
                    }

                    adjacentRooms.Add(currentRoom);
                }

                result.Add(adjacentRooms);
            }

            return result;
        }

        private List<int> GetRandomStartingPoints()
        {
            List<int> yCoors = null;
            var uniquePoints = 0;
            while (uniquePoints < 2)
            {
                uniquePoints = 0;
                yCoors = [];

                for (var i = 0; i < Data.Paths; i++)
                {
                    Random r = new();
                    var startingPoint = r.Next((int)Data.MapWidth);
                    if (!yCoors.Contains(startingPoint))
                    {
                        uniquePoints += 1;
                    }

                    yCoors.Add(startingPoint);
                }
            }

            return yCoors;
        }

        private int SetUpConnection(int i, int j)
        {
            Room nextRoom = null;
            var currentRoom = MapData[i][j];
            while (nextRoom == null || WouldCrossExistingPath(i, j, nextRoom))
            {
                Random r = new();
                var randomJ = (int)Math.Clamp(r.Next(j - 1, j + 2), 0, Data.MapWidth - 1);
                nextRoom = MapData[i + 1][randomJ];
            }

            currentRoom.NextRooms.Add(nextRoom.Row * 100 + nextRoom.Col);
            return nextRoom.Col;
        }

        private bool WouldCrossExistingPath(int i, int j, Room room)
        {
            Room leftNeighbour = null, rightNeighbour = null;
            if (j > 0)
                leftNeighbour = MapData[i][j - 1];
            if (j < Data.MapWidth - 1)
                rightNeighbour = MapData[i][j + 1];
            if (rightNeighbour != null && room.Col > j)
            {
                foreach (var nextRoom in rightNeighbour.NextRooms)
                {
                    if (nextRoom % 100 < room.Col) return true;
                }
            }

            if (leftNeighbour != null && room.Col < j)
            {
                foreach (var nextRoom in leftNeighbour.NextRooms)
                {
                    if (nextRoom % 100 > room.Col) return true;
                }
            }

            return false;
        }

        private void SetUpBossRoom()
        {
            var middle = (int)Math.Floor(Data.MapWidth * 0.5);
            var bossRoom = MapData[(int)Data.Floors - 1][middle];
            for (var j = 0; j < Data.MapWidth; j++)
            {
                var currentRoom = MapData[(int)Data.Floors - 2][j];
                if (currentRoom.NextRooms is { Count: > 0 })
                {
                    currentRoom.NextRooms = [bossRoom.Row * 100 + bossRoom.Col];
                }
            }

            bossRoom.Type = RoomType.Boss;
            //List<string> plist = new();
            //foreach (var kvp in Datas.Ins.PresetPool)
            //{
            //    if (kvp.Value.is_boss && kvp.Value.tier >= Data.MinTier && kvp.Value.tier <= Data.MaxTier) plist.Add(kvp.Key);
            //}
            //if (plist.Count == 0)
            //{
            //    StaticInstance.MainRoot.ShowBanner($"找不到任何处于此地图设定范围内的怪物配置，生成地图出错");
            //    return;
            //}
            //Random r = new();
            //BossRoom.RoomPresetId = plist[r.Next(plist.Count)];
        }

        private void SetUpRandomRoomWeight()
        {
            RandomRoomTypeWeights[RoomType.Monster] = Data.MonsterRoomWeight;
            RandomRoomTypeWeights[RoomType.Event] = Data.MonsterRoomWeight + Data.EventRoomWeight;
            RandomRoomTypeWeights[RoomType.Shop] = Data.ShopRoomWeight + RandomRoomTypeWeights[RoomType.Event];

            RandomRoomTypeTotalWeight = RandomRoomTypeWeights[RoomType.Shop];
        }

        private void SetUpRoomTypes()
        {
            foreach (var room in MapData[0].Where(room => room.NextRooms is { Count: > 0 }))
            {
                room.Type = RoomType.Monster;
                //var random = Math.Round(StaticUtils.GenerateRandomValue(0, Data.PresetPool.Count - 1, Data.PresetPool.Count / (Data.FLOORS - Room.Row), OffsetRange));
                //Room.RoomPresetId = Data.PresetPool[(int)random];
            }

            foreach (var room in MapData[(int)Math.Floor(Data.Floors / 2d)]
                         .Where(room => room.NextRooms is { Count: > 0 }))
            {
                room.Type = RoomType.Treasure;
                //SetTreasureRoom(Room);
            }

            foreach (Room room in MapData[(int)Data.Floors - 2].Where(room => room.NextRooms is { Count: > 0 }))
            {
                room.Type = RoomType.Treasure;
                //SetTreasureRoom(Room);
            }

            foreach (var r in from floor in MapData
                     from room in floor
                     from nextRoom in room.NextRooms
                     select StaticInstance.PlayerData.Gsd.MapGenerator.MapData[nextRoom / 100][nextRoom % 100]
                     into r
                     where r.Type == RoomType.None
                     select r)
            {
                SetRoomRandomly(r);
            }
        }

        private const int OffsetRange = 5;

        private void SetRoomRandomly(Room room)
        {
            var consecutiveShop = true;
            var noShopUnderFloor4 = true;
            var typeCandidate = RoomType.None;
            while (consecutiveShop || noShopUnderFloor4)
            {
                typeCandidate = GetRandomRoomTypeByWeight();
                var isShop = typeCandidate == RoomType.Shop;
                var hasShopParent = RoomHasParentOfType(room, RoomType.Shop);
                consecutiveShop = isShop && hasShopParent;
                noShopUnderFloor4 = room.Row <= 4 && typeCandidate == RoomType.Shop;
            }

            room.Type = typeCandidate;
            //if (room.Type == RoomType.Monster)
            //{
            //    var random = Math.Round(StaticUtils.GenerateRandomValue(0, Data.PresetPool.Count - 1, Data.PresetPool.Count / (Data.FLOORS - room.Row), OffsetRange));
            //    room.RoomPresetId = Data.PresetPool[(int)random];
            //}
            //else if (room.Type == RoomType.Event)
            //{
            //    Random r = new();
            //    var random = r.Next(0, Data.EventPool.Count);
            //    room.RoomEventId = Data.EventPool[random];
            //}
            //else if (room.Type == RoomType.Treasure)
            //{
            //    SetTreasureRoom(room);
            //}
        }

        private bool RoomHasParentOfType(Room room, RoomType roomType)
        {
            List<Room> parents = [];
            var idx = room.Row * 100 + room.Col;
            // 左下父节点
            if (room.Col > 0 && room.Row > 0)
            {
                var parentCandidate = MapData[room.Row - 1][room.Col - 1];
                if (parentCandidate.NextRooms.Contains(idx))
                    parents.Add(parentCandidate);
            }

            // 下父节点
            if (room.Row > 0)
            {
                var parentCandidate = MapData[room.Row - 1][room.Col];
                if (parentCandidate.NextRooms.Contains(idx))
                    parents.Add(parentCandidate);
            }

            // 右下父节点
            if (room.Col >= Data.MapWidth - 1 || room.Row <= 0) return parents.Any(parent => parent.Type == roomType);
            {
                var parentCandidate = MapData[room.Row - 1][room.Col + 1];
                if (parentCandidate.NextRooms.Contains(idx))
                    parents.Add(parentCandidate);
            }

            return parents.Any(parent => parent.Type == roomType);
        }

        private RoomType GetRandomRoomTypeByWeight()
        {
            Random r = new();
            var roll = r.NextDouble() * RandomRoomTypeTotalWeight;
            foreach (var type in RandomRoomTypeWeights.Keys.Where(type => RandomRoomTypeWeights[type] > roll))
            {
                return type;
            }

            return RoomType.Monster;
        }

        public override string ToString()
        {
            var res = "";
            var i = 0;
            MapData.ForEach(rooms =>
            {
                res += $"floor {i}:\t[{string.Join(",", rooms)}]\n";
                i += 1;
            });
            res += $"CardPool:{string.Join(",", Data.CardPool)}\n";
            res += $"EquipPool:{string.Join(",", Data.EquipPool)}\n";
            res += $"PresetPool:{string.Join(",", Data.PresetPool)}\n";
            res += $"EventPool:{string.Join(",", Data.EventPool)}\n";
            return res;
        }

        public void EndMap(bool isAbort = false)
        {
            if (Data == null) return;
            var oldMapId = Data.Id;
            if (!isAbort)
            {
                var key = $"Map{Data.Id}Passed";
                if (!StaticInstance.PlayerData.Gsd.IntData.TryAdd(key, 1))
                {
                    StaticInstance.PlayerData.Gsd.IntData[key] += 1;
                }

                Data.MapEndAction?.Invoke();
            }

            if (oldMapId == Data.Id)
            {
                Data = new MapData();
                IsStillRunning = false;
                FloorsClimbed = 0;
                LastRoom = null;
                MapData = [];
            }

            var ms = StaticInstance.WindowMgr.GetSceneByName("MainScene") as MainScene;
            ms?.MapView?.HideMap();
            StaticInstance.EventMgr.Dispatch("MapStateChange");
        }

        public static void SetTreasureRoom(Room room)
        {
            Random r = new();
            var random = r.Next(0, StaticInstance.PlayerData.Gsd.MapGenerator.Data.EquipPool.Count);
            room.RoomEquipId = StaticInstance.PlayerData.Gsd.MapGenerator.Data.EquipPool[random];
        }

        public static void SetEventRoom(Room room)
        {
            Random r = new();
            var random = r.Next(0, StaticInstance.PlayerData.Gsd.MapGenerator.Data.EventPool.Count);
            room.RoomEventId = StaticInstance.PlayerData.Gsd.MapGenerator.Data.EventPool[random];
        }

        public static void SetMonsterRoom(Room room, bool notBoss = false)
        {
            var data = StaticInstance.PlayerData.Gsd.MapGenerator.Data;
            if (notBoss)
            {
                var random = Math.Round(StaticUtils.GenerateRandomValue(0, data.PresetPool.Count - 1, (int)(data
                    .PresetPool.Count / (data.Floors - room.Row)), OffsetRange));
                room.RoomPresetId = data.PresetPool[(int)random];
            }
            else
            {
                List<string> plist = [];
                plist.AddRange(from kvp in Datas.Ins.PresetPool
                    where kvp.Value.IsBoss && kvp.Value.Tier >= data.MinTier && kvp.Value.Tier <= data.MaxTier
                    select kvp.Key);
                if (plist.Count == 0)
                {
                    StaticInstance.MainRoot.ShowBanner("找不到任何处于此地图设定范围内的怪物配置，生成地图出错");
                    return;
                }

                Random r = new();
                room.RoomPresetId = plist[r.Next(plist.Count)];
            }
        }
    }
}