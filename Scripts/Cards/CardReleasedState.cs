using Godot;
using System.Collections.Generic;
using System.Linq;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    partial class CardReleasedState : CardState
    {
        public override void Enter()
        {
            List<BaseRole> roles = new();
            if (BattleStatic.Targets.Count > 0) roles = BattleStatic.Targets.ToList();
            bool isCosted = true;
            if (BattleStatic.Targets.Count > 0)
            {
                switch (cardObject.card.CostType)
                {
                    case CostType.Health:
                        if (cardObject.card.Owner.CurrHealth > cardObject.card.Cost)
                        {
                            cardObject.card.Owner.CurrHealth -= cardObject.card.Cost;
                        }
                        else
                        {
                            StaticInstance.MainRoot.ShowBanner("血量不足以支付卡牌费用");
                            isCosted = false;
                        }
                        break;
                    case CostType.Magic:
                        if (cardObject.card.Owner.CurrMagic > cardObject.card.Cost)
                        {
                            cardObject.card.Owner.CurrMagic -= cardObject.card.Cost;
                        }
                        else
                        {
                            StaticInstance.MainRoot.ShowBanner("魔力不足以支付卡牌费用");
                            isCosted = false;
                        }
                        break;
                    case CostType.ActionPoint:
                        var owner = cardObject.card.Owner as PlayerRole;
                        if (owner != null && owner.CurrentActionPoint >= cardObject.card.Cost)
                        {
                            owner.CurrentActionPoint -= cardObject.card.Cost;
                        }
                        else
                        {
                            StaticInstance.MainRoot.ShowBanner("行动力不足以支付卡牌费用");
                            isCosted = false;
                        }
                        break;
                }
            }
            bool flag = true;
            if (cardObject.card.UseFilter != null) flag = cardObject.card.UseFilter.Invoke(cardObject.card?.Owner, roles, null);
            if (isCosted && BattleStatic.Targets.Count > 0 && flag)
            {
                cardObject.card.FunctionUse?.Invoke(cardObject.card.Owner, roles, new[] { cardObject.card });
                BattleStatic.AddUsedCard(cardObject.card);
                GD.Print($"卡牌C{cardObject.card.Id}已使用");
                BattleStatic.currCard = null;
                BattleStatic.Targets.Clear();
                if (cardObject.card.Owner is PlayerRole ifp)
                {
                    if (!cardObject.card.CheckHasSymbol("Exhaust"))
                    {
                        ifp.AddCardToGrave(cardObject.card);
                        //if (StaticInstance.currWindow is BattleScene bs)
                        //{
                        //    Vector2 startPos = cardObject.Position;
                        //    Vector2 endPos = bs.GraveCount.GlobalPosition;
                        //    cardObject.MouseFilter = MouseFilterEnum.Ignore;
                        //    cardObject.Reparent(cardObject.GetParent());
                        //    Tween tween = cardObject.GetTree().CreateTween();
                        //    float duration = 1.0f;
                        //    tween.SetEase(Tween.EaseType.Out);
                        //    tween.SetParallel(true);
                        //    tween.TweenProperty(cardObject, "global_position", bs.GraveCount.GlobalPosition, duration);
                        //    tween.TweenProperty(cardObject, "scale", Vector2.Zero, duration);
                        //    tween.TweenCallback(Callable.From(cardObject.QueueFree));
                        //}
                        //else
                        //{
                        //    cardObject.QueueFree();
                        //}
                        cardObject.QueueFree();
                    }
                    else
                    {
                        cardObject.SVContainer.UseParentMaterial = true;
                        cardObject.ShowHint = false;
                        var tween = CreateTween();
                        tween.TweenProperty((cardObject.Material as ShaderMaterial), "shader_parameter/dissolve_value", 0, 1f);
                        tween.TweenCallback(Callable.From(cardObject.QueueFree));
                        tween.Play();
                    }
                    ifp.InFightHands.Remove(cardObject.card);
                }
                cardObject.GetTree().CreateTimer(0.1f).Timeout += new(() =>
                {
                    StaticInstance.EventMgr.Dispatch("RepositionHand");
                    StaticInstance.EventMgr.Dispatch("DraggingCard");
                });
            }
            else
            {
                GetViewport().SetInputAsHandled();
                BattleStatic.Targets.Clear();
                StaticInstance.EventMgr.Dispatch("EndSelectTarget");
                cardObject.csm.OnTransitionRequest(this, CardStateEnum.Base);
                BattleStatic.currCard = null;
            }
            BattleStatic.Targets.Clear();
        }
    }
}
