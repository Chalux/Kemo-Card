using KemoCard.Scripts;
using KemoCard.Scripts.Equips;

namespace KemoCard.Mods.MainPackage.Scripts.Equips
{
    internal partial class EQbase_create_equip : BaseEquipScript
    {
        public override void OnEquipInit(EquipImplBase eq)
        {
            eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_222.png";
            eq.Desc = "基础创物包。\n为卡组添加两张创物(消耗。从2张牌中选择1张牌临时加入你的牌组。敌方单体造成5 + (1*工艺)点无属性物理伤害/获得2 + (1*工艺)点护甲)";
            eq.CardDic.Add("create_equip", 2);
            eq.Name = "基础创物包";
        }
    }
}
