using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;
using System.Linq;

public partial class SelectCardScene : BaseScene, IEvent
{
    //private Card currCard = null;
    private List<string> data = new();
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
        StaticInstance.eventMgr.RegistIEvent(this);
    }

    ~SelectCardScene()
    {
        StaticInstance.eventMgr.UnregistIEvent(this);
    }

    public void Init(List<string> list)
    {
        data = list;
        foreach (Node child in boxContainer?.GetChildren())
            boxContainer?.RemoveChild(child);
        for (int i = 0; i < list.Count; i++)
        {
            string id = list[i];
            if (id != null || id != "")
            {
                var card = new Card(id)
                {
                    IsTemp = true
                };
                SelectCardItem item = ResourceLoader.Load<PackedScene>("res://Pages/SelectCardItem.tscn").Instantiate<SelectCardItem>();
                item.showObject.InitDataByCard(card);
                item.GuiInput += (InputEvent @event) =>
                {
                    OnItemGuiInput(@event, item);
                };
                boxContainer?.AddChild(item);
            }
        }
        UpdateView();
        SelectBtn.Disabled = true;
        SelectBtn.Pressed += new(() =>
        {
            if (CurrIdx > 0)
            {
                StaticInstance.playerData.gsd.MajorRole.TempDeck.Add((boxContainer?.GetChild(CurrIdx) as SelectCardItem).showObject.card);
                CurrIdx = -1;
                MainScene ms = ResourceLoader.Load<PackedScene>("res://Pages/MainScene.tscn").Instantiate<MainScene>();
                StaticInstance.windowMgr.ChangeScene(ms);
            }
        });
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {

    }

    private void UpdateView()
    {
        foreach (SelectCardItem item in boxContainer?.GetChildren().Cast<SelectCardItem>())
        {
            item.shaderRect.Visible = CurrIdx == item.GetIndex();
        }
    }

    private void OnItemGuiInput(InputEvent @event, SelectCardItem item)
    {
        if (@event.IsActionPressed("left_mouse"))
        {
            CurrIdx = data.IndexOf(item.showObject.card.Id);
            UpdateView();
            SelectBtn.Disabled = false;
        }
    }
}
