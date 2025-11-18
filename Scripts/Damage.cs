using System.Collections.Generic;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts
{
    public class Damage
    {
        public double Value;
        public BaseRole From;
        public List<BaseRole> Targets;
        public AttributeEnum Attribute;
        public AttackType AttackType;
        public bool ValidTag;
        public int Times;
        public int CriticalTimes;
    }
}
