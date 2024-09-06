using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Ccreate_book_armor : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.FunctionUse = UseFunction;
        }

        private void UseFunction(BaseRole user, List<BaseRole> targets, params object[] datas)
        {
            var bs = StaticUtils.TryGetBattleScene();
            if (bs != null)
            {
                (user as PlayerRole).CurrMBlock += 2 + user.CurrCraftBook;
            }
        }
    }
}
