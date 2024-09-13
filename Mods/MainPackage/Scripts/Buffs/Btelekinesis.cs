using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using System;
using System.Linq;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class Btelekinesis : BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase buff)
        {
            buff.AddEvent("AfterAttacked", (data) =>
            {
                if (data[0] != null)
                {
                    if (data[0] is Damage damage)
                    {
                        if (damage.targets.Contains(buff.Binder))
                        {
                            if (buff.Binder is PlayerRole pr)
                            {
                                pr.CurrMBlock += (int)Math.Ceiling(damage.value / 2);
                            }
                        }
                    }
                }
            });
        }
    }
}
