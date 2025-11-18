using System.Collections.Generic;
using System.Linq;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

namespace KemoCard.Pages;

public partial class RoleDeckView : BaseScene
{
    [Export] private FlowContainer _flowContainer;
    [Export] private Button _exitBtn;
    [Export] private TabBar _tab;
    private PlayerRole _role;

    public override void OnAdd(params object[] datas)
    {
        _exitBtn.Pressed += () => { StaticInstance.WindowMgr.RemoveScene(this); };
        if (datas[0] is PlayerRole pr)
        {
            _role = pr;
        }

        _tab.TabChanged += UpdateView;
        _tab.TabHovered += ShowHint;
        _tab.MouseExited += StaticInstance.MainRoot.HideRichHint;
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

        if (cards.Count > _flowContainer.GetChildCount())
        {
            for (var i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                if (i < _flowContainer.GetChildCount())
                {
                    var cardShowObject = (CardShowObject)_flowContainer.GetChild(i);
                    cardShowObject.InitDataByCard(card);
                }
                else
                {
                    var cardShowObject = ResourceLoader.Load<PackedScene>("res://Pages/CardShowObject.tscn")
                        .Instantiate<CardShowObject>();
                    cardShowObject.InitDataByCard(card);
                    _flowContainer.AddChild(cardShowObject);
                }
            }
        }
        else
        {
            for (var i = 0; i < _flowContainer.GetChildCount(); i++)
            {
                var cardShowObject = (CardShowObject)_flowContainer.GetChild(i);
                if (i < cards.Count)
                {
                    cardShowObject.InitDataByCard(cards[i]);
                }
                else
                {
                    cardShowObject.QueueFree();
                }
            }
        }
    }
}