using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using static StaticClass.StaticEnums;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class B10004 : BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase b)
        {
            b.BuffId = 10004;
            b.BuffName = "fire_injury";
            b.BuffShowname = "火属性易伤";
            b.IsInfinite = true;
            b.Desc = "受到的火属性伤害提升至1.5倍";
            b.IconPath = "res://Mods/MainPackage/Resources/Icons/Skillicon14_14.png";
            b.AddEvent("BeforeDealDamageSingle", new(datas =>
            {
                if (datas != null)
                {
                    if (datas is Damage d)
                    {
                        if (d.targets[0] == b.Binder && d.atrribute == AttributeEnum.FIRE)
                        {
                            GD.Print("Buff 10004通过条件检测,修改前的值：" + d.value);
                            d.value *= 1.5;
                            GD.Print("Buff 10004通过条件检测,修改后的值：" + d.value);
                        }
                    }
                }
            }));
        }
    }
}
