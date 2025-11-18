using System.Collections.Generic;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

namespace KemoCard.Pages;

public partial class DeckView : BaseScene
{
    [Export] private FlowContainer _flowContainer;
    [Export] private Button _exitBtn;

    public override void OnAdd(params object[] datas)
    {
        _exitBtn.Pressed += () => { StaticInstance.WindowMgr.RemoveScene(this); };
        if (datas[0] is List<Card> deck)
        {
            deck.ForEach(card =>
            {
                var cardObject = ResourceLoader.Load<PackedScene>("res://Pages/CardShowObject.tscn")
                    .Instantiate<CardShowObject>();
                cardObject.InitDataByCard(card);
                _flowContainer.AddChild(cardObject);
            });
        }
    }
}