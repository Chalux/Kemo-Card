using KemoCard.Scripts.Cards;
using StaticClass;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Ctelekinesis : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.UseFilter = new((user, targets, datas) =>
            {
                StaticUtils.CreateBuffAndAddToRole("telekinesis", c.owner, c.owner);
                return true;
            });
        }
    }
}
