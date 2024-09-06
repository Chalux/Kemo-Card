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
    internal partial class Ctelekinesis : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            StaticUtils.CreateBuffAndAddToRole("telekinesis", c.owner);
        }
    }
}
