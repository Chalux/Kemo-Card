using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class Bangry : BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase b)
        {
            b.BuffId = "angry";
            b.BuffShowname = "愤怒";
            b.IsInfinite = true;
            b.Desc = "回合开始时获得3点力量";
            b.IconPath = "res://Mods/MainPackage/Resources/Icons/icons_031.png";
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
