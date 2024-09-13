using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;
using System.Linq;
using static StaticClass.StaticEnums;

public partial class ChoseCardScene : BaseScene, IEvent
{
    [Export] HBoxContainer hBox;
    [Export] Godot.Button confirmBtn;
    private int min = 1;
    private int max = 1;
    public override void OnAdd(params object[] datas)
    {
        BattleStatic.StartSelect();
        foreach (Card c in datas[0] as List<Card>)
        {
            PackedScene cardobj = ResourceLoader.Load<PackedScene>("res://Pages/CardObject.tscn");
            if (cardobj == null) continue;
            //var obj = new CardObject();
            var obj = cardobj.Instantiate<CardObject>();
            obj.InitData(c);
            hBox?.AddChild(obj);
        }
        min = (int)datas[1];
        max = (int)datas[2];
        confirmBtn.Pressed += Confirm;
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        if (@event == "SelectCard")
        {
            int nowCount = 0;
            foreach (CardObject obj in hBox?.GetChildren())
            {
                if (obj.csm.currentState.state == CardStateEnum.DISCARDING) nowCount++;
            }
            confirmBtn.Disabled = nowCount < min || nowCount > max;
        }
    }

    private void Confirm()
    {
        List<Card> cards = new();
        foreach (CardObject obj in hBox?.GetChildren().Cast<CardObject>())
        {
            if (obj.csm.currentState.state == CardStateEnum.DISCARDING) cards.Add(obj.card);
        }
        if (cards.Count < min || cards.Count > max) return;
        BattleStatic.EndSelect();
        if (BattleStatic.isFighting)
        {
            BattleScene bs = StaticInstance.windowMgr.GetSceneByName("BattleScene") as BattleScene;
            if (bs != null)
            {
                foreach (Card c in cards)
                {
                    bs.nowPlayer.AddCardIntoInfightHand(c);
                }
            }
        }
        StaticInstance.windowMgr.RemoveScene(this);
    }
}
