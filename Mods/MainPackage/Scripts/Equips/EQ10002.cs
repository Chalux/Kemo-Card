using KemoCard.Scripts;
using KemoCard.Scripts.Equips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Equips
{
    internal partial class EQ10002 : BaseEquipScript
    {
        public override void OnEquipInit(EquipImplBase eq)
        {
            eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_77.png";
            eq.CardDic.Add(10005, 1);
            eq.CardDic.Add(10006, 1);
        }
    }
}
