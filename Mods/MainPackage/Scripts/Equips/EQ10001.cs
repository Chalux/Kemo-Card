using KemoCard.Scripts;
using KemoCard.Scripts.Equips;

namespace KemoCard.Mods.MainPackage.Scripts.Equips
{
    internal partial class EQ10001 : BaseEquipScript
    {
        public override void OnEquipInit(EquipImplBase eq)
        {
            eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_094.png";
        }
    }
}
