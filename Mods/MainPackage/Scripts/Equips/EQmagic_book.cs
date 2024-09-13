using KemoCard.Scripts;
using KemoCard.Scripts.Equips;

namespace KemoCard.Mods.MainPackage.Scripts.Equips
{
    internal partial class EQmagic_book : BaseEquipScript
    {
        public override void OnEquipInit(EquipImplBase eq)
        {
            eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_225.png";
            eq.Desc = "增加3点效率。添加2张暗影魔弹和1张法力狂潮到卡组中。";
            eq.Name = "魔导书";
            eq.CardDic.Add("shadow_shot", 2);
            eq.CardDic.Add("mana_tide", 1);
            eq.Binder.Symbol.TryAdd("EffeciencyAddProp", 3);
        }
    }
}
