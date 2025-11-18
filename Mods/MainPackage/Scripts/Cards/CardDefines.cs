using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

namespace KemoCard.Mods.MainPackage.Scripts.Cards;

internal partial class CreateBook : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.GlobalDict["Exhaust"] = 1;
        c.FunctionUse = UseFunction;
        c.HintCardIds = ["create_book_attack", "create_book_armor"];
    }

    private static void UseFunction(BaseRole user, List<BaseRole> targets, params object[] datas)
    {
        Card c1 = new("create_book_attack");
        Card c2 = new("create_book_armor");
        List<Card> cs = [c1, c2];
        var bs = StaticUtils.TryGetBattleScene();
        if (bs == null) return;
        var res = ResourceLoader.Load<PackedScene>("res://Pages/ChoseCardScene.tscn");
        if (res == null) return;
        var scene = res.Instantiate<Pages.ChoseCardScene>();
        StaticInstance.WindowMgr.AddScene(scene, cs, 1, 1);
    }
}

internal partial class CreateBookArmor : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.FunctionUse = UseFunction;
    }

    private static void UseFunction(BaseRole user, List<BaseRole> targets, params object[] datas)
    {
        var bs = StaticUtils.TryGetBattleScene();
        if (bs != null && user is PlayerRole pr)
        {
            pr.CurrMBlock += 2 + user.CurrCraftBook;
        }
    }
}

internal partial class CreateBookAttack : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.FunctionUse = UseFunction;
    }

    private static void UseFunction(BaseRole user, List<BaseRole> targets, params object[] datas)
    {
        var bs = StaticUtils.TryGetBattleScene();
        bs?.DealDamage(5 + user.CurrCraftBook, StaticEnums.AttackType.Magic, user, targets);
    }
}

internal partial class CreateEquip : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.GlobalDict["Exhaust"] = 1;
        c.HintCardIds = ["create_equip_attack", "create_equip_armor"];
        c.FunctionUse = UseFunction;
    }

    private static void UseFunction(BaseRole user, List<BaseRole> targets, dynamic[] datas)
    {
        Card c1 = new("create_equip_attack");
        Card c2 = new("create_equip_armor");
        List<Card> list = [c1, c2];
        var bs = StaticUtils.TryGetBattleScene();
        if (bs == null) return;
        var res = ResourceLoader.Load<PackedScene>("res://Pages/ChoseCardScene.tscn");
        if (res == null) return;
        var scene = res.Instantiate<Pages.ChoseCardScene>();
        StaticInstance.WindowMgr.AddScene(scene, list, 1, 1);
    }
}

internal partial class CreateEquipArmor : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.FunctionUse = UseFunction;
    }

    private static void UseFunction(BaseRole user, List<BaseRole> targets, params object[] datas)
    {
        if (StaticInstance.CurrWindow is Pages.BattleScene && user is PlayerRole pr)
        {
            pr.CurrPBlock += 2 + user.CurrCraftEquip;
        }
    }
}

internal partial class CreateEquipAttack : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.FunctionUse = UseFunction;
    }

    private static void UseFunction(BaseRole user, List<BaseRole> targets, params object[] datas)
    {
        if (StaticInstance.CurrWindow is Pages.BattleScene bs)
        {
            bs.DealDamage(5 + user.CurrCraftEquip, StaticEnums.AttackType.Physics, user, targets);
        }
    }
}

internal partial class DoubleHit : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.UseFilter = (_, _, _) => false;
        c.DiscardAction = (user, reason, _) =>
        {
            if (reason != Pages.BattleScene.DisCardReason.Effect) return;
            if (StaticInstance.CurrWindow is not Pages.BattleScene bs) return;
            if (!BattleStatic.IsFighting) return;
            List<BaseRole> tars = [];
            Random r = new();
            tars.Add(bs.CurrentEnemyRoles[r.Next(bs.CurrentEnemyRoles.Count)]);
            bs.DealDamage(14, StaticEnums.AttackType.Physics, user, tars);
        };
    }
}

internal partial class FireBall : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.UseFilter = (_, targets, _) => targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
        c.FunctionUse = (user, targets, _) =>
        {
            if (StaticInstance.CurrWindow is not Pages.BattleScene bs) return;
            if (BattleStatic.IsFighting)
            {
                bs.DealDamage(7, StaticEnums.AttackType.Magic, user, targets, StaticEnums.AttributeEnum.Fire);
            }
        };
    }
}

internal partial class GracefulCharity : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.Cost = 8;
        c.Alias = "〇〇的施舍";
        c.Desc = "抽3张牌，然后选择2张手牌丢弃";
        c.TargetType = StaticEnums.TargetType.Self;
        c.CostType = StaticEnums.CostType.Magic;
        c.UseFilter = (_, targets, _) => targets is { Count: > 0 } && targets[0] is PlayerRole { IsFriendly: true };
        c.FunctionUse = (user, targets, _) =>
        {
            if (StaticInstance.CurrWindow is not Pages.BattleScene bs) return;
            if (!BattleStatic.IsFighting) return;
            bs.DrawCard(3, (PlayerRole)targets[0]);
            bs.SelectCard(2, 2, null,
                cards => { bs.DisCard(cards, (PlayerRole)targets[0], Pages.BattleScene.DisCardReason.Effect, user); });
        };
    }
}

internal partial class Infinite : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.FunctionUse = ActionFunc;
    }

    private static void ActionFunc(BaseRole user, List<BaseRole> targets, dynamic[] datas)
    {
        if (targets is not { Count: > 0 }) return;
        var bs = StaticUtils.TryGetBattleScene();
        bs?.DealDamage(user.Body + user.MagicAbility + user.Knowlegde + user.Technique,
            StaticEnums.AttackType.Physics, user, targets);
    }
}

internal partial class IronShard : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.FunctionUse = UseFunction;
    }

    private static void UseFunction(BaseRole user, List<BaseRole> targets, dynamic[] datas)
    {
        if (user is not PlayerRole pr) return;
        if (datas[0] is not Card card) return;
        pr.CurrPBlock += (int)Math.Max(0, 10 - card.InGameDict.GetValueOrDefault("usecount", 0));
        card.InGameDict["usecount"] = card.InGameDict.GetValueOrDefault("usecount", 0) + 1;
    }
}

internal partial class Lucky : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.FunctionUse = (_, _, _) =>
        {
            if (c.Owner is PlayerRole inFightPlayer)
            {
                inFightPlayer.CurrentActionPoint += 1;
            }
        };
        c.GlobalDict["Exhaust"] = 1;
        c.GlobalDict["KeepInHand"] = 1;
    }
}

internal partial class MagDraw : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.UseFilter = (_, targets, _) => targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
        c.FunctionUse = (user, targets, _) =>
        {
            if (StaticInstance.CurrWindow is not Pages.BattleScene bs) return;
            if (!BattleStatic.IsFighting) return;
            if (targets[0] is not EnemyRole er) return;
            var oldHealth = er.CurrHealth;
            bs.DealDamage(3 + user.CurrEffeciency, StaticEnums.AttackType.Magic, user, targets);
            if (er.CurrHealth < oldHealth && user is PlayerRole pr)
            {
                bs.DrawCard(1, pr);
            }
        };
    }
}

internal partial class MagicMissile : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.UseFilter = (_, targets, _) => targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
        c.FunctionUse = (user, targets, _) =>
        {
            if (StaticInstance.CurrWindow is not Pages.BattleScene bs) return;
            if (BattleStatic.IsFighting)
            {
                bs.DealDamage(5, StaticEnums.AttackType.Physics, user, targets);
            }
        };
    }
}

internal partial class ManaTide : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.FunctionUse += UseFunction;
    }

    private static void UseFunction(BaseRole user, List<BaseRole> targets, dynamic[] datas)
    {
        if (user is not PlayerRole pr) return;
        var recoverMana = user.CurrMantra * 3 + (int)user.MagicAbility;
        var overflow = recoverMana - user.CurrMpLimit + user.CurrMagic;
        if (overflow <= 0) return;
        user.CurrMagic += recoverMana;
        pr.CurrMBlock += (int)Math.Floor(overflow / 3f);
    }
}

internal partial class NoAttack : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.FunctionUse = (user, _, _) =>
        {
            var bs = StaticUtils.TryGetBattleScene();
            if (bs == null) return;
            if (!BattleStatic.IsFighting) return;
            if (user is not PlayerRole pr) return;
            List<Card> cards = [];
            cards.AddRange(pr.InFightHands.Where(card => (card.FilterFlags & (ulong)StaticEnums.CardFlag.Attack) != 0));

            bs.DisCard(cards, pr, Pages.BattleScene.DisCardReason.Effect, user);
            pr.CurrPBlock += cards.Count * 3;
            pr.CurrMBlock += cards.Count * 3;
        };
    }
}

internal partial class NoDefense : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.UseFilter = (_, targets, _) => targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
        c.FunctionUse = (user, targets, _) =>
        {
            if (StaticInstance.CurrWindow is not Pages.BattleScene bs) return;
            if (!BattleStatic.IsFighting) return;
            if (user is not PlayerRole pr) return;
            List<Card> cards = [];
            cards.AddRange(pr.InFightHands.Where(card => (card.FilterFlags & (ulong)StaticEnums.CardFlag.Armor) != 0));

            bs.DisCard(cards, pr, Pages.BattleScene.DisCardReason.Effect, user);
            bs.DealDamage(4, StaticEnums.AttackType.Physics, user, targets, StaticEnums.AttributeEnum.None,
                cards.Count);
        };
    }
}

internal partial class Punch : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.UseFilter = (_, targets, _) => targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
        c.FunctionUse = (user, targets, _) =>
        {
            if (StaticInstance.CurrWindow is not Pages.BattleScene bs) return;
            if (BattleStatic.IsFighting)
            {
                bs.DealDamage(5, StaticEnums.AttackType.Physics, user, targets);
            }
        };
    }
}

internal partial class RockDrill : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.UseFilter = (_, targets, _) => targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
        c.FunctionUse = (user, targets, _) =>
        {
            if (StaticInstance.CurrWindow is not Pages.BattleScene bs) return;
            if (!BattleStatic.IsFighting) return;
            if (targets[0] is EnemyRole { CurrPBlock: > 0 })
            {
                bs.DealDamage(8, StaticEnums.AttackType.Physics, user, targets,
                    StaticEnums.AttributeEnum.Earth);
            }

            bs.DealDamage(4, StaticEnums.AttackType.Magic, user, targets, StaticEnums.AttributeEnum.Earth);
        };
    }
}

internal partial class SelfMblock : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.FunctionUse = (_, targets, _) =>
        {
            foreach (var target in targets)
            {
                if (target is PlayerRole ifp)
                {
                    ifp.CurrMBlock += 5;
                }
            }
        };
    }
}

internal partial class SelfPblock : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.FunctionUse = (_, targets, _) =>
        {
            foreach (var target in targets)
            {
                if (target is PlayerRole ifp)
                {
                    ifp.CurrPBlock += 5;
                }
            }
        };
    }
}

internal partial class ShadowShot : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.UseFilter = (_, targets, _) => targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
        c.FunctionUse = (user, targets, _) =>
        {
            if (StaticInstance.CurrWindow is not Pages.BattleScene bs) return;
            if (BattleStatic.IsFighting)
            {
                bs.DealDamage(3 + user.CurrEffeciency * 2, StaticEnums.AttackType.Magic, user, targets,
                    StaticEnums.AttributeEnum.Dark);
            }
        };
    }
}

internal partial class Telekinesis : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.UseFilter = (_, _, _) =>
        {
            StaticUtils.CreateBuffAndAddToRole("telekinesis", c.Owner, c.Owner);
            return true;
        };
    }
}

internal partial class WaterSlash : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.UseFilter = (_, targets, _) => targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
        c.FunctionUse = (user, targets, _) =>
        {
            if (StaticInstance.CurrWindow is not Pages.BattleScene bs) return;
            if (BattleStatic.IsFighting)
            {
                bs.DealDamage(7, StaticEnums.AttackType.Magic, user, targets, StaticEnums.AttributeEnum.Water);
            }
        };
    }
}