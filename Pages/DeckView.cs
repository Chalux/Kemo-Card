using Godot;
using KemoCard.Pages;
using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;

public partial class DeckView : BaseScene
{
    [Export] FlowContainer FlowContainer;
    [Export] Godot.Button ExitBtn;
    public override void OnAdd(params object[] datas)
    {
        ExitBtn.Pressed += new(() =>
        {
            StaticInstance.windowMgr.RemoveScene(this);
        });
        if (datas[0] is List<Card> deck)
        {
            deck.ForEach(card =>
            {
                var cardobject = ResourceLoader.Load<PackedScene>($"res://Pages/CardShowObject.tscn").Instantiate<CardShowObject>();
                cardobject.InitDataByCard(card);
                FlowContainer.AddChild(cardobject);
            });
        }
    }
}
