using System.Collections.Generic;
using System.Linq;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

namespace KemoCard.Pages;

public partial class RoleDeckView : BaseScene
{
    [Export] FlowContainer FlowContainer;
    [Export] Godot.Button ExitBtn;
    [Export] TabBar Tab;
    private PlayerRole _role;

    public override void OnAdd(params object[] datas)
    {
        ExitBtn.Pressed += () => { StaticInstance.WindowMgr.RemoveScene(this); };
        if (datas[0] is PlayerRole pr)
        {
            _role = pr;
        }

        Tab.TabChanged += UpdateView;
        Tab.TabHovered += ShowHint;
        Tab.MouseExited += StaticInstance.MainRoot.HideRichHint;
        UpdateView(0);
    }

    private static void ShowHint(long tab)
    {
        switch (tab)
        {
            case 0:
                StaticInstance.MainRoot.ShowRichHint("全部卡牌");
                break;
            case 1:
                StaticInstance.MainRoot.ShowRichHint("角色卡牌，角色自身附带的卡牌，一般无法变更");
                break;
            case 2:
                StaticInstance.MainRoot.ShowRichHint("装备卡牌，角色装备的卡牌，可以通过穿脱装备来变更");
                break;
            case 3:
                StaticInstance.MainRoot.ShowRichHint("临时卡牌，临时获得的卡牌，一般在一张地图内有效，地图通关后将清除。");
                break;
        }
    }

    private void UpdateView(long tab)
    {
        if (_role == null) return;
        List<Card> cards = [];
        switch (tab)
        {
            case 0:
                cards = _role.Deck.Concat(_role.TempDeck).ToList();
                break;
            case 1:
                cards = _role.Deck.Where(c => c.EquipId == "").ToList();
                break;
            case 2:
                cards = _role.Deck.Where(c => c.EquipId != "").ToList();
                break;
            case 3:
                cards = _role.TempDeck;
                break;
        }

        if (cards.Count > FlowContainer.GetChildCount())
        {
            for (var i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                if (i < FlowContainer.GetChildCount())
                {
                    var cardobject = (CardShowObject)FlowContainer.GetChild(i);
                    cardobject.InitDataByCard(card);
                }
                else
                {
                    var cardobject = ResourceLoader.Load<PackedScene>("res://Pages/CardShowObject.tscn")
                        .Instantiate<CardShowObject>();
                    cardobject.InitDataByCard(card);
                    FlowContainer.AddChild(cardobject);
                }
            }
        }
        else
        {
            for (var i = 0; i < FlowContainer.GetChildCount(); i++)
            {
                var cardobject = (CardShowObject)FlowContainer.GetChild(i);
                if (i < cards.Count)
                {
                    cardobject.InitDataByCard(cards[i]);
                }
                else
                {
                    cardobject.QueueFree();
                }
            }
        }
    }
}