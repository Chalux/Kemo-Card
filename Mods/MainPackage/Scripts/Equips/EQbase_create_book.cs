using KemoCard.Scripts;
using KemoCard.Scripts.Equips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Equips
{
    internal partial class EQbase_create_book : BaseEquipScript
    {
        public override void OnEquipInit(EquipImplBase eq)
        {
            eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_223.png";
            eq.Desc = "基础创术包。\n为卡组添加两张创术(消耗。从2张牌中选择1张牌临时加入你的牌组。敌方单体造成5 + (1*书写)点无属性魔法伤害/获得2 + (1*书写)点魔甲)";
            eq.CardDic.Add("create_book", 2);
            eq.Name = "基础创术包";
        }
    }
}
