using KemoCard.Scripts;
using KemoCard.Scripts.Equips;

namespace KemoCard.Mods.MainPackage.Scripts.Equips
{
    internal partial class EQbase_defense : BaseEquipScript
    {
        public override void OnEquipInit(EquipImplBase eq)
        {
            eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_094.png";
            eq.CardDic.Add("self_pblock", 1);
            eq.CardDic.Add("self_mblock", 1);
            eq.Desc = "基础防御。\n为卡组添加一张重整魔防(消耗6魔力，自身提高5点魔甲)和重整物防(消耗6魔力，自身提高5点物甲)。";
            eq.Name = "基础防御";
        }
    }
}
