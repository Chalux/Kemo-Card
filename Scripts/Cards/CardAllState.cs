using Godot;
using KemoCard.Pages;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    internal partial class CardAllState : CardState
    {
        private ulong _recordTimeStamp;

        public override void Enter()
        {
            OnEnter();
            BattleStatic.CurrCard = CardObject;
        }

        private void OnEnter()
        {
            StaticInstance.EventMgr.Dispatch("DraggingCard", true);
            CardObject.AnimTween.Stop();
            CardObject.GlobalPosition = CardObject.GetGlobalMousePosition() - CardObject.PivotOffset;
            CardObject.BindRoot(CardObject.GetParent());
            Node uiLayer = StaticInstance.MainRoot;
            if (uiLayer != null)
            {
                CardObject.Reparent(uiLayer);
            }

            //cardObject.BgRect.Color = Colors.NavyBlue;
            CardObject.Rotation = 0;
            CardObject.StateLabel.Text = "ALLDRAGGING";
            _recordTimeStamp = Time.GetTicksMsec();
            switch (CardObject.Card.TargetType)
            {
                case TargetType.Self:
                    BattleStatic.Targets.Add(CardObject.Card.Owner);
                    StaticInstance.EventMgr.Dispatch("SelectTargetOwner", CardObject.Card.Owner);
                    break;
                case TargetType.EnemyAll:
                    if (StaticInstance.CurrWindow is BattleScene bs)
                    {
                        bs.CurrentEnemyRoles.ForEach(t => BattleStatic.Targets.Add(t));
                    }

                    StaticInstance.EventMgr.Dispatch("SelectTargetAll", CardObject.Card.TargetType);
                    break;
                case TargetType.All:
                    if (StaticInstance.CurrWindow is BattleScene bs2)
                    {
                        bs2.CurrentEnemyRoles.ForEach(t => BattleStatic.Targets.Add(t));
                        bs2.CurrentPlayerRoles.ForEach(t => BattleStatic.Targets.Add(t));
                    }

                    StaticInstance.EventMgr.Dispatch("SelectTargetAll", CardObject.Card.TargetType);
                    break;
                case TargetType.TeamAll:
                    if (StaticInstance.CurrWindow is BattleScene bs3)
                    {
                        bs3.CurrentPlayerRoles.ForEach(t => BattleStatic.Targets.Add(t));
                    }

                    StaticInstance.EventMgr.Dispatch("SelectTargetAll", CardObject.Card.TargetType);
                    break;
                case TargetType.EnemySingle:
                case TargetType.TeamSingle:
                case TargetType.AllSingle:
                    break;
                default:
                    StaticInstance.EventMgr.Dispatch("EndSelectTarget");
                    CardObject.Csm.OnTransitionRequest(this, CardStateEnum.Base);
                    BattleStatic.CurrCard = null;
                    break;
            }
        }

        public override void OnInput(InputEvent @event)
        {
            var mouseMotion = @event is InputEventMouseMotion;
            // var cancel = @event.IsActionPressed("right_mouse") || _recordTimeStamp + 200 > Time.GetTicksMsec();
            var confirm = (@event.IsActionPressed("left_mouse") || @event.IsActionReleased("left_mouse"));
            if (confirm && StaticInstance.CurrWindow is BattleScene bs)
            {
                var rectA = CardObject.GetGlobalRect();
                var rectB = bs.DragDownArea.GetGlobalRect();
                confirm = rectA.Intersection(rectB).Area > 0f;
            }

            if (mouseMotion)
            {
                CardObject.GlobalPosition = CardObject.GetGlobalMousePosition() - CardObject.PivotOffset;
            }
            else if (confirm)
            {
                //GD.Print(RecordTimeStamp + 10000 + ":" + Time.GetTicksMsec());
                GetViewport().SetInputAsHandled();
                StaticInstance.EventMgr.Dispatch("EndSelectTarget");
                CardObject.Csm.OnTransitionRequest(this, CardStateEnum.Released);
            }
            else
            {
                GetViewport().SetInputAsHandled();
                BattleStatic.Targets.Clear();
                StaticInstance.EventMgr.Dispatch("EndSelectTarget");
                CardObject.Csm.OnTransitionRequest(this, CardStateEnum.Base);
                BattleStatic.CurrCard = null;
            }
        }
    }
}