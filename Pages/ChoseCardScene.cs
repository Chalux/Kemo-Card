using System.Collections.Generic;
using System.Linq;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Pages;

public partial class ChoseCardScene : BaseScene, IEvent
{
    [Export] private HBoxContainer _hBox;
    [Export] private Button _confirmBtn;
    private int _min = 1;
    private int _max = 1;

    public override void OnAdd(params object[] datas)
    {
        BattleStatic.StartSelect();
        foreach (var c in (List<Card>)datas[0])
        {
            var cardObj = ResourceLoader.Load<PackedScene>("res://Pages/CardObject.tscn");
            if (cardObj == null) continue;
            //var obj = new CardObject();
            var obj = cardObj.Instantiate<CardObject>();
            obj.InitData(c);
            _hBox?.AddChild(obj);
        }

        _min = (int)datas[1];
        _max = (int)datas[2];
        _confirmBtn.Pressed += Confirm;
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        if (@event != "SelectCard") return;
        var nowCount = _hBox?.GetChildren()?.Cast<CardObject>()
            .Count(obj => obj.Csm.CurrentState.State == CardStateEnum.Discarding);
        _confirmBtn.Disabled = nowCount < _min || nowCount > _max;
    }

    private void Confirm()
    {
        List<Card> cards = [];
        cards.AddRange(from obj in _hBox?.GetChildren()?.Cast<CardObject>()
            where obj.Csm.CurrentState.State == CardStateEnum.Discarding
            select obj.Card);

        if (cards.Count < _min || cards.Count > _max) return;
        BattleStatic.EndSelect();
        if (BattleStatic.IsFighting)
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