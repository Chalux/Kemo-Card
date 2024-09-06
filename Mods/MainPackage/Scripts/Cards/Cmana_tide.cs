using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cmana_tide : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.FunctionUse += ManaTide;
        }

        private void ManaTide(BaseRole user, List<BaseRole> targets, dynamic[] datas)
        {
            int recover_mana = user.CurrMantra * 3 + (int)user.MagicAbility;
            int overflow = recover_mana - user.CurrMpLimit + user.CurrMagic;
            user.CurrMagic += recover_mana;
            if (overflow > 0)
            {
                (user as PlayerRole).CurrMBlock += (int)Math.Floor(overflow / 3f);
            }
        }
    }
}
