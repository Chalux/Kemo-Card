using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Godot;
using Godot.Collections;

namespace KemoCard.Scripts.Map
{
    public class MapData
    {
        /// <summary>
        /// x轴两个点的间距
        /// </summary>
        public int XDistance { get; } = 180;

        /// <summary>
        /// y轴两个点的间距
        /// </summary>
        public int YDistance { get; } = 150;

        /// <summary>
        /// 每个点随机偏移多少像素
        /// </summary>
        public int PlacementRandomness { get; set; } = 25;

        /// <summary>
        /// 一共多少层
        /// </summary>
        public uint Floors { get; set; } = 15;

        /// <summary>
        /// 一层有多少格
        /// </summary>
        public uint MapWidth { get; set; } = 7;

        /// <summary>
        /// 代码会生成多少条路径
        /// </summary>
        public uint Paths { get; set; } = 6;

        /// <summary>
        /// 生成怪物房间的权重
        /// </summary>
        public double MonsterRoomWeight { get; set; } = 10.0;

        /// <summary>
        /// 生成商店房间的权重
        /// </summary>
        public double ShopRoomWeight { get; set; } = 2.5;

        /// <summary>
        /// 生成事件房间的权重
        /// </summary>
        public double EventRoomWeight { get; set; } = 4.0;

        [JsonIgnore] public List<string> PresetPool { get; set; } = [];
        [JsonIgnore] public List<string> EventPool { get; set; } = [];
        [JsonIgnore] public List<string> EquipPool { get; set; } = [];
        [JsonIgnore] public List<string> CardPool { get; set; } = [];
        public uint MinTier { get; set; } = 1;
        public uint MaxTier { get; set; } = 1;
        public string ShowName { get; set; } = "";
        public string Id { get; set; } = "";
        public uint HealTimes { get; private set; } = 3;
        public Godot.Collections.Dictionary<string, Array<Variant>> Cond;
        public bool CanAbort { get; set; } = true;

        /// <summary>
        /// 在地图生成后，会将生成完的地图传给这个委托。作者可以用自己的规则来给地图重新规定房间内容。
        /// </summary>
        [JsonIgnore]
        public Action<List<List<Room>>> Rules = null;

        public MapData(string id)
        {
            if (!Datas.Ins.MapPool.TryGetValue(id, out var pool)) return;
            Id = pool.MapId;
            MinTier = pool.MinTier;
            MaxTier = pool.MaxTier;
            Cond = pool.ShowCond;
            Floors = pool.Floor;
            MapWidth = pool.MapWidth;
            Paths = pool.Paths;
            HealTimes = pool.HealTimes;
            CanAbort = pool.CanAbort;
            ReloadPools();
            var mapScript = MapFactory.CreateMap(id);
            mapScript.Init(this);
        }

        public MapData()
        {
        }

        [JsonIgnore] public Action MapEndAction;
        [JsonIgnore] public Action MapStartAction;

        public void ReloadPools()
        {
            if (PresetPool is { Count: 0 })
            {
                foreach (var p in Datas.Ins.PresetPool.Values.Where(p =>
                             p.Tier >= MinTier && p.Tier <= MaxTier && p is { IsBoss: false, IsSpecial: false }))
                {
                    PresetPool.Add(p.PresetId);
                }
            }

            if (PresetPool.Count == 0)
            {
                PresetPool.Add("bat");
            }

            PresetPool.Sort((a, b) => (int)(Datas.Ins.PresetPool[a].Tier - Datas.Ins.PresetPool[b].Tier));
            if (EventPool is { Count: 0 })
            {
                foreach (var e in Datas.Ins.EventPool.Values.Where(e => !e.IsSpecial))
                {
                    EventPool.Add(e.EventId);
                }
            }

            if (EventPool.Count == 0)
            {
                EventPool.Add("empty_event");
            }

            if (EquipPool is { Count: 0 })
            {
                foreach (var eq in Datas.Ins.EquipPool.Values.Where(eq => !eq.IsSpecial))
                {
                    EquipPool.Add(eq.EquipId);
                }
            }

            if (EquipPool.Count == 0)
            {
                EquipPool.Add("test_equip_1");
            }

            if (CardPool is not { Count: 0 }) return;
            foreach (var card in Datas.Ins.CardPool.Values.Where(card => !card.IsSpecial))
            {
                CardPool.Add(card.CardId);
            }
        }

        public void TryHeal()
        {
            if (HealTimes > 0)
            {
                StaticInstance.EventMgr.Dispatch("BeforeHeal");
                StaticInstance.PlayerData.Gsd.MajorRole.CurrHealth +=
                    StaticInstance.PlayerData.Gsd.MajorRole.CurrHpLimit / 2;
                StaticInstance.MainRoot.ShowBanner("已治疗生命上限一半的血量");
                HealTimes -= 1;
                StaticInstance.EventMgr.Dispatch("AfterHeal");
            }
            else
            {
                StaticInstance.MainRoot.ShowBanner("休息次数不足");
            }
        }
    }
}