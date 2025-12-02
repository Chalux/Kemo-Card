using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    internal partial class CardReleasedState : CardState
    {
        public override void Enter()
        {
            List<BaseRole> roles = [];
            if (BattleStatic.Targets.Count > 0) roles = BattleStatic.Targets.ToList();
            var isCosted = true;
            if (BattleStatic.Targets.Count > 0)
            {
                switch (CardObject.Card.CostType)
                {
                    case CostType.Health:
                        if (CardObject.Card.Owner.CurrHealth > CardObject.Card.Cost)
                        {
                            CardObject.Card.Owner.CurrHealth -= CardObject.Card.Cost;
                        }
                        else
                        {
                            StaticInstance.MainRoot.ShowBanner("血量不足以支付卡牌费用");
                            isCosted = false;
                        }

                        break;
                    case CostType.Magic:
                        if (CardObject.Card.Owner.CurrMagic > CardObject.Card.Cost)
                        {
                            CardObject.Card.Owner.CurrMagic -= CardObject.Card.Cost;
                        }
                        else
                        {
                            StaticInstance.MainRoot.ShowBanner("魔力不足以支付卡牌费用");
                            isCosted = false;
                        }

                        break;
                    case CostType.ActionPoint:
                        if (CardObject.Card.Owner is PlayerRole owner &&
                            owner.CurrentActionPoint >= CardObject.Card.Cost)
                        {
                            owner.CurrentActionPoint -= CardObject.Card.Cost;
                        }
                        else
                        {
                            StaticInstance.MainRoot.ShowBanner("行动力不足以支付卡牌费用");
                            isCosted = false;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var flag = CardObject.Card?.Script?.UseFilter(CardObject.Card, CardObject.Card.Owner, roles, null) ?? true;
            if (isCosted && BattleStatic.Targets.Count > 0 && flag)
            {
                CardObject.Card?.Script?.UseFunction(CardObject.Card, CardObject.Card.Owner, roles);
                BattleStatic.AddUsedCard(CardObject.Card);
                GD.Print($"卡牌C{CardObject.Card?.Id}已使用");
                BattleStatic.CurrCard = null;
                BattleStatic.Targets.Clear();
                if (CardObject.Card?.Owner is PlayerRole ifp)
                {
                    if (!CardObject.Card.CheckHasSymbol("Exhaust"))
                    {
                        ifp.AddCardToGrave(CardObject.Card);
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
                        CardObject.QueueFree();
                    }
                    else
                    {
                        CardObject.SvContainer.UseParentMaterial = true;
                        CardObject.ShowHint = false;
                        var tween = CreateTween();
                        tween.TweenProperty(CardObject.Material as ShaderMaterial, "shader_parameter/dissolve_value",
                            0, 1f);
                        tween.TweenCallback(Callable.From(CardObject.QueueFree));
                        tween.Play();
                    }

                    ifp.InFightHands.Remove(CardObject.Card);
                }

                CardObject.GetTree().CreateTimer(0.1f).Timeout += () =>
                {
                    StaticInstance.EventMgr.Dispatch("RepositionHand");
                    StaticInstance.EventMgr.Dispatch("DraggingCard");
                };
            }
            else
            {
                GetViewport().SetInputAsHandled();
                BattleStatic.Targets.Clear();
                StaticInstance.EventMgr.Dispatch("EndSelectTarget");
                CardObject.Csm.OnTransitionRequest(this, CardStateEnum.Base);
                BattleStatic.CurrCard = null;
            }

            BattleStatic.Targets.Clear();
        }
    }
}