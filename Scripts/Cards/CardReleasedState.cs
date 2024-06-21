using Godot;
using StaticClass;
using System.Collections.Generic;
using System.Linq;
using static StaticClass.StaticEnums;

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
                    case CostType.HEALTH:
                        if (cardObject.card.owner.CurrHealth > cardObject.card.Cost)
                        {
                            cardObject.card.owner.CurrHealth -= cardObject.card.Cost;
                        }
                        else
                        {
                            StaticInstance.MainRoot.ShowBanner("血量不足以支付卡牌费用");
                            isCosted = false;
                        }
                        break;
                    case CostType.MAGIC:
                        if (cardObject.card.owner.CurrMagic > cardObject.card.Cost)
                        {
                            cardObject.card.owner.CurrMagic -= cardObject.card.Cost;
                        }
                        else
                        {
                            StaticInstance.MainRoot.ShowBanner("魔力不足以支付卡牌费用");
                            isCosted = false;
                        }
                        break;
                    case CostType.ACTIONPOINT:
                        if ((cardObject.card.owner as InFightPlayer).CurrentActionPoint >= cardObject.card.Cost)
                        {
                            (cardObject.card.owner as InFightPlayer).CurrentActionPoint -= cardObject.card.Cost;
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
            if (cardObject.card.UseFilter != null) flag = cardObject.card.UseFilter.Invoke(cardObject.card?.owner, roles, null);
            if (isCosted && BattleStatic.Targets.Count > 0 && flag)
            {
                cardObject.card.FunctionUse?.Invoke(cardObject.card.owner, roles, null);
                BattleStatic.currCard = null;
                BattleStatic.Targets.Clear();
                if (cardObject.card.owner is InFightPlayer ifp)
                {
                    if (!cardObject.card.GlobalDict.ContainsKey("Exhaust")
                    && !cardObject.card.InGameDict.ContainsKey("Exhaust"))
                    {
                        ifp.AddCardToGrave(cardObject.card);
                        cardObject.QueueFree();
                    }
                    else
                    {
                        var tween = CreateTween();
                        tween.TweenProperty((cardObject.Material as ShaderMaterial), "shader_parameter/dissolve_value", 0, 1f);
                        tween.TweenCallback(Callable.From(cardObject.QueueFree));
                        tween.Play();
                    }
                    ifp.InFightHands.Remove(cardObject.card);
                }
                cardObject.GetTree().CreateTimer(0.1f).Timeout += new(() =>
                {
                    StaticInstance.eventMgr.Dispatch("RepositionHand");
                    StaticInstance.eventMgr.Dispatch("DraggingCard");
                });
            }
            else
            {
                GetViewport().SetInputAsHandled();
                BattleStatic.Targets.Clear();
                StaticInstance.eventMgr.Dispatch("EndSelectTarget");
                cardObject.csm.OnTransitionRequest(this, State.BASE);
                BattleStatic.currCard = null;
            }
            BattleStatic.Targets.Clear();
        }
    }
}
