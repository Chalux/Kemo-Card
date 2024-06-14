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
        }

        public static Dictionary<AttributeEnum, string> AttributeDic = new()
        {
            { AttributeEnum.None, "NONE"},
            { AttributeEnum.WATER, "WATER" },
            { AttributeEnum.FIRE, "FIRE" },
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

        public enum State { BASE, CLICKED, ALL_OR_SELF_DRAGGING, TARGET_DRAGGING, AIMING, RELEASED }

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

        public static Dictionary<CostType, string> CostBgColor = new()
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
        public static Dictionary<string, string> HintDictionary = new()
        {

        };
    }
}
