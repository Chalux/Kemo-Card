using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StaticClass.StaticEnums;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class B10002 : BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase b)
        {
            b.BuffName = "fire_resis";
            b.BuffShowname = "火属性抵抗";
            b.IsInfinite = true;
            b.Desc = "受到的火属性伤害降低至0.75倍";
            b.AddEvent("BeforeDealDamageSingle", new(datas =>
            {
                if (datas != null)
                {
                    if (datas is Damage d)
                    {
                        if (d.targets[0] == b.Binder && d.atrribute == AttributeEnum.FIRE)
                        {
                            GD.Print("Buff 10002通过条件检测,修改前的值：" + d.value);
                            d.value *= 0.75;
                            GD.Print("Buff 10002通过条件检测,修改后的值：" + d.value);
                        }
                    }
                }
            }));
        }
    }
}
