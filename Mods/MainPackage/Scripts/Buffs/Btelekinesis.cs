using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class Btelekinesis : BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase buff)
        {
            buff.BuffShowname = "念力护罩";
            buff.BuffCount = 1;
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
