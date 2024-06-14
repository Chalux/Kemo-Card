using System.Collections.Generic;
using static StaticClass.StaticEnums;

namespace KemoCard.Scripts
{
    public class Damage
    {
        public double value;
        public BaseRole from;
        public List<BaseRole> targets;
        public AttributeEnum atrribute;
        public AttackType attackType;
        public bool validTag;
    }
}
