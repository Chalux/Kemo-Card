using System;
using System.Collections.Generic;

namespace KemoCard.Scripts
{
    public static class StaticEnums
    {
        public const int BagCount = 30;

        public enum BuffDict
        {
            None,
        }

        public enum AttributeEnum
        {
            None,
            Water,
            Fire,
            Earth,
            Wind,
            Light,
            Dark,
        }

        public static readonly Dictionary<AttributeEnum, string> AttributeDic = new()
        {
            { AttributeEnum.None, "NONE" },
            { AttributeEnum.Water, "WATER" },
            { AttributeEnum.Fire, "FIRE" },
            { AttributeEnum.Earth, "EARTH" },
            { AttributeEnum.Wind, "WIND" },
            { AttributeEnum.Light, "LIGHT" },
            { AttributeEnum.Dark, "DARK" },
        };

        public enum TeamEnum
        {
            Player,
            Enemy,
        }

        public enum AttackResult
        {
            Success,
            Dodged,
            Invalidated,
            Failed,
        }

        public enum AttackType
        {
            Physics,
            Magic,
        }

        public enum CardStateEnum
        {
            Base,
            Clicked,
            AllOrSelfDragging,
            TargetDragging,
            Aiming,
            Released,
            Discarding
        }

        [Flags]
        public enum TargetType
        {
            EnemySingle = 0x1,
            EnemyAll = 0x2,
            TeamSingle = 0x4,
            TeamAll = 0x8,
            Self = 0x10,
            All = 0x20,
            AllSingle = 0x40,
        }

        public enum CostType
        {
            Magic,
            Health,
            ActionPoint,
        }

        public static readonly Dictionary<CostType, string> CostBgColor = new()
        {
            { CostType.ActionPoint, "#28ff005f" },
            { CostType.Health, "#f701015f" },
            { CostType.Magic, "#0073e0a9" },
        };

        public enum EquipType
        {
            None = 0,
            Helmet = 1,
            Armor = 2,
            Trousers = 3,
            Glove = 4,
            Shoe = 5,
            Weapon1 = 6,
            Weapon2 = 7,
            Other = 8,
            DoubleWeapon = 9,
            BothWeapon = 10,
        }

        [Flags]
        public enum EffectTriggerTiming
        {
            None = 0,
            OnEffectCreate = 0x1,
            OnEffectRemove = 0x2,
            OnBattleStart = 0x4,
            OnBattleEnd = 0x8,
            OnTurnStart = 0x10,
            OnTurnEnd = 0x20,
            OnPropertiesChanged = 0x40,
            OnBeAttacked = 0x80,
            OnBeTargeted = 0x100,
        }

        /// <summary>
        /// 提示词字典，可以在这里面存一些专有名词的说明什么的
        /// </summary>
        public static readonly Dictionary<string, HintStruct> HintDictionary = new()
        {
            { "KeepInHand", new HintStruct { Alias = "保留", Desc = "回合结束时不送去墓地保留在手上。" } },
            { "Exhaust", new HintStruct { Alias = "耗尽", Desc = "使用后不送去墓地直接移出游戏。" } },
        };

        public struct HintStruct
        {
            public string Alias;
            public string Desc;
        }

        public static readonly Dictionary<RoomType, string> RoomIconPath = new()
        {
            { RoomType.None, null },
            { RoomType.Monster, "res://Mods/MainPackage/Resources/Icons/icons_087.png" },
            { RoomType.Treasure, "res://Mods/MainPackage/Resources/Icons/icons_188.png" },
            { RoomType.Event, "res://Mods/MainPackage/Resources/Icons/icons_228.png" },
            { RoomType.Shop, "res://Mods/MainPackage/Resources/Icons/icons_206.png" },
            { RoomType.Boss, "res://Mods/MainPackage/Resources/Icons/icons_002.png" },
        };

        public static readonly Dictionary<int, string> CardFlagDic = new()
        {
            { 0x1, "攻击" },
            { 0x2, "护甲" },
            { 0x4, "治疗" },
            { 0x8, "无属性" },
            { 0x10, "水属性" },
            { 0x20, "火属性" },
            { 0x40, "地属性" },
            { 0x80, "风属性" },
            { 0x100, "光属性" },
            { 0x200, "暗属性" },
            { 0x400, "物理" },
            { 0x800, "魔法" },
            { 0x1000, "抽卡" },
            { 0x2000, "弃牌" },
            { 0x4000, "对护甲特效" },
            { 0x8000, "被弃牌特效" },
            { 0x10000, "创造" },
            { 0x20000, "回复法力" },
        };

        [Flags]
        public enum CardFlag : ulong
        {
            Attack = 0x1,
            Armor = 0x2,
            Heal = 0x4,
            NoneProperty = 0x8,
            WaterProperty = 0x10,
            FireProperty = 0x20,
            EarthProperty = 0x40,
            WindProperty = 0x80,
            LightProperty = 0x100,
            DarkProperty = 0x200,
            Physical = 0x400,
            Magic = 0x800,
            DrawCard = 0x1000,
            DiscardCard = 0x2000,
            ArmorEffect = 0x4000,
            DiscardEffect = 0x8000,
            Create = 0x10000,
            RecoverMana = 0x20000,
        }
    }
}