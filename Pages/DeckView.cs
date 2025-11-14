using System.Collections.Generic;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

namespace KemoCard.Pages;

public partial class DeckView : BaseScene
{
    [Export] FlowContainer FlowContainer;
    [Export] Godot.Button ExitBtn;

    public override void OnAdd(params object[] datas)
    {
        ExitBtn.Pressed += () => { StaticInstance.WindowMgr.RemoveScene(this); };
        if (datas[0] is List<Card> deck)
        {
            deck.ForEach(card =>
            {
                var cardobject = ResourceLoader.Load<PackedScene>($"res://Pages/CardShowObject.tscn")
                    .Instantiate<KemoCard.Pages.CardShowObject>();
                cardobject.InitDataByCard(card);
                FlowContainer.AddChild(cardobject);
            });
        }
    }
}