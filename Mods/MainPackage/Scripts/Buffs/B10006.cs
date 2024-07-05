using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class B10006:BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase b)
        {
            b.BuffId = 10006;
            b.BuffName = "angry";
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
                        if(b.Binder is BaseRole Binder)
                        {
                            Binder.AddSymbolValue("StrengthAddProp", 3);
                        }
                    }
                }
            }));
        }
    }
}
