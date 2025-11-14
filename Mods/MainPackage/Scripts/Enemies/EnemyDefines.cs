using System;
using System.Collections.Generic;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Enemies;

namespace KemoCard.Mods.MainPackage.Scripts.Enemies;

public partial class Bat : BaseEnemyScript
{
    public override void OnEnemyInit(EnemyImplBase e)
    {
        // 参考怪，数值模型为21+3*阶级点总点数,boss为30+3*阶级点总点数
        e.Speed = 9;
        e.Strength = 4;
        e.Dodge = 6;
        e.Critical = 5;
        e.Name = "蝙蝠";
        e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
        e.ActionFunc = ActioinFunc;
        e.AddEvent("StartBattle", StartBattleFunc);
    }

    private static void ActioinFunc(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase e)
    {
        if (players.Count == 0) return;
        var rand = new Random();
        var idx = rand.Next(players.Count);
        List<BaseRole> list = [players[idx]];
        if (StaticUtils.TryGetBattleScene() is { } bs)
        {
            bs.DealDamage(e.Binder.CurrStrength + e.Binder.Body * 0.5, StaticEnums.AttackType.Physics, e.Binder,
                list, StaticEnums.AttributeEnum.Wind);
        }
    }

    private static void StartBattleFunc(EnemyImplBase e, dynamic datas)
    {
        var intent =
            $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
            $"{e.Binder.CurrStrength + e.Binder.Body * 0.5:N2}点风属性物理伤害(" +
            $"{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor)}+0.5*" +
            $"{StaticUtils.MakeColorString("身体", StaticInstance.BodyColor)})";
        e.ChangeIntent(intent);
    }
}

public partial class FireBat : BaseEnemyScript
{
    public override void OnEnemyInit(EnemyImplBase e)
    {
        e.Speed = 9;
        e.CraftBook = 0;
        e.CraftEquip = 0;
        e.Strength = 4;
        e.Dodge = 6;
        e.Critical = 5;
        e.Mantra = 0;
        e.Effeciency = 0;
        e.Name = "火蝙蝠";
        e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
        e.ActionFunc = ActionFunc;
        e.AddEvent("StartBattle", StartBattleFunc);
        StaticUtils.CreateBuffAndAddToRole("water_injury", e.Binder, e.Binder);
        StaticUtils.CreateBuffAndAddToRole("fire_resis", e.Binder, e.Binder);
    }

    private static void ActionFunc(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase e)
    {
        if (players.Count == 0) return;
        var rand = new Random();
        var idx = rand.Next(players.Count);
        List<BaseRole> list = [players[idx]];
        if (StaticInstance.CurrWindow is Pages.BattleScene bs)
        {
            bs.DealDamage(e.Binder.CurrStrength + e.Binder.Body * 0.5, StaticEnums.AttackType.Physics, e.Binder, list,
                StaticEnums.AttributeEnum.Fire);
        }
    }

    private static void StartBattleFunc(EnemyImplBase e, dynamic datas)
    {
        var intent =
            $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
            $"{e.Binder.CurrStrength + e.Binder.Body * 0.5:N2}点火属性物理伤害(" +
            $"{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor)}+0.5*" +
            $"{StaticUtils.MakeColorString("身体", StaticInstance.BodyColor)})";
        e.ChangeIntent(intent);
    }
}

public partial class Frog : BaseEnemyScript
{
    private EnemyImplBase _data;

    public override void OnEnemyInit(EnemyImplBase enemy)
    {
        _data = enemy;
        enemy.Speed = 6;
        enemy.Strength = 3;
        enemy.CraftBook = 0;
        enemy.CraftEquip = 0;
        enemy.Dodge = 10;
        enemy.Critical = 5;
        enemy.Effeciency = 10;
        enemy.Mantra = 5;
        enemy.Name = "青蛙";
        enemy.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
        enemy.ActionFunc = EfrogAction;
        SetAction(enemy, null);
        enemy.AddEvent("NewTurn", SetAction);
        enemy.AddEvent("StartBattle", SetAction);
        enemy.AddEvent("PropertiesChanged", UpdateIntent);
        StaticUtils.CreateBuffAndAddToRole("water_resis", enemy.Binder, enemy.Binder);
    }

    private void SetAction(EnemyImplBase e, dynamic datas)
    {
        Random r = new();
        if (r.Next(0, 10) < 5)
        {
            _data.InGameDict["action"] = 1;
        }
        else
        {
            _data.InGameDict["action"] = 2;
        }

        UpdateIntent(e);
    }

    private void UpdateIntent(EnemyImplBase e, dynamic datas = null)
    {
        if (_data == null || !_data.InGameDict.TryGetValue("action", out var value)) return;
        var intent = "";
        switch (value)
        {
            case 1:
                intent =
                    $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                    $"{_data.Binder.CurrEffeciency + _data.Binder.MagicAbility * 0.5:N2}点无属性魔法伤害(" +
                    $"{StaticUtils.MakeColorString("效率", StaticInstance.MagicColor)}+0.5*" +
                    $"{StaticUtils.MakeColorString("魔法", StaticInstance.MagicColor)})";
                _data.ChangeIntent(intent);
                break;
            case 2:
                intent =
                    $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]消耗一半的当前法力值，造成" +
                    $"等量无属性魔法伤害({_data.Binder.CurrMagic * 0.5:N2}," +
                    $"0.5*{StaticUtils.MakeColorString("法力值", StaticInstance.MagicColor)})";
                _data.ChangeIntent($"");
                break;
        }

        _data.ChangeIntent(intent);
    }

    private void EfrogAction(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
    {
        if (!@base.InGameDict.TryGetValue("action", out var value) || players.Count == 0) return;
        if (StaticUtils.TryGetBattleScene() is not { } bs) return;
        if (players.Count == 0) return;
        Random r = new();
        List<BaseRole> targets = [players[r.Next(players.Count)]];
        switch (value)
        {
            case 1:
                bs.DealDamage(_data.Binder.CurrEffeciency + _data.Binder.MagicAbility * 0.5,
                    StaticEnums.AttackType.Magic, _data.Binder, targets);
                break;
            case 2:
                bs.DealDamage(_data.Binder.CurrMagic * 0.5, StaticEnums.AttackType.Magic, _data.Binder, targets);
                _data.Binder.CurrMagic /= 2;
                break;
        }
    }
}

public partial class Ghost : BaseEnemyScript
{
    private EnemyImplBase _data;

    public override void OnEnemyInit(EnemyImplBase e)
    {
        _data = e;
        e.Speed = 6;
        e.Strength = 1;
        e.Dodge = 10;
        e.Effeciency = 6;
        e.Mantra = 4;
        e.Binder.CurrMBlock = 15;
        e.Name = "鬼魂";
        e.ActionFunc = ActionFunc;
        e.AddEvent("StartBattle", UpdateIntent);
        StaticUtils.CreateBuffAndAddToRole("ghost_body", e.Binder, e.Binder);
    }

    private void ActionFunc(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
    {
        if (players.Count == 0) return;
        var rand = new Random();
        var idx = rand.Next(players.Count);
        List<BaseRole> list = [players[idx]];
        if (StaticUtils.TryGetBattleScene() is { } bs)
        {
            bs.DealDamage(12, StaticEnums.AttackType.Magic, _data.Binder, list, StaticEnums.AttributeEnum.Dark);
        }
    }

    private void UpdateIntent(EnemyImplBase e, dynamic dynamic)
    {
        const string intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                              $"12点暗属性魔法伤害";
        _data.ChangeIntent(intent);
    }
}

public partial class Giant : BaseEnemyScript
{
    private EnemyImplBase _data;

    public override void OnEnemyInit(EnemyImplBase e)
    {
        _data = e;
        e.Speed = 3;
        e.Strength = 20;
        e.Critical = 13;
        e.Binder.CurrPBlock = 20;
        e.Binder.CurrMBlock = 20;
        e.Name = "巨人";
        e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
        e.ActionFunc = ActionFunc;
        e.AddEvent("StartBattle", UpdateIntent);
    }

    private void UpdateIntent(EnemyImplBase e, dynamic dynamic)
    {
        const string intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]行动时," +
                              $"若护甲低于魔甲，则将护甲补充至20，然后对随机一名敌人造成10点无属性物理伤害。" +
                              $"否则，将魔甲补充至20，然后对随机一名敌人造成10点无属性魔法伤害";
        _data.ChangeIntent(intent);
    }

    private static void ActionFunc(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
    {
        if (players.Count == 0) return;
        var rand = new Random();
        var idx = rand.Next(players.Count);
        List<BaseRole> list = [players[idx]];
        if (StaticUtils.TryGetBattleScene() is not { } bs) return;
        if (@base.Binder.CurrPBlock < @base.Binder.CurrMBlock)
        {
            @base.Binder.CurrPBlock = 20;
            bs.DealDamage(10, StaticEnums.AttackType.Physics, @base.Binder, list);
        }
        else
        {
            @base.Binder.CurrMBlock = 20;
            bs.DealDamage(10, StaticEnums.AttackType.Magic, @base.Binder, list);
        }
    }
}

public partial class Goblin : BaseEnemyScript
{
    private EnemyImplBase _data;

    public override void OnEnemyInit(EnemyImplBase e)
    {
        _data = e;
        e.Speed = 3;
        e.Strength = 7;
        e.CraftBook = 7;
        e.CraftEquip = 7;
        e.Dodge = 0;
        e.Critical = 0;
        e.Mantra = 0;
        e.Effeciency = 0;
        e.Name = "哥布林";
        e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
        e.ActionFunc = ActionFunc;
        e.AddEvent("NewTurn", UpdateIntent);
    }

    private void UpdateIntent(EnemyImplBase e, dynamic dynamic)
    {
        if (dynamic == null || dynamic[0] is not int || _data == null) return;
        string intent;
        if (dynamic[0] % 2 == 0)
        {
            intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                     $"9 + (哥布林与目标的工艺属性的差值)点土属性物理伤害";
        }
        else
        {
            intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                     $"9 + (哥布林与目标的书写属性的差值)点土属性魔法伤害";
        }

        _data.ChangeIntent(intent);
    }

    private static void ActionFunc(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
    {
        if (players.Count == 0) return;
        Random r = new();
        var idx = r.Next(players.Count);
        var target = players[idx];
        List<BaseRole> targets = [target];
        if (StaticUtils.TryGetBattleScene() is not { } bs) return;
        if (round % 2 == 0)
        {
            bs.DealDamage(9 + @base.Binder.CurrCraftEquip - target.CurrCraftEquip, StaticEnums.AttackType.Physics,
                @base.Binder, targets, StaticEnums.AttributeEnum.Earth);
        }
        else
        {
            bs.DealDamage(9 + @base.Binder.CurrCraftBook - target.CurrCraftBook, StaticEnums.AttackType.Magic,
                @base.Binder, targets, StaticEnums.AttributeEnum.Earth);
        }
    }
}

public partial class Skeleton : BaseEnemyScript
{
    private EnemyImplBase _data;

    public override void OnEnemyInit(EnemyImplBase e)
    {
        _data = e;
        e.Speed = 2;
        e.Strength = 5;
        e.Critical = 17;
        e.Name = "骷髅";
        e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
        e.ActionFunc = ActionFunc;
        e.AddEvent("StartBattle", UpdateIntent);
        StaticUtils.CreateBuffAndAddToRole("", e.Binder, e.Binder);
    }

    private void ActionFunc(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
    {
        if (players.Count == 0) return;
        var rand = new Random();
        var idx = rand.Next(players.Count);
        List<BaseRole> list = [players[idx]];
        if (StaticUtils.TryGetBattleScene() is { } bs)
        {
            bs.DealDamage(_data.Binder.CurrStrength + 5, StaticEnums.AttackType.Physics, _data.Binder, list,
                StaticEnums.AttributeEnum.Dark);
        }
    }

    private void UpdateIntent(EnemyImplBase e, dynamic datas)
    {
        var intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                     $"{5 + _data.Binder.CurrStrength}点暗属性物理伤害(" +
                     $"{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor)}+5)";
        _data.ChangeIntent(intent);
    }
}

public partial class Slime : BaseEnemyScript
{
    private EnemyImplBase _data;

    public override void OnEnemyInit(EnemyImplBase e)
    {
        _data = e;
        e.Speed = 1;
        e.Strength = 15;
        e.Mantra = 8;
        e.Name = "史莱姆";
        e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
        e.ActionFunc = ActionFunc;
        e.AddEvent("StartBattle", UpdateIntent);
    }

    private static void ActionFunc(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
    {
        if (players.Count == 0) return;
        var rand = new Random();
        var idx = rand.Next(players.Count);
        var target = players[idx];
        List<BaseRole> list = [target];
        if (StaticUtils.TryGetBattleScene() is not { } bs) return;
        var value = @base.Binder.CurrMagic / 2;
        var diff = value - target.CurrMagic;
        if (diff > 0)
        {
            bs.DealDamage(diff, StaticEnums.AttackType.Magic, @base.Binder, list, StaticEnums.AttributeEnum.Water);
            target.CurrMagic = 0;
        }
        else
        {
            target.CurrMagic -= value;
        }
    }

    private void UpdateIntent(EnemyImplBase e, dynamic datas)
    {
        const string intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]以随机一名敌人为目标," +
                              $"吸取其相当于史莱姆一半法力值上限的法力,溢出的数值将转化为水属性魔法伤害攻击目标。";
        _data.ChangeIntent(intent);
    }
}

public partial class SmallGnome : BaseEnemyScript
{
    private EnemyImplBase _data;

    public override void OnEnemyInit(EnemyImplBase e)
    {
        _data = e;
        e.Effeciency = 10;
        e.Mantra = 5;
        e.Speed = 3;
        e.Strength = 6;
        e.CraftBook = 3;
        e.ActionFunc = ActionFunc;
        e.AddEvent("StartBattle", UpdateIntent);
    }

    private void UpdateIntent(EnemyImplBase e, dynamic dynamic)
    {
        var intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                     $"{_data.Binder.CurrEffeciency + _data.Binder.MagicAbility * 0.5:N2}点无属性魔法伤害(" +
                     $"{StaticUtils.MakeColorString("效率", StaticInstance.MagicColor)}+0.5*" +
                     $"{StaticUtils.MakeColorString("魔法", StaticInstance.MagicColor)})";
        _data.ChangeIntent(intent);
    }

    private void ActionFunc(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
    {
        if (players.Count == 0) return;
        var rand = new Random();
        var idx = rand.Next(players.Count);
        List<BaseRole> list = [players[idx]];
        if (StaticUtils.TryGetBattleScene() is { } bs)
        {
            bs.DealDamage(_data.Binder.CurrEffeciency + _data.Binder.MagicAbility * 0.5, StaticEnums.AttackType.Magic,
                _data.Binder, list);
        }
    }
}

public partial class Troll : BaseEnemyScript
{
    public override void OnEnemyInit(EnemyImplBase e)
    {
        e.Strength = 9;
        e.CraftBook = 0;
        e.CraftEquip = 0;
        e.Dodge = 2;
        e.Critical = 25;
        e.Speed = 3;
        e.Mantra = 0;
        e.Effeciency = 0;
        e.Name = "巨魔";
        e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";

        e.ActionFunc = Action;
        e.AddEvent("RepositionHand", RepositionHandFunc);
        e.AddEvent("StartBattle", StartBattleFunc);
        StaticUtils.CreateBuffAndAddToRole("fire_injury", e.Binder, e.Binder);
        StaticUtils.CreateBuffAndAddToRole("water_injury", e.Binder, e.Binder);
    }

    private static void Action(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
    {
        if (!@base.InGameDict.TryGetValue("action", out var tag)) return;
        switch (tag)
        {
            case 1:
                StaticUtils.CreateBuffAndAddToRole("angry", @base.Binder, @base.Binder);
                break;
            case 2 when players.Count == 0:
                return;
            case 2:
            {
                var rand = new Random();
                var idx = rand.Next(players.Count);
                List<BaseRole> list = [players[idx]];
                if (StaticInstance.CurrWindow is Pages.BattleScene bs)
                {
                    bs.DealDamage(
                        @base.Binder.CurrStrength + @base.Binder.Body * 0.5 +
                        StaticInstance.PlayerData.Gsd.MajorRole.InFightHands.Count * 3, StaticEnums.AttackType.Physics,
                        @base.Binder, list);
                }

                break;
            }
        }
    }

    private static void RepositionHandFunc(EnemyImplBase e, dynamic datas)
    {
        if (StaticInstance.CurrWindow is Pages.BattleScene { Turns: <= 1 }) return;

        var intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                     $"{e.Binder.CurrStrength + e.Binder.Body * 0.5 + StaticInstance.PlayerData.Gsd.MajorRole.InFightHands.Count * 3:N2}点无属性物理伤害(" +
                     $"{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor)}+0.5*" +
                     $"{StaticUtils.MakeColorString("身体", StaticInstance.BodyColor)}+3*{StaticUtils.MakeColorString("手牌", "#f70101")})";
        e.ChangeIntent(intent);
        e.InGameDict["action"] = 2;
    }

    private static void StartBattleFunc(EnemyImplBase e, dynamic datas)
    {
        var intent =
            $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_031.png[/img]获得Buff-愤怒(每回合提高自身3点{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor)})";
        e.ChangeIntent(intent);
        //StaticUtils.CreateBuffAndAddToRole("angry", e.Binder);
        e.InGameDict["action"] = 1;
    }
}

public partial class WaterBat : BaseEnemyScript
{
    private EnemyImplBase _data;

    public override void OnEnemyInit(EnemyImplBase e)
    {
        _data = e;
        e.Speed = 9;
        e.CraftBook = 0;
        e.CraftEquip = 0;
        e.Strength = 4;
        e.Dodge = 6;
        e.Critical = 5;
        e.Mantra = 0;
        e.Effeciency = 0;
        e.Name = "水蝙蝠";
        e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";


        e.ActionFunc = ActionFunc;
        e.AddEvent("StartBattle", UpdateIntent);
        e.AddEvent("PropertiesChanged", UpdateIntent);
        StaticUtils.CreateBuffAndAddToRole("fire_injury", e.Binder, e.Binder);
        StaticUtils.CreateBuffAndAddToRole("water_resis", e.Binder, e.Binder);
    }

    private void UpdateIntent(EnemyImplBase e, dynamic datas = null)
    {
        var intent =
            $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
            $"{_data.Binder.CurrStrength + _data.Binder.Body * 0.5:N2}点水属性物理伤害(" +
            $"{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor)}+0.5*" +
            $"{StaticUtils.MakeColorString("身体", StaticInstance.BodyColor)})";
        _data.ChangeIntent(intent);
    }

    private static void ActionFunc(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
    {
        if (players.Count == 0) return;
        var rand = new Random();
        var idx = rand.Next(players.Count);
        List<BaseRole> list = [players[idx]];
        if (StaticInstance.CurrWindow is Pages.BattleScene bs)
        {
            bs.DealDamage(@base.Binder.CurrStrength + @base.Binder.Body * 0.5, StaticEnums.AttackType.Physics,
                @base.Binder, list,
                StaticEnums.AttributeEnum.Water);
        }
    }
}

public partial class Wolf : BaseEnemyScript
{
    private EnemyImplBase _data;

    public override void OnEnemyInit(EnemyImplBase enemy)
    {
        _data = enemy;
        enemy.Speed = 12;
        enemy.Strength = 12;
        enemy.CraftBook = 0;
        enemy.CraftEquip = 0;
        enemy.Dodge = 5;
        enemy.Critical = 7;
        enemy.Mantra = 0;
        enemy.Effeciency = 0;
        enemy.Name = "狼";
        enemy.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
        enemy.ActionFunc = ActionFunction;
        if (BattleStatic.isFighting)
        {
            SetAction(enemy, null);
        }

        enemy.AddEvent("NewTurn", SetAction);
        enemy.AddEvent("StartBattle", SetAction);
    }

    private void SetAction(EnemyImplBase e, dynamic datas)
    {
        Random r = new();
        if (!BattleStatic.GameTags.ContainsKey("wolf_call_helper") && r.Next(0, 100) < 25)
        {
            _data.InGameDict["action"] = 1;
        }
        else
        {
            if (r.Next(0, 100) < 50)
            {
                _data.InGameDict["action"] = 2;
            }
            else
            {
                _data.InGameDict["action"] = 3;
            }
        }

        UpdateIntent();
    }

    private void UpdateIntent()
    {
        var intent = _data.InGameDict["action"] switch
        {
            1 => $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_176.png[/img]召唤一匹狼加入战斗",
            2 =>
                $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]攻击一名随机玩家。造成{_data.Binder.CurrStrength}点无属性物理伤害(1*{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor)})。如果此伤害暴击，则给目标施加一层流血效果(每回合开始时受到0.5*附加此效果时的力量点无属性物理伤害)。",
            3 =>
                $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]攻击一名随机玩家。造成{_data.Binder.CurrStrength}点无属性物理伤害(1*{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor)})。目标每有1点护甲增加1点伤害，最多增加10点。",
            _ => ""
        };

        _data.ChangeIntent(intent);
    }

    private void ActionFunction(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
    {
        if (StaticUtils.TryGetBattleScene() is not { } bs) return;
        Random r = new();
        switch (@base.InGameDict["action"])
        {
            case 0:
                break;
            case 1:
                BattleStatic.GameTags.TryAdd("wolf_call_helper", 1);
                EnemyRole helper = new("wolf");
                var res = ResourceLoader.Load<PackedScene>("res://Pages/EnemyRoleObject.tscn");
                if (res != null)
                {
                    bs.CurrentEnemyRoles.Add(helper);
                    EnemyRoleObject ero = res.Instantiate<EnemyRoleObject>();
                    ero.Init(helper);
                    helper.Script.InGameDict["action"] = 0;
                    bs.EnemyContainer.AddChild(ero);
                }

                break;
            case 2:
                List<BaseRole> targets = [players[r.Next(0, players.Count)]];
                bs.DealDamage(@base.Strength, StaticEnums.AttackType.Physics, @base.Binder, targets,
                    StaticEnums.AttributeEnum.None, 1, 0, 0, null, CriticalAction);
                break;
            case 3:
                var t = players[r.Next(0, players.Count)];
                List<BaseRole> targets2 = [t];
                bs.DealDamage(@base.Strength + (t.CurrPBlock > 10 ? 10 : t.CurrPBlock),
                    StaticEnums.AttackType.Physics, @base.Binder, targets2);
                break;
        }
    }

    private void CriticalAction(Damage damage, BaseRole currTarget)
    {
        BuffImplBase buff = new("bleed_wolf")
        {
            Creator = damage.from,
            BuffValue = (int)Math.Floor(damage.from.CurrStrength * 0.5)
        };
        buff.Desc = $"每回合开始时，受到{buff.BuffValue}点无属性物理伤害";
        currTarget.AddBuff(buff);
        if (StaticUtils.TryGetBattleScene() is not { } bs) return;
        if (damage.from is not EnemyRole wolf) return;
        if (bs.FindEnemyObjectByRole(wolf) is { } ero)
        {
            ero.AddBuff(buff);
        }
    }
}

public partial class Zombie : BaseEnemyScript
{
    private EnemyImplBase _data;

    public override void OnEnemyInit(EnemyImplBase enemy)
    {
        _data = enemy;
        enemy.Speed = 6;
        enemy.Strength = 24;
        enemy.Critical = 6;
        enemy.Name = "丧尸";
        enemy.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
        enemy.ActionFunc = ZombieAction;
        enemy.AddEvent("StartBattle", UpdateIntent);
    }

    private void UpdateIntent(EnemyImplBase e, dynamic dynamic)
    {
        var intent =
            $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
            $"{_data.Binder.CurrStrength:N2}点物理伤害(" +
            $"{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor)})，将造成的伤害值转化为生命值治疗丧尸";
        _data.ChangeIntent(intent);
    }

    private static void ZombieAction(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
    {
        var bs = StaticUtils.TryGetBattleScene();
        if (bs == null) return;
        if (players.Count == 0) return;
        Random r = new();
        var index = r.Next(players.Count);
        List<BaseRole> list = [players[index]];
        var oldHealth = list[0].CurrHealth;
        bs.DealDamage(@base.Binder.CurrStrength, StaticEnums.AttackType.Physics, @base.Binder, list);
        if (oldHealth > list[0].CurrHealth)
        {
            @base.Binder.CurrHealth += oldHealth - list[0].CurrHealth;
        }
    }
}