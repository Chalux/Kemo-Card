using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public List<string> PresetPool { get; set; } = new();
        [JsonIgnore]
        public List<string> EventPool { get; set; } = new();
        [JsonIgnore]
        public List<string> EquipPool { get; set; } = new();
        [JsonIgnore]
        public List<string> CardPool { get; set; } = new();
        public uint MinTier { get; set; } = 1;
        public uint MaxTier { get; set; } = 1;
        public string ShowName { get; set; } = "";
        public Godot.Collections.Dictionary<string, Godot.Collections.Array<Variant>> Cond;
        /// <summary>
        /// 在地图生成后，会将生成完的地图传给这个委托。作者可以用自己的规则来给地图重新规定房间内容。
        /// </summary>
        public Action<List<List<Room>>> Rules;

        public MapData(string id)
        {
            if (Datas.Ins.MapPool.TryGetValue(id, out var pool))
            {
                ReloadPools();
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
            }
        }

        public MapData() { }

        public Action MapEndAction;

        public void ReloadPools()
        {
            if (PresetPool != null && PresetPool.Count == 0)
            {
                foreach (var p in Datas.Ins.PresetPool.Values)
                {
                    if (p.tier >= MinTier && p.tier <= MaxTier && p.is_boss == false && p.is_special != false)
                    {
                        PresetPool.Add(p.preset_id);
                    }
                }
            }
            if (PresetPool.Count == 0)
            {
                PresetPool.Add("test_preset_1");
            }
            PresetPool.Sort((a, b) => (int)(Datas.Ins.PresetPool[a].tier - Datas.Ins.PresetPool[b].tier));
            if (EventPool != null && EventPool.Count == 0)
            {
                foreach (var e in Datas.Ins.EventPool.Values)
                {
                    if (e.is_special != false)
                    {
                        EventPool.Add(e.event_id);
                    }
                }
            }
            if (EventPool.Count == 0)
            {
                EventPool.Add("test_event_1");
            }
            if (EquipPool != null && EquipPool.Count == 0)
            {
                foreach (var eq in Datas.Ins.EquipPool.Values)
                {
                    if (eq.is_special != false)
                    {
                        EquipPool.Add(eq.equip_id);
                    }
                }
            }
            if (EquipPool.Count == 0)
            {
                EquipPool.Add("test_equip_1");
            }
            if (CardPool != null && CardPool.Count == 0)
            {
                foreach (var card in Datas.Ins.CardPool.Values)
                {
                    if (card.is_special != false)
                    {
                        CardPool.Add(card.card_id);
                    }
                }
            }
        }
    }
}
