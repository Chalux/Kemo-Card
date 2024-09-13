using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using static StaticClass.StaticEnums;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class Bwater_resis : BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase b)
        {
            b.AddEvent("BeforeDealDamageSingle", new(datas =>
            {
                if (datas != null)
                {
                    if (datas is Damage d)
                    {
                        if (d.targets != null && d.targets != null && d.targets[0] == b.Binder && d.atrribute == AttributeEnum.WATER)
                        {
                            GD.Print("Buff water_resis通过条件检测,修改前的值：" + d.value);
                            d.value *= 0.75;
                            GD.Print("Buff water_resis通过条件检测,修改后的值：" + d.value);
                        }
                    }
                }
            }));
        }
    }
}
