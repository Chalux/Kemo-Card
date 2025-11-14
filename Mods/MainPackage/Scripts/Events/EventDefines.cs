using System;
using KemoCard.Scripts;
using KemoCard.Scripts.Events;

namespace KemoCard.Mods.MainPackage.Scripts.Events;

public partial class EmptyEvent : BaseEventScript
{
    public override void Init(EventScript e)
    {
        e.EventTitle = "似乎什么都没遇到……真的吗？";
        e.AddEvent("继续前进", "res://Mods/MainPackage/Resources/Icons/icons_064.png", null);
    }
}

public partial class HealEvent : BaseEventScript
{
    public override void Init(EventScript e)
    {
        e.EventTitle = "在路上偶遇了一个牧师，他免费为你治疗了";
        e.AddEvent("太好了!(回复50%血量上限的血量)", "res://Mods/MainPackage/Resources/Icons/icons_048.png", () =>
        {
            StaticInstance.PlayerData.Gsd.MajorRole.CurrHealth +=
                (int)Math.Ceiling(StaticInstance.PlayerData.Gsd.MajorRole.CurrHpLimit * 0.5);
            StaticUtils.CloseEvent();
        });
    }
}