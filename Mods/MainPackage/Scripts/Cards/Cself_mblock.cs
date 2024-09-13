using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cself_mblock : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.FunctionUse = new((user, targets, datas) =>
            {
                foreach (var target in targets)
                {
                    if (target != null && target is PlayerRole ifp)
                    {
                        ifp.CurrMBlock += 5;
                    }
                }
            });
        }
    }
}
