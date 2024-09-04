using KemoCard.Scripts;
using KemoCard.Scripts.Equips;

namespace KemoCard.Mods.MainPackage.Scripts.Equips
{
    internal partial class EQbase_attack : BaseEquipScript
    {
        public override void OnEquipInit(EquipImplBase eq)
        {
            eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_078.png";
            eq.CardDic.Add("punch", 1);
            eq.CardDic.Add("magic_missile", 1);
            eq.Desc = "基础攻击。\n为卡组添加一张无属性物攻(消耗1行动力，对敌方单体造成5点无属性物理伤害.)和一张无属性魔攻(消耗1行动力，对敌方单体造成5点无属性魔法伤害.)";
            eq.Name = "基础攻击";
        }
    }
}
