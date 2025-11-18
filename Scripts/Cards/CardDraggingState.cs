using Godot;
using KemoCard.Pages;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    public partial class CardDraggingState : CardState
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
            CardObject.BindRoot(CardObject.GetParent());
            Node uiLayer = StaticInstance.MainRoot;
            if (uiLayer != null)
            {
                CardObject.Reparent(uiLayer);
            }

            //cardObject.BgRect.Color = Colors.NavyBlue;
            CardObject.Rotation = 0;
            CardObject.StateLabel.Text = "DRAGGING";
            _recordTimeStamp = Time.GetTicksMsec();
            switch (CardObject.Card.TargetType)
            {
                case TargetType.EnemySingle:
                case TargetType.AllSingle:
                case TargetType.TeamSingle:
                    StaticInstance.EventMgr.Dispatch("StartSelectTarget", CardObject.Card.TargetType);
                    break;
                case TargetType.EnemyAll:
                case TargetType.TeamAll:
                case TargetType.Self:
                case TargetType.All:
                    break;
                default:
                    StaticInstance.EventMgr.Dispatch("EndSelectTarget");
                    CardObject.Csm.OnTransitionRequest(this, CardStateEnum.Base);
                    BattleStatic.CurrCard = null;
                    return;
            }

            CardObject.BezierControl.Visible = true;
        }

        public override void Exit()
        {
            CardObject.BezierControl.Visible = false;
            base.Exit();
        }

        public override void OnInput(InputEvent @event)
        {
            var mouseMotion = @event is InputEventMouseMotion;
            var cancel = @event.IsActionPressed("right_mouse") || _recordTimeStamp + 200 > Time.GetTicksMsec();
            var confirm = (@event.IsActionPressed("left_mouse") || @event.IsActionReleased("left_mouse"));
            if (confirm && StaticInstance.CurrWindow is BattleScene bs)
            {
                var rectA = CardObject.GetGlobalRect();
                var rectB = bs.DragDownArea.GetGlobalRect();
                confirm = rectA.Intersection(rectB).Area > 0f;
            }

            if (mouseMotion)
            {
                //cardObject.GlobalPosition = cardObject.GetGlobalMousePosition() - cardObject.PivotOffset;
                if (CardObject.BezierControl is Bezier bezier)
                    bezier.Reset(CardObject.GlobalPosition + CardObject.Size / 2, GetGlobalMousePosition());
            }
            else if (confirm && BattleStatic.IsFighting)
            {
                //GD.Print(RecordTimeStamp + 10000 + ":" + Time.GetTicksMsec());
                GetViewport().SetInputAsHandled();
                StaticInstance.EventMgr.Dispatch("EndSelectTarget");
                CardObject.Csm.OnTransitionRequest(this, CardStateEnum.Released);
            }
            else if (cancel || !confirm)
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