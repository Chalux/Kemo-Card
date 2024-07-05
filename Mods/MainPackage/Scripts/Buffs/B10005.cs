using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using static StaticClass.StaticEnums;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class B10005 : BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase b)
        {
            b.BuffId = 10005;
            b.BuffName = "water_resis";
            b.BuffShowname = "水属性抵抗";
            b.IsInfinite = true;
            b.Desc = "受到的水属性伤害降低至0.75倍";
            b.IconPath = "res://Mods/MainPackage/Resources/Icons/Skillicon14_14.png";
            b.AddEvent("BeforeDealDamageSingle", new(datas =>
            {
                if (datas != null)
                {
                    if (datas is Damage d)
                    {
                        if (d.targets[0] == b.Binder && d.atrribute == AttributeEnum.WATER)
                        {
                            GD.Print("Buff 10005通过条件检测,修改前的值：" + d.value);
                            d.value *= 0.75;
                            GD.Print("Buff 10005通过条件检测,修改后的值：" + d.value);
                        }
                    }
                }
            }));
        }
    }
}
