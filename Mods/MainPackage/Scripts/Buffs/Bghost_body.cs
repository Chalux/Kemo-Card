using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class Bghost_body : BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase buff)
        {
            buff.AddEvent("BeforeDealDamageSingle", new((datas) =>
            {
                if (datas[0] is Damage damage)
                {
                    if (damage.attackType == StaticClass.StaticEnums.AttackType.Physics)
                    {
                        damage.value /= 2;
                    }
                }
            }));
        }
    }
}
