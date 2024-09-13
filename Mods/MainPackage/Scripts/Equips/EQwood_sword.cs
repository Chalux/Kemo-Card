using KemoCard.Scripts;
using KemoCard.Scripts.Equips;

namespace KemoCard.Mods.MainPackage.Scripts.Equips
{
    internal partial class EQwood_sword : BaseEquipScript
    {
        public override void OnEquipInit(EquipImplBase eq)
        {
            eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_078.png";
            eq.Desc = "增加5点力量";
            eq.Binder.Symbol.TryAdd("StrengthAddProp", 5);
            eq.Name = "木剑";
        }
    }
}
