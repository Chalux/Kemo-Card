using Godot;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    internal partial class CardAllState : CardState
    {
        private ulong RecordTimeStamp = 0;
        public override void Enter()
        {
            OnEnter();
            BattleStatic.currCard = cardObject;
        }

        void OnEnter()
        {
            StaticInstance.EventMgr.Dispatch("DraggingCard", true);
            cardObject.AnimTween.Stop();
            cardObject.GlobalPosition = cardObject.GetGlobalMousePosition() - cardObject.PivotOffset;
            cardObject.BindRoot(cardObject.GetParent());
            Node uiLayer = StaticInstance.MainRoot;
            if (uiLayer != null)
            {
                cardObject.Reparent(uiLayer);
            }
            //cardObject.BgRect.Color = Colors.NavyBlue;
            cardObject.Rotation = 0;
            cardObject.stateLabel.Text = "ALLDRAGGING";
            RecordTimeStamp = Time.GetTicksMsec();
            switch (cardObject.card.TargetType)
            {
                case TargetType.Self:
                    BattleStatic.Targets.Add(cardObject.card.Owner);
                    StaticInstance.EventMgr.Dispatch("SelectTargetOwner", cardObject.card.Owner);
                    break;
                case TargetType.EnemyAll:
                    if (StaticInstance.CurrWindow is Pages.BattleScene bs)
                    {
                        bs.CurrentEnemyRoles.ForEach(t => BattleStatic.Targets.Add(t));
                    }
                    StaticInstance.EventMgr.Dispatch("SelectTargetAll", cardObject.card.TargetType);
                    break;
                case TargetType.All:
                    if (StaticInstance.CurrWindow is Pages.BattleScene bs2)
                    {
                        bs2.CurrentEnemyRoles.ForEach(t => BattleStatic.Targets.Add(t));
                        bs2.CurrentPlayerRoles.ForEach(t => BattleStatic.Targets.Add(t));
                    }
                    StaticInstance.EventMgr.Dispatch("SelectTargetAll", cardObject.card.TargetType);
                    break;
                case TargetType.TeamAll:
                    if (StaticInstance.CurrWindow is Pages.BattleScene bs3)
                    {
                        bs3.CurrentPlayerRoles.ForEach(t => BattleStatic.Targets.Add(t));
                    }
                    StaticInstance.EventMgr.Dispatch("SelectTargetAll", cardObject.card.TargetType);
                    break;
                default:
                    StaticInstance.EventMgr.Dispatch("EndSelectTarget");
                    cardObject.csm.OnTransitionRequest(this, CardStateEnum.Base);
                    BattleStatic.currCard = null;
                    break;
            }
        }

        public override void OnInput(InputEvent @event)
        {
            bool mouseMotion = @event is InputEventMouseMotion;
            bool cancel = @event.IsActionPressed("right_mouse") || RecordTimeStamp + 200 > Time.GetTicksMsec();
            bool confirm = (@event.IsActionPressed("left_mouse") || @event.IsActionReleased("left_mouse"));
            if (confirm && StaticInstance.CurrWindow is Pages.BattleScene bs)
            {
                var rectA = cardObject.GetGlobalRect();
                var rectB = bs.DragDownArea.GetGlobalRect();
                confirm = rectA.Intersection(rectB).Area > 0f;
            }

            if (mouseMotion)
            {
                cardObject.GlobalPosition = cardObject.GetGlobalMousePosition() - cardObject.PivotOffset;
            }
            else if (confirm)
            {
                //GD.Print(RecordTimeStamp + 10000 + ":" + Time.GetTicksMsec());
                GetViewport().SetInputAsHandled();
                StaticInstance.EventMgr.Dispatch("EndSelectTarget");
                cardObject.csm.OnTransitionRequest(this, CardStateEnum.Released);
            }
            else if (cancel || !confirm)
            {
                GetViewport().SetInputAsHandled();
                BattleStatic.Targets.Clear();
                StaticInstance.EventMgr.Dispatch("EndSelectTarget");
                cardObject.csm.OnTransitionRequest(this, CardStateEnum.Base);
                BattleStatic.currCard = null;
            }
        }
    }
}
