using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using System;
using System.Collections.Generic;

public partial class ChoseCardScene : BaseScene
{
    [Export] HBoxContainer hBox;
    [Export] Godot.Button confirmBtn;
    public override void OnAdd(dynamic datas = null)
    {
        BattleStatic.StartSelect();
        foreach (Card c in datas[0] as List<Card>)
        {
            var obj = new CardObject();
            obj.InitData(c);
            hBox.AddChild(obj);
        }
    }
}
