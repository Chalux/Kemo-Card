using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class Bwater_injury : BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase b)
        {
            b.AddEvent("BeforeDealDamageSingle", new((datas) =>
            {
                if (datas != null)
                {
                    if (datas is Damage d)
                    {
                        if (d.targets != null && d.targets[0] == b.Binder && d.atrribute == StaticClass.StaticEnums.AttributeEnum.WATER)
                        {
                            GD.Print("Buff water_injury通过条件检测,修改前的值：" + d.value);
                            d.value *= 1.5;
                            GD.Print("Buff water_injury通过条件检测,修改后的值：" + d.value);
                        }
                    }
                }
            }));
        }
    }
}
