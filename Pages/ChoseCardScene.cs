using System.Collections.Generic;
using System.Linq;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Pages;

public partial class ChoseCardScene : BaseScene, IEvent
{
    [Export] HBoxContainer hBox;
    [Export] Godot.Button confirmBtn;
    private int _min = 1;
    private int _max = 1;

    public override void OnAdd(params object[] datas)
    {
        BattleStatic.StartSelect();
        foreach (var c in (List<Card>)datas[0])
        {
            var cardobj = ResourceLoader.Load<PackedScene>("res://Pages/CardObject.tscn");
            if (cardobj == null) continue;
            //var obj = new CardObject();
            var obj = cardobj.Instantiate<CardObject>();
            obj.InitData(c);
            hBox?.AddChild(obj);
        }

        _min = (int)datas[1];
        _max = (int)datas[2];
        confirmBtn.Pressed += Confirm;
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        if (@event != "SelectCard") return;
        var nowCount = hBox?.GetChildren()?.Cast<CardObject>()
            .Count(obj => obj.csm.currentState.state == CardStateEnum.Discarding);
        confirmBtn.Disabled = nowCount < _min || nowCount > _max;
    }

    private void Confirm()
    {
        List<Card> cards = [];
        cards.AddRange(from obj in hBox?.GetChildren()?.Cast<CardObject>()
            where obj.csm.currentState.state == CardStateEnum.Discarding
            select obj.card);

        if (cards.Count < _min || cards.Count > _max) return;
        BattleStatic.EndSelect();
        if (BattleStatic.isFighting)
        {
            if (StaticInstance.WindowMgr.GetSceneByName("BattleScene") is BattleScene bs)
            {
                foreach (var c in cards)
                {
                    bs.NowPlayer.AddCardIntoInfightHand(c);
                }
            }
        }

        StaticInstance.WindowMgr.RemoveScene(this);
    }
}