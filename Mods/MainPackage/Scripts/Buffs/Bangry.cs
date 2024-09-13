using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class Bangry : BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase b)
        {
            b.AddEvent("NewTurn", new(datas =>
            {
                if (datas != null)
                {
                    if (datas[0] > 1)
                    {
                        if (b.Binder is BaseRole Binder)
                        {
                            Binder.AddSymbolValue("StrengthAddProp", 3);
                        }
                    }
                }
            }));
        }
    }
}
