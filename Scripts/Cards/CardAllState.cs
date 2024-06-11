using Godot;
using StaticClass;
using static StaticClass.StaticEnums;

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
            StaticInstance.eventMgr.Dispatch("DraggingCard", new object[] { true });
            cardObject.AnimTween.Stop();
            cardObject.GlobalPosition = cardObject.GetGlobalMousePosition() - cardObject.PivotOffset;
            cardObject.BindRoot(cardObject.GetParent());
            Node uiLayer = StaticInstance.MainRoot;
            if (uiLayer != null)
            {
                cardObject.Reparent(uiLayer);
            }
            cardObject.colorRect.Color = Colors.NavyBlue;
            cardObject.Rotation = 0;
            cardObject.stateLabel.Text = "ALLDRAGGING";
            RecordTimeStamp = Time.GetTicksMsec();
            switch (cardObject.card.TargetType)
            {
                case TargetType.SELF:
                    BattleStatic.Targets.Add(cardObject.card.owner);
                    StaticInstance.eventMgr.Dispatch("SelectTargetOwner", cardObject.card.owner);
                    break;
                case TargetType.ENEMY_ALL:
                    if (StaticInstance.currWindow is BattleScene bs)
                    {
                        bs.currentEnemyRoles.ForEach(t => BattleStatic.Targets.Add(t));
                    }
                    StaticInstance.eventMgr.Dispatch("SelectTargetAll", cardObject.card.TargetType);
                    break;
                case TargetType.ALL:
                    if (StaticInstance.currWindow is BattleScene bs2)
                    {
                        bs2.currentEnemyRoles.ForEach(t => BattleStatic.Targets.Add(t));
                        bs2.currentPlayerRoles.ForEach(t => BattleStatic.Targets.Add(t));
                    }
                    StaticInstance.eventMgr.Dispatch("SelectTargetAll", cardObject.card.TargetType);
                    break;
                case TargetType.TEAM_ALL:
                    if (StaticInstance.currWindow is BattleScene bs3)
                    {
                        bs3.currentPlayerRoles.ForEach(t => BattleStatic.Targets.Add(t));
                    }
                    StaticInstance.eventMgr.Dispatch("SelectTargetAll", cardObject.card.TargetType);
                    break;
                default:
                    StaticInstance.eventMgr.Dispatch("EndSelectTarget");
                    cardObject.csm.OnTransitionRequest(this, State.BASE);
                    BattleStatic.currCard = null;
                    break;
            }
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
                cardObject.GlobalPosition = cardObject.GetGlobalMousePosition() - cardObject.PivotOffset;
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
