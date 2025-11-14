using System;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using KemoCard.Scripts.Cards;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs;

public partial class Angry : BaseBuffScript
{
    public override void OnBuffInit(BuffImplBase b)
    {
        b.AddEvent("NewTurn", InitFunc);
    }

    private static void InitFunc(BuffImplBase b, dynamic datas)
    {
        if (datas == null) return;
        if (datas[0] <= 1) return;
        if (b.Binder is BaseRole binder)
        {
            binder.AddSymbolValue("StrengthAddProp", 3);
        }
    }
}

public partial class BleedWolf : BaseBuffScript
{
    public override void OnBuffInit(BuffImplBase buff)
    {
        buff.AddEvent("NewTurn", InitFunc);
    }

    private static void InitFunc(BuffImplBase buff, dynamic datas)
    {
        if (StaticUtils.TryGetBattleScene() is not { } bs) return;
        if (buff.Creator is BaseRole creator && buff.Binder is BaseRole binder)
            bs.DealDamage(buff.BuffValue, StaticEnums.AttackType.Physics, creator, [binder]);
    }
}

public partial class FireInjury : BaseBuffScript
{
    public override void OnBuffInit(BuffImplBase b)
    {
        b.AddEvent("BeforeDealDamageSingle", InitFunc);
    }

    private static void InitFunc(BuffImplBase b, dynamic datas)
    {
        if (datas == null) return;
        if (datas is not Damage d) return;
        if (d.targets == null || d.targets[0] != b.Binder || d.atrribute != StaticEnums.AttributeEnum.Fire) return;
        GD.Print("Buff fire_injury通过条件检测,修改前的值：" + d.value);
        d.value *= 1.5;
        GD.Print("Buff fire_injury通过条件检测,修改后的值：" + d.value);
    }
}

public partial class FireResis : BaseBuffScript
{
    public override void OnBuffInit(BuffImplBase b)
    {
        b.AddEvent("BeforeDealDamageSingle", DealDamageFunc);
    }

    private static void DealDamageFunc(BuffImplBase b, dynamic datas)
    {
        if (datas is not Damage d) return;
        if (d.targets == null || d.targets[0] != b.Binder || d.atrribute != StaticEnums.AttributeEnum.Fire) return;
        GD.Print("Buff fire_resis通过条件检测,修改前的值：" + d.value);
        d.value *= 0.75;
        GD.Print("Buff fire_resis通过条件检测,修改后的值：" + d.value);
    }
}

public partial class GetLucky : BaseBuffScript
{
    public override void OnBuffInit(BuffImplBase b)
    {
        b.AddEvent("StartBattle", StartBattleFunc);
    }

    private static void StartBattleFunc(BuffImplBase b, dynamic datas)
    {
        if (StaticInstance.CurrWindow is not Pages.BattleScene bs) return;
        if (!BattleStatic.isFighting) return;
        if (b.Binder is not PlayerRole inFightPlayer) return;
        for (var i = 0; i < 2; i++)
        {
            Card card = new("lucky")
            {
                Owner = inFightPlayer
            };
            inFightPlayer.InFightHands.Add(card);
            if (inFightPlayer != bs.NowPlayer) continue;
            var cardObject = ResourceLoader.Load<PackedScene>("res://Pages/CardObject.tscn")
                .Instantiate<CardObject>();
            cardObject.InitData(card);
            bs.HandControl.AddChild(cardObject);
        }
    }
}

public partial class GhostBody : BaseBuffScript
{
    public override void OnBuffInit(BuffImplBase buff)
    {
        buff.AddEvent("BeforeDealDamageSingle", DealDamageFunc);
    }

    private static void DealDamageFunc(BuffImplBase b, dynamic datas)
    {
        if (datas[0] is not Damage damage) return;
        if (damage.attackType == StaticEnums.AttackType.Physics)
        {
            damage.value /= 2;
        }
    }
}

public partial class Telekinesis : BaseBuffScript
{
    public override void OnBuffInit(BuffImplBase buff)
    {
        buff.AddEvent("AfterAttacked", AfterAttackedFunc);
    }

    private static void AfterAttackedFunc(BuffImplBase buff, dynamic datas)
    {
        if (datas[0] == null) return;
        if (datas[0] is not Damage damage) return;
        if (!damage.targets.Contains(buff.Binder as BaseRole)) return;
        if (buff.Binder is PlayerRole pr)
        {
            pr.CurrMBlock += (int)Math.Ceiling(damage.value / 2);
        }
    }
}

internal partial class WaterInjury : BaseBuffScript
{
    public override void OnBuffInit(BuffImplBase b)
    {
        b.AddEvent("BeforeDealDamageSingle", DealDamageFunc);
    }

    private static void DealDamageFunc(BuffImplBase b, dynamic datas)
    {
        if (datas == null) return;
        if (datas is not Damage d) return;
        if (d.targets == null || d.targets[0] != b.Binder || d.atrribute != StaticEnums.AttributeEnum.Water) return;
        GD.Print("Buff water_injury通过条件检测,修改前的值：" + d.value);
        d.value *= 1.5;
        GD.Print("Buff water_injury通过条件检测,修改后的值：" + d.value);
    }
}

internal partial class WaterResis : BaseBuffScript
{
    public override void OnBuffInit(BuffImplBase b)
    {
        b.AddEvent("BeforeDealDamageSingle", DealDamageFunc);
    }

    private static void DealDamageFunc(BuffImplBase b, dynamic datas)
    {
        if (datas == null) return;
        if (datas is not Damage d) return;
        if (d.targets is null || d.targets[0] != b.Binder ||
            d.atrribute != StaticEnums.AttributeEnum.Water) return;
        GD.Print("Buff water_resis通过条件检测,修改前的值：" + d.value);
        d.value *= 0.75;
        GD.Print("Buff water_resis通过条件检测,修改后的值：" + d.value);
    }
}