using Godot;
using System;
using System.Collections.Generic;

namespace KemoCard.Scripts.Map
{
    public class MapData
    {
        /// <summary>
        /// x轴两个点的间距
        /// </summary>
        public int X_DISTANCE { get; set; } = 180;
        /// <summary>
        /// y轴两个点的间距
        /// </summary>
        public int Y_DISTANCE { get; set; } = 150;
        /// <summary>
        /// 每个点随机偏移多少像素
        /// </summary>
        public int PLACEMENT_RANDOMNESS { get; set; } = 25;
        /// <summary>
        /// 一共多少层
        /// </summary>
        public uint FLOORS { get; set; } = 15;
        /// <summary>
        /// 一层有多少格
        /// </summary>
        public uint MAP_WIDTH { get; set; } = 7;
        /// <summary>
        /// 代码会生成多少条路径
        /// </summary>
        public uint PATHS { get; set; } = 6;
        /// <summary>
        /// 生成怪物房间的权重
        /// </summary>
        public double MONSTER_ROOM_WEIGHT { get; set; } = 10.0;
        /// <summary>
        /// 生成商店房间的权重
        /// </summary>
        public double SHOP_ROOM_WEIGHT { get; set; } = 2.5;
        /// <summary>
        /// 生成事件房间的权重
        /// </summary>
        public double EVENT_ROOM_WEIGHT { get; set; } = 4.0;
        public List<uint> PresetPool { get; set; } = new();
        public uint MinTier { get; set; } = 1;
        public uint MaxTier { get; set; } = 1;
        public string ShowName { get; set; } = "";
        public Godot.Collections.Dictionary<string, Godot.Collections.Array<Variant>> Cond;
        /// <summary>
        /// 在地图生成后，会将生成完的地图传给这个委托。作者可以用自己的规则来给地图重新规定房间内容。
        /// </summary>
        public Action<List<List<Room>>> Rules;

        public MapData(uint id)
        {
            if (Datas.Ins.MapPool.TryGetValue(id, out var pool))
            {
                var path = $"res://Mods/{pool.mod_id}/Scripts/Maps/M{pool.map_id}.cs";
                using var res = ResourceLoader.Load<CSharpScript>(path);
                if (res != null)
                {
                    var MapScript = res.New().As<BaseMapScript>();
                    MinTier = pool.min_tier;
                    MaxTier = pool.max_tier;
                    Cond = pool.show_cond;
                    FLOORS = pool.floor;
                    MAP_WIDTH = pool.map_width;
                    PATHS = pool.paths;
                    MapScript.Init(this);
                }
                if (PresetPool != null && PresetPool.Count == 0)
                {
                    foreach (var p in Datas.Ins.PresetPool.Values)
                    {
                        if (p.tier >= MinTier && p.tier <= MaxTier && p.is_boss == false)
                        {
                            PresetPool.Add(p.preset_id);
                        }
                    }
                }
                if (PresetPool.Count == 0)
                {
                    PresetPool.Add(1);
                }
                PresetPool.Sort((a, b) => (int)(Datas.Ins.PresetPool[a].tier - Datas.Ins.PresetPool[b].tier));
            }
        }

        public MapData() { }
    }
}
