using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class B10001 : BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase b)
        {
            b.BuffId = 10001;
            b.BuffName = "water_injury";
            b.BuffShowname = "水属性易伤";
            b.IsInfinite = true;
            b.Desc = "受到的水属性伤害提高至1.5倍";
            b.IconPath = "res://Mods/MainPackage/Resources/Icons/Skillicon14_04.png";
            b.AddEvent("BeforeDealDamageSingle", new((datas) =>
            {
                if (datas != null)
                {
                    if (datas is Damage d)
                    {
                        if (d.targets[0] == b.Binder && d.atrribute == StaticClass.StaticEnums.AttributeEnum.WATER)
                        {
                            GD.Print("Buff 10001通过条件检测,修改前的值：" + d.value);
                            d.value *= 1.5;
                            GD.Print("Buff 10001通过条件检测,修改后的值：" + d.value);
                        }
                    }
                }
            }));
        }
    }
}
