using Godot;
using KemoCard.Pages;
using StaticClass;
using static StaticClass.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    public partial class CardDraggingState : CardState
    {
        private ulong RecordTimeStamp = 0;
        public override void Enter()
        {
            OnEnter();
            BattleStatic.currCard = cardObject;
        }

        void OnEnter()
        {
            StaticInstance.eventMgr.Dispatch("DraggingCard", new object[] { true });
            cardObject.AnimTween.Stop();
            cardObject.BindRoot(cardObject.GetParent());
            Node uiLayer = StaticInstance.MainRoot;
            if (uiLayer != null)
            {
                cardObject.Reparent(uiLayer);
            }
            cardObject.colorRect.Color = Colors.NavyBlue;
            cardObject.Rotation = 0;
            cardObject.stateLabel.Text = "DRAGGING";
            RecordTimeStamp = Time.GetTicksMsec();
            switch (cardObject.card.TargetType)
            {
                case TargetType.ENEMY_SINGLE:
                case TargetType.ALL_SINGLE:
                case TargetType.TEAM_SINGLE:
                    StaticInstance.eventMgr.Dispatch("StartSelectTarget", cardObject.card.TargetType);
                    break;
                default:
                    StaticInstance.eventMgr.Dispatch("EndSelectTarget");
                    cardObject.csm.OnTransitionRequest(this, State.BASE);
                    BattleStatic.currCard = null;
                    return;
            }
            cardObject.BezierControl.Visible = true;
        }

        public override void Exit()
        {
            cardObject.BezierControl.Visible = false;
            base.Exit();
        }

        public override void OnInput(InputEvent @event)
        {
            bool mouseMotion = @event is InputEventMouseMotion;
            bool cancel = @event.IsActionPressed("right_mouse") || RecordTimeStamp + 200 > Time.GetTicksMsec();
            bool confirm = (@event.IsActionPressed("left_mouse") || @event.IsActionReleased("left_mouse"));
            if (confirm && StaticInstance.currWindow is BattleScene bs)
            {
                var rectA = cardObject.GetGlobalRect();
                var rectB = bs.DragDwonArea.GetGlobalRect();
                confirm = rectA.Intersection(rectB).Area > 0f;
            }

            if (mouseMotion)
            {
                //cardObject.GlobalPosition = cardObject.GetGlobalMousePosition() - cardObject.PivotOffset;
                (cardObject.BezierControl as Bezier).Reset(cardObject.GlobalPosition + cardObject.Size / 2, GetGlobalMousePosition());
            }
            else if (confirm)
            {
                //GD.Print(RecordTimeStamp + 10000 + ":" + Time.GetTicksMsec());
                GetViewport().SetInputAsHandled();
                StaticInstance.eventMgr.Dispatch("EndSelectTarget");
                cardObject.csm.OnTransitionRequest(this, State.RELEASED);
            }
            else if (cancel || !confirm)
            {
                GetViewport().SetInputAsHandled();
                BattleStatic.Targets.Clear();
                StaticInstance.eventMgr.Dispatch("EndSelectTarget");
                cardObject.csm.OnTransitionRequest(this, State.BASE);
                BattleStatic.currCard = null;
            }
        }
    }
}
