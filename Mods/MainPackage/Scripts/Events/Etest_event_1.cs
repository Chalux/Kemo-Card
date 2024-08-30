using KemoCard.Scripts.Events;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Events
{
    internal partial class Etest_event_1 : BaseEventScript
    {
        public override void Init(Event e)
        {
            e.EventTitle = "在路上偶遇了一个牧师，他免费为你治疗了";
            e.AddEvent("太好了!(回复50%血量上限的血量)", "res://Mods/MainPackage/Resources/Icons/icons_048.png", () =>
            {
                StaticInstance.playerData.gsd.MajorRole.CurrHealth += (int)Math.Ceiling(StaticInstance.playerData.gsd.MajorRole.CurrHpLimit * 0.5);
                StaticUtils.CloseEvent();
            });
        }
    }
}
