using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

namespace KemoCard.Mods.MainPackage.Scripts.Cards;

internal partial class CreateBook : BaseCardScript
{
    public override void OnCardScriptInit(Card c)
    {
        c.GlobalDict["Exhaust"] = 1;
        c.HintCardIds = ["create_book_attack", "create_book_armor"];
    }

    public override void UseFunction(Card c, BaseRole user, List<BaseRole> targets, params object[] datas)
    {
        Card c1 = new("create_book_attack");
        Card c2 = new("create_book_armor");
        List<Card> cs = [c1, c2];
        var bs = StaticUtils.TryGetBattleScene();
        if (bs == null) return;
        var res = ResourceLoader.Load<PackedScene>("res://Pages/ChoseCardScene.tscn");
        if (res == null) return;
        var scene = res.Instantiate<ChoseCardScene>();
        StaticInstance.WindowMgr.AddScene(scene, cs, 1, 1);
    }
}

internal partial class CreateBookArmor : BaseCardScript
{
    public override void UseFunction(Card c, BaseRole user, List<BaseRole> targets, params object[] datas)
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
    public override void UseFunction(Card c, BaseRole user, List<BaseRole> targets, params object[] datas)
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
    }

    public override void UseFunction(Card c, BaseRole user, List<BaseRole> targets, params object[] datas)
    {
        Card c1 = new("create_equip_attack");
        Card c2 = new("create_equip_armor");
        List<Card> list = [c1, c2];
        var bs = StaticUtils.TryGetBattleScene();
        if (bs == null) return;
        var res = ResourceLoader.Load<PackedScene>("res://Pages/ChoseCardScene.tscn");
        if (res == null) return;
        var scene = res.Instantiate<ChoseCardScene>();
        StaticInstance.WindowMgr.AddScene(scene, list, 1, 1);
    }
}

internal partial class CreateEquipArmor : BaseCardScript
{
    public override void UseFunction(Card c, BaseRole user, List<BaseRole> targets, params object[] datas)
    {
        if (StaticInstance.CurrWindow is BattleScene && user is PlayerRole pr)
        {
            pr.CurrPBlock += 2 + user.CurrCraftEquip;
        }
    }
}

internal partial class CreateEquipAttack : BaseCardScript
{
    public override void UseFunction(Card c, BaseRole user, List<BaseRole> targets, params object[] datas)
    {
        if (StaticInstance.CurrWindow is BattleScene bs)
        {
            bs.DealDamage(5 + user.CurrCraftEquip, StaticEnums.AttackType.Physics, user, targets);
        }
    }
}

internal partial class DoubleHit : BaseCardScript
{
    public override bool UseFilter(Card self, BaseRole owner, List<BaseRole> targets, params dynamic[] datas)
    {
        return false;
    }

    public override void DiscardAction(Card self, BaseRole user, BattleScene.DisCardReason reason, BaseRole from)
    {
        if (reason != BattleScene.DisCardReason.Effect) return;
        if (StaticInstance.CurrWindow is not BattleScene bs) return;
        if (!BattleStatic.IsFighting) return;
        List<BaseRole> tars = [];
        Random r = new();
        tars.Add(bs.CurrentEnemyRoles[r.Next(bs.CurrentEnemyRoles.Count)]);
        bs.DealDamage(14, StaticEnums.AttackType.Physics, user, tars);
    }
}

internal partial class FireBall : BaseCardScript
{
    public override bool UseFilter(Card self, BaseRole owner, List<BaseRole> targets, params dynamic[] datas)
    {
        return targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
    }

    public override void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
    {
        if (StaticInstance.CurrWindow is not BattleScene bs) return;
        if (BattleStatic.IsFighting)
        {
            bs.DealDamage(7, StaticEnums.AttackType.Magic, owner, targets, StaticEnums.AttributeEnum.Fire);
        }
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
    }

    public override bool UseFilter(Card self, BaseRole owner, List<BaseRole> targets, params dynamic[] datas)
    {
        return targets is { Count: > 0 } && targets[0] is PlayerRole { IsFriendly: true };
    }

    public override void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
    {
        if (StaticInstance.CurrWindow is not BattleScene bs) return;
        if (!BattleStatic.IsFighting) return;
        bs.DrawCard(3, (PlayerRole)targets[0]);
        bs.SelectCard(2, 2, null,
            cards => { bs.DisCard(cards, (PlayerRole)targets[0], BattleScene.DisCardReason.Effect, owner); });
    }
}

internal partial class Infinite : BaseCardScript
{
    public override void UseFunction(Card c, BaseRole user, List<BaseRole> targets, params object[] datas)
    {
        if (targets is not { Count: > 0 }) return;
        var bs = StaticUtils.TryGetBattleScene();
        bs?.DealDamage(user.Body + user.MagicAbility + user.Knowlegde + user.Technique,
            StaticEnums.AttackType.Physics, user, targets);
    }
}

internal partial class IronShard : BaseCardScript
{
    public override void UseFunction(Card c, BaseRole user, List<BaseRole> targets, params object[] datas)
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
        c.GlobalDict["Exhaust"] = 1;
        c.GlobalDict["KeepInHand"] = 1;
    }

    public override void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
    {
        if (self.Owner is PlayerRole inFightPlayer)
        {
            inFightPlayer.CurrentActionPoint += 1;
        }
    }
}

internal partial class MagDraw : BaseCardScript
{
    public override bool UseFilter(Card self, BaseRole owner, List<BaseRole> targets, params dynamic[] datas)
    {
        return targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
    }

    public override void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
    {
        if (StaticInstance.CurrWindow is not BattleScene bs) return;
        if (!BattleStatic.IsFighting) return;
        if (targets[0] is not EnemyRole er) return;
        var oldHealth = er.CurrHealth;
        bs.DealDamage(3 + owner.CurrEffeciency, StaticEnums.AttackType.Magic, owner, targets);
        if (er.CurrHealth < oldHealth && owner is PlayerRole pr)
        {
            bs.DrawCard(1, pr);
        }
    }
}

internal partial class MagicMissile : BaseCardScript
{
    public override bool UseFilter(Card self, BaseRole owner, List<BaseRole> targets, params dynamic[] datas)
    {
        return targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
    }

    public override void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
    {
        if (StaticInstance.CurrWindow is not BattleScene bs) return;
        if (BattleStatic.IsFighting)
        {
            bs.DealDamage(5, StaticEnums.AttackType.Physics, owner, targets);
        }
    }
}

internal partial class ManaTide : BaseCardScript
{
    public override void UseFunction(Card self, BaseRole user, List<BaseRole> targets, params object[] datas)
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
    public override void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
    {
        var bs = StaticUtils.TryGetBattleScene();
        if (bs == null) return;
        if (!BattleStatic.IsFighting) return;
        if (owner is not PlayerRole pr) return;
        List<Card> cards = [];
        cards.AddRange(pr.InFightHands.Where(card => (card.FilterFlags & (ulong)StaticEnums.CardFlag.Attack) != 0));

        bs.DisCard(cards, pr, BattleScene.DisCardReason.Effect, owner);
        pr.CurrPBlock += cards.Count * 3;
        pr.CurrMBlock += cards.Count * 3;
    }
}

internal partial class NoDefense : BaseCardScript
{
    public override bool UseFilter(Card self, BaseRole owner, List<BaseRole> targets, params dynamic[] datas)
    {
        return targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
    }

    public override void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
    {
        if (StaticInstance.CurrWindow is not BattleScene bs) return;
        if (!BattleStatic.IsFighting) return;
        if (owner is not PlayerRole pr) return;
        List<Card> cards = [];
        cards.AddRange(pr.InFightHands.Where(card => (card.FilterFlags & (ulong)StaticEnums.CardFlag.Armor) != 0));

        bs.DisCard(cards, pr, BattleScene.DisCardReason.Effect, owner);
        bs.DealDamage(4, StaticEnums.AttackType.Physics, owner, targets, StaticEnums.AttributeEnum.None,
            cards.Count);
    }
}

internal partial class Punch : BaseCardScript
{
    public override bool UseFilter(Card self, BaseRole owner, List<BaseRole> targets, params dynamic[] datas)
    {
        return targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
    }

    public override void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
    {
        if (StaticInstance.CurrWindow is not BattleScene bs) return;
        if (BattleStatic.IsFighting)
        {
            bs.DealDamage(5, StaticEnums.AttackType.Physics, owner, targets);
        }
    }
}

internal partial class RockDrill : BaseCardScript
{
    public override bool UseFilter(Card self, BaseRole owner, List<BaseRole> targets, params dynamic[] datas)
    {
        return targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
    }

    public override void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
    {
        if (StaticInstance.CurrWindow is not BattleScene bs) return;
        if (!BattleStatic.IsFighting) return;
        if (targets[0] is EnemyRole { CurrPBlock: > 0 })
        {
            bs.DealDamage(8, StaticEnums.AttackType.Physics, owner, targets,
                StaticEnums.AttributeEnum.Earth);
        }

        bs.DealDamage(4, StaticEnums.AttackType.Magic, owner, targets, StaticEnums.AttributeEnum.Earth);
    }
}

internal partial class SelfMblock : BaseCardScript
{
    public override void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
    {
        foreach (var target in targets)
        {
            if (target is PlayerRole ifp)
            {
                ifp.CurrMBlock += 5;
            }
        }
    }
}

internal partial class SelfPblock : BaseCardScript
{
    public override void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
    {
        foreach (var target in targets)
        {
            if (target is PlayerRole ifp)
            {
                ifp.CurrPBlock += 5;
            }
        }
    }
}

internal partial class ShadowShot : BaseCardScript
{
    public override bool UseFilter(Card self, BaseRole owner, List<BaseRole> targets, params dynamic[] datas)
    {
        return targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
    }

    public override void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
    {
        if (StaticInstance.CurrWindow is not BattleScene bs) return;
        if (BattleStatic.IsFighting)
        {
            bs.DealDamage(3 + owner.CurrEffeciency * 2, StaticEnums.AttackType.Magic, owner, targets,
                StaticEnums.AttributeEnum.Dark);
        }
    }
}

internal partial class Telekinesis : BaseCardScript
{
    public override void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
    {
        StaticUtils.CreateBuffAndAddToRole("telekinesis", self.Owner, self.Owner);
    }
}

internal partial class WaterSlash : BaseCardScript
{
    public override bool UseFilter(Card self, BaseRole owner, List<BaseRole> targets, params dynamic[] datas)
    {
        return targets is { Count: > 0 } && targets[0] is EnemyRole { IsFriendly: false };
    }

    public override void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
    {
        if (StaticInstance.CurrWindow is not BattleScene bs) return;
        if (BattleStatic.IsFighting)
        {
            bs.DealDamage(7, StaticEnums.AttackType.Magic, owner, targets, StaticEnums.AttributeEnum.Water);
        }
    }
}