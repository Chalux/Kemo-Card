using KemoCard.Scripts;
using System;
using System.Collections.Generic;

namespace StaticClass
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
            WATER,
            FIRE,
            EARTH,
        }

        public readonly static Dictionary<AttributeEnum, string> AttributeDic = new()
        {
            { AttributeEnum.None, "NONE"},
            { AttributeEnum.WATER, "WATER" },
            { AttributeEnum.FIRE, "FIRE" },
            { AttributeEnum.EARTH, "EARTH" }
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

        public enum CardStateEnum { BASE, CLICKED, ALL_OR_SELF_DRAGGING, TARGET_DRAGGING, AIMING, RELEASED, DISCARDING }

        [System.Flags]
        public enum TargetType
        {
            ENEMY_SINGLE = 0x1,
            ENEMY_ALL = 0x2,
            TEAM_SINGLE = 0x4,
            TEAM_ALL = 0x8,
            SELF = 0x10,
            ALL = 0x20,
            ALL_SINGLE = 0x40,
        }

        public enum CostType
        {
            MAGIC,
            HEALTH,
            ACTIONPOINT,
        }

        public readonly static Dictionary<CostType, string> CostBgColor = new()
        {
            { CostType.ACTIONPOINT , "#28ff005f" },
            { CostType.HEALTH , "#f701015f" },
            { CostType.MAGIC , "#0073e0a9" },
        };

        public enum EquipType
        {
            NONE = 0,
            HALMET = 1,
            ARMOR = 2,
            TROUSERS = 3,
            GLOVE = 4,
            SHOE = 5,
            WEAPON1 = 6,
            WEAPON2 = 7,
            OTHER = 8,
            DOUBLE_WEAPON = 9,
        }

        public enum EffectTriggerTiming
        {
            NONE = 0,
            ON_EFFECT_CREATE = 0x1,
            ON_EFFECT_REMOVE = 0x2,
            ON_BATTLE_START = 0x4,
            ON_BATTLE_END = 0x8,
            ON_TURN_START = 0x10,
            ON_TURN_END = 0x20,
            ON_PROPERTIES_CHANGED = 0x40,
            ON_BEATTACKED = 0x80,
            ON_BETARGETTED = 0x100,
        }

        /// <summary>
        /// 提示词字典，可以在这里面存一些专有名词的说明什么的
        /// </summary>
        public readonly static Dictionary<string, HintStruct> HintDictionary = new()
        {
            { "KeepInHand", new(){ Alias = "保留", Desc = "回合结束时不送去墓地保留在手上。"} },
            { "Exhaust", new(){ Alias = "耗尽", Desc = "使用后不送去墓地直接移出游戏。"} },
        };

        public struct HintStruct
        {
            public string Alias;
            public string Desc;
        }

        public readonly static Dictionary<RoomType, string> RoomIconPath = new()
        {
            { RoomType.None, null },
            { RoomType.Monster, "res://Mods/MainPackage/Resources/Icons/icons_087.png" },
            { RoomType.Treasure, "res://Mods/MainPackage/Resources/Icons/icons_188.png" },
            { RoomType.Event, "res://Mods/MainPackage/Resources/Icons/icons_228.png" },
            { RoomType.Shop, "res://Mods/MainPackage/Resources/Icons/icons_206.png" },
            { RoomType.Boss, "res://Mods/MainPackage/Resources/Icons/icons_002.png" },
        };

        public readonly static Dictionary<int, string> CardFlagDic = new()
        {
            { 0x1 , "攻击" },
            { 0x2 , "护甲" },
            { 0x4 , "治疗" },
            { 0x8 , "无属性" },
            { 0x10 , "水属性" },
            { 0x20 , "火属性" },
            { 0x40 , "地属性" },
            { 0x80 , "风属性" },
            { 0x100 , "光属性" },
            { 0x200 , "暗属性" },
            { 0x400 , "物理" },
            { 0x800 , "魔法" },
            { 0x1000 , "抽卡" },
            { 0x2000 , "弃牌" },
            { 0x4000 , "对护甲特效" },
            { 0x8000 , "被弃牌特效" },
            { 0x10000 , "创造" },
        };

        [Flags]
        public enum CardFlag : ulong
        {
            ATTACK = 0x1,
            ARMOR = 0x2,
            HEAL = 0x4,
            NONE_PROPERTY = 0x8,
            WATER_PROPERTY = 0x10,
            FIRE_PROPERTY = 0x20,
            EARTH_PROPERTY = 0x40,
            WIND_PROPERTY = 0x80,
            LIGHT_PROPERTY = 0x100,
            DARK_PROPERTY = 0x200,
            PHYSICAL = 0x400,
            MAGIC = 0x800,
            DRAW_CARD = 0x1000,
            DISCARD_CARD = 0x2000,
            ARMOR_EFFECT = 0x4000,
            DISCARD_EFFECT = 0x8000,
            CREATE = 0x10000,
        }
    }
}
