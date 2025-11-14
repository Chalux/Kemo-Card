using System.Collections.Generic;
using System.Linq;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

namespace KemoCard.Pages;

public partial class SelectCardScene : BaseScene, IEvent
{
    //private Card currCard = null;
    private List<string> _data = [];
    private int _currIdx = -1;

    private int CurrIdx
    {
        get => _currIdx;
        set
        {
            _currIdx = value;
            UpdateView();
        }
    }

    [Export] public HBoxContainer boxContainer;
    [Export] Godot.Button SelectBtn;

    public SelectCardScene()
    {
        StaticInstance.EventMgr.RegisterIEvent(this);
    }

    ~SelectCardScene()
    {
        StaticInstance.EventMgr.UnregisterIEvent(this);
    }

    public void Init(List<string> list)
    {
        _data = list;
        foreach (var child in boxContainer?.GetChildren() ?? [])
            boxContainer?.RemoveChild(child);
        foreach (var id in list)
        {
            if (string.IsNullOrEmpty(id)) continue;
            var card = new Card(id)
            {
                IsTemp = true
            };
            var item = ResourceLoader.Load<PackedScene>("res://Pages/SelectCardItem.tscn")
                .Instantiate<SelectCardItem>();
            item.showObject.InitDataByCard(card);
            item.GuiInput += @event => { OnItemGuiInput(@event, item); };
            boxContainer?.AddChild(item);
        }

        UpdateView();
        SelectBtn.Disabled = true;
        SelectBtn.Pressed += () =>
        {
            if (CurrIdx <= 0) return;
            StaticInstance.PlayerData.Gsd.MajorRole.TempDeck.Add((boxContainer?.GetChild(CurrIdx) as SelectCardItem)
                ?.showObject.Card);
            CurrIdx = -1;
            var ms = ResourceLoader.Load<PackedScene>("res://Pages/MainScene.tscn")
                .Instantiate<MainScene>();
            StaticInstance.WindowMgr.ChangeScene(ms);
        };
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
    }

    private void UpdateView()
    {
        foreach (var item in boxContainer?.GetChildren().Cast<SelectCardItem>() ?? [])
        {
            item.shaderRect.Visible = CurrIdx == item.GetIndex();
        }
    }

    private void OnItemGuiInput(InputEvent @event, SelectCardItem item)
    {
        if (!@event.IsActionPressed("left_mouse")) return;
        CurrIdx = _data.IndexOf(item.showObject.Card.Id);
        UpdateView();
        SelectBtn.Disabled = false;
    }
}