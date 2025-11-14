using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using KemoCard.Scripts.Presets;

namespace KemoCard.Pages;

public struct RewardStruct
{
    public RewardType Type;
    public List<string> Rewards;
}

public struct ShopStruct
{
    public Card Card;
    public int Price;
    public bool IsBought;
}

public partial class BattleScene : BaseScene, IEvent
{
    [Export] public HBoxContainer PlayerContainer;
    [Export] public HBoxContainer EnemyContainer;
    [Export] public Control HandControl;
    [Export] private Label _deckCount;
    [Export] public Label GraveCount;
    [Export] private Label _actionPointLabel;
    [Export] private float _tweenSpeed = 0.25f;
    [Export] private float _totalAngle;
    [Export] private float _spaceX = 200f;
    [Export] private float _spaceY = 7f;
    [Export] private Godot.Button _turnEndBtn;
    [Export] public Control DragDownArea;
    [Export] private Godot.Button _debugDrawBtn;
    [Export] private Label _turnLabel;
    [Export] private Godot.Button _deckBtn;
    [Export] private Godot.Button _graveBtn;
    [Export] private Godot.Button _wholeDeckBtn;
    [Export] private Godot.Button _debugWinBtn;
    [Export] private Godot.Button _returnBtn;
    [Export] private Control _endControl;
    [Export] private Godot.Button _selectConfirmBtn;
    [Export] private Control _selectControl;

    private bool _isDebugFight;

    private int _hoveredIndex = -1;
    private bool _isDragging;
    private Preset _battlePreset;
    private Action _afterBattleHandler;

    public PlayerRole NowPlayer;

    public void NewBattle(PlayerRole playerRole, string[] enemyRoles, bool isDebugFight = false,
        Action afterBattleHandler = null)
    {
        //playerRole.Init();
        List<PlayerRole> players = [playerRole];
        List<EnemyRole> enemies = [];
        enemies.AddRange(enemyRoles.Select(role => new EnemyRole(role)));
        BattleStatic.Reset();
        _isDebugFight = isDebugFight;
        NewBattleCore(players, enemies);
        _afterBattleHandler = afterBattleHandler;
    }

    public void NewBattleByPreset(string presetId, bool isDebugFight = false, Action afterBattleHandler = null)
    {
        Preset preset = new();
        if (!Datas.Ins.PresetPool.ContainsKey(presetId)) return;
        var presetScript = PresetFactory.CreatePreset(presetId);
        presetScript.Init(preset);
        _battlePreset = preset;
        NewBattle(StaticInstance.PlayerData.Gsd.MajorRole, preset.Enemies.ToArray(), isDebugFight, afterBattleHandler);
    }

    public override void _Ready()
    {
        _debugWinBtn.Visible = _debugDrawBtn.Visible = OS.IsDebugBuild();
        _debugDrawBtn.Pressed += OnDebugDrawBtnOnPressed;
        _debugWinBtn.Pressed += OnDebugWinBtnOnPressed;
        _turnEndBtn.Pressed += OnTurnEndBtnOnPressed;
        //GD.Print("battleScene Ready");
        _deckBtn.Pressed += OnDeckBtnOnPressed;
        _graveBtn.Pressed += OnGraveBtnOnPressed;
        _wholeDeckBtn.Pressed += OnWholeDeckBtnOnPressed;
        _returnBtn.Pressed += OnReturnBtnOnPressed;
    }

    private void OnReturnBtnOnPressed()
    {
        if (!BattleStatic.isFighting)
        {
            if (_afterBattleHandler != null)
            {
                _afterBattleHandler.Invoke();
                _afterBattleHandler = null;
            }
            else
            {
                //StaticInstance.windowMgr.RemoveScene(this);
                var res = ResourceLoader.Load<PackedScene>("res://Pages/MainScene.tscn");
                if (res != null)
                {
                    var ms = res.Instantiate<MainScene>();
                    StaticInstance.WindowMgr.ChangeScene(ms);
                    ms.MapView?.ShowMap();
                }
            }

            StaticUtils.AutoSave();
        }
    }

    private void OnWholeDeckBtnOnPressed()
    {
        StaticInstance.WindowMgr.AddScene(
            (BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/DeckView.tscn").Instantiate(),
            [CurrentPlayerRoles[0].Deck.ToList()]);
    }

    private void OnGraveBtnOnPressed()
    {
        StaticInstance.WindowMgr.AddScene(
            (BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/DeckView.tscn").Instantiate(),
            [CurrentPlayerRoles[0].InFightGrave.ToList()]);
    }

    private void OnDeckBtnOnPressed()
    {
        var list = CurrentPlayerRoles[0].InFightDeck.ToList();
        StaticUtils.ShuffleArray(list);
        StaticInstance.WindowMgr.AddScene(
            (BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/DeckView.tscn").Instantiate(), [list]);
    }

    private void OnTurnEndBtnOnPressed()
    {
        EndTurn();
    }

    private void OnDebugWinBtnOnPressed()
    {
        EndBattle(true);
    }

    private void OnDebugDrawBtnOnPressed()
    {
        if (NowPlayer != null)
        {
            DrawCard(10, NowPlayer);
        }
    }

    private void UpdateCardPosition()
    {
        if (_isDragging) return;
        if (HandControl == null) return;
        foreach (var i in HandControl.GetChildren().Cast<CardObject>())
        {
            i.StartReposition(CalculateCardPosition(i.GetIndex()), GetCardAngle(i.GetIndex()), _tweenSpeed,
                GetCardScales(i.GetIndex()));
            //GD.Print("序号：" + i.GetIndex() + ",角度：" + GetCardAngle(i.GetIndex()) + ",位置：" + CalculateCardPosition(i.GetIndex()) + ",Pivot：" + i.PivotOffset);
        }
    }

    private Vector2 CalculateCardPosition(int index)
    {
        Vector2 vector = new()
        {
            X = GetCardXPosition(index),
            Y = GetCardYPosition(index)
        };
        return vector;
    }

    private float GetCardAngle(int cardIndex)
    {
        //return 0;
        if (HandControl.GetChildCount() == 0) return 0;
        else
            return (cardIndex - (HandControl.GetChildCount() - 1f) / 2f) * _totalAngle / HandControl.GetChildCount() /
                   360f;
    }

    private float GetCardYPosition(int cardIndex)
    {
        //return 0;
        float hoverShift;
        if (_hoveredIndex == -1)
        {
            hoverShift = 0f;
        }
        else if (cardIndex == _hoveredIndex)
        {
            hoverShift = -50f;
        }
        else
        {
            hoverShift = 50f;
        }

        return Math.Abs(cardIndex - (HandControl.GetChildCount() - 1f) / 2f) * _spaceY + hoverShift;
    }

    private float GetCardXPosition(int cardIndex)
    {
        var hoverShift = 0f;
        if (_hoveredIndex == -1)
        {
            hoverShift = 0f;
        }
        else if (cardIndex < _hoveredIndex)
        {
            hoverShift = -100f;
        }
        else if (cardIndex == _hoveredIndex)
        {
            hoverShift = 0f;
        }
        else if (cardIndex > _hoveredIndex)
        {
            hoverShift = 100f;
        }

        return (cardIndex - (HandControl.GetChildCount() - 1f) / 2f) * _spaceX + HandControl.Size.X / 2f - 128 +
               hoverShift;
    }

    private Vector2 GetCardScales(int cardIndex)
    {
        return _hoveredIndex == cardIndex ? new Vector2(1.2f, 1.2f) : Vector2.One;
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        switch (@event)
        {
            case "RepositionHand":
            {
                if (datas is { Length: > 0 })
                {
                    _hoveredIndex = (int)datas[0];
                }
                else
                {
                    _hoveredIndex = -1;
                }

                UpdateCardPosition();
                break;
            }
            case "DraggingCard":
            {
                if (datas is { Length: > 0 })
                {
                    _isDragging = (bool)datas[0];
                }
                else
                {
                    _isDragging = false;
                }

                UpdateCardPosition();
                break;
            }
            case "Attack":
                DealDamage((int)datas[0], (StaticEnums.AttackType)datas[1], (BaseRole)datas[2],
                    (List<BaseRole>)datas[3]);
                break;
            case "PropertiesChanged":
            {
                if (datas?[0] is PlayerRole ifp && ifp == NowPlayer)
                {
                    if (ifp == NowPlayer)
                    {
                        UpdateCounts();
                    }
                }

                break;
            }
            case "StartSelectTarget":
                DragDownArea.Visible = false;
                break;
            case "EndSelectTarget":
                DragDownArea.Visible = true;
                break;
            case "SelectCard":
                UpdateSelectBtn();
                break;
        }
    }

    public List<PlayerRole> PlayerRoles = [];
    public List<PlayerRole> CurrentPlayerRoles = [];
    public List<PlayerRole> StartPlayerRoles = [];
    public List<EnemyRole> EnemyRoles = [];
    public List<EnemyRole> CurrentEnemyRoles = [];
    public List<EnemyRole> StartEnemyRoles = [];
    public int Turns;

    public StaticEnums.TeamEnum CurrentController;

    public void NewBattleCore(List<PlayerRole> players, List<EnemyRole> enemies)
    {
        BattleStatic.isFighting = true;
        _endControl.Visible = false;
        foreach (var i in players)
        {
            // i.Deck.ForEach(card =>
            // {
            //     var c = new Card(card.Id)
            //     {
            //         Owner = i
            //     };
            //     i.InFightDeck.Add(c);
            // });
            PlayerRoles.Add(i);
            CurrentPlayerRoles.Add(i);
            StartPlayerRoles.Add(i);
            var po = ResourceLoader.Load<PackedScene>("res://Pages/PlayerRoleObject.tscn")
                .Instantiate<PlayerRoleObject>();
            po.InitByPlayerRole(i);
            i.TurnActionPoint = i.CurrentActionPoint = i.ActionPoint;
            i.InitFighter();
            i.OnBattleStart?.Invoke();
            PlayerContainer?.AddChild(po);
        }

        foreach (var i in enemies)
        {
            EnemyRoles.Add(i);
            CurrentEnemyRoles.Add(i);
            StartEnemyRoles.Add(i);
            var eo = ResourceLoader.Load<PackedScene>("res://Pages/EnemyRoleObject.tscn")
                .Instantiate<EnemyRoleObject>();
            StaticInstance.EventMgr.RegisterIEvent(eo);
            eo.Init(i);
            i.OnBattleStart?.Invoke(eo);
            EnemyContainer?.AddChild(eo);
        }

        Turns = 1;
        _turnLabel.Text = "回合数：" + Turns;
        CurrentController = StaticEnums.TeamEnum.Player;
        NowPlayer = CurrentPlayerRoles[0];
        StartBattle();
    }

    private void StartBattle()
    {
        foreach (var i in CurrentPlayerRoles)
        {
            StaticUtils.ShuffleArray(i.InFightDeck);
        }

        foreach (var player in PlayerRoles)
        {
            player.CurrMBlock = (int)Math.Floor(player.OriginMBlock);
            player.CurrPBlock = (int)Math.Floor(player.OriginPBlock);
        }

        foreach (var enemy in EnemyRoles)
        {
            enemy.CurrMBlock = (int)Math.Floor(enemy.OriginMBlock);
            enemy.CurrPBlock = (int)Math.Floor(enemy.OriginPBlock);
        }

        NowPlayer = CurrentPlayerRoles[0];
        foreach (var i in CurrentPlayerRoles)
        {
            DrawCard(5, i);
        }

        var objects = new object[] { CurrentPlayerRoles, CurrentEnemyRoles, Turns };
        StaticInstance.EventMgr.Dispatch("StartBattle", objects);
        UpdateCounts();
        StaticInstance.EventMgr.Dispatch("NewTurn", new dynamic[] { Turns, CurrentPlayerRoles, CurrentEnemyRoles });
    }

    public void NextTurn()
    {
        if (!BattleStatic.isFighting) return;
        Turns++;
        _turnLabel.Text = "回合数：" + Turns;
        foreach (var player in CurrentPlayerRoles)
        {
            player.RecoverMagic();
            player.CurrentActionPoint = player.TurnActionPoint;
        }

        foreach (var enemy in CurrentEnemyRoles)
        {
            enemy.RecoverMagic();
        }

        if (CurrentController == StaticEnums.TeamEnum.Player)
        {
            NowPlayer = CurrentPlayerRoles[0];
            foreach (var i in CurrentPlayerRoles)
            {
                DrawCard(5, i);
            }
        }

        StaticInstance.EventMgr.Dispatch("NewTurn", new dynamic[] { Turns, CurrentPlayerRoles, CurrentEnemyRoles });
        UpdateCounts();
    }

    public void EndTurn()
    {
        foreach (var player in CurrentPlayerRoles)
        {
            var list = player.InFightHands.ToList();
            var discardList = list.Where(c => !c.CheckHasSymbol("KeepInHand")).ToList();
            DisCard(discardList, player, DisCardReason.EndTurn, NowPlayer);
        }

        foreach (var i in HandControl.GetChildren().Cast<CardObject>())
        {
            if (i.card.CheckHasSymbol("KeepInHand")) continue;
            i.QueueFree();
        }

        foreach (var currEnemy in CurrentEnemyRoles)
        {
            if (currEnemy == null || !BattleStatic.isFighting) return;
            currEnemy.Script.ActionFunc?.Invoke(Turns, CurrentPlayerRoles, CurrentEnemyRoles, currEnemy.Script);
        }

        BattleStatic.TurnUsedCard.Clear();
        BattleStatic.TurnTags.Clear();
        NextTurn();
    }

    /// <summary>
    /// 造成一次伤害
    /// </summary>
    /// <param name="value">伤害值</param>
    /// <param name="attackType">攻击类型</param>
    /// <param name="from">伤害来源</param>
    /// <param name="targets">目标列表</param>
    /// <param name="attribute">伤害的属性</param>
    /// <param name="times">伤害次数</param>
    /// <param name="tempAddCri">临时增加的暴击率(固定值)</param>
    /// <param name="tempMulCri">临时增加的暴击率(百分比)</param>
    /// <param name="dodgedAction">闪避时触发</param>
    /// <param name="criticalAction">暴击时触发</param>
    public void DealDamage(double value, StaticEnums.AttackType attackType, BaseRole from = null,
        List<BaseRole> targets = null, StaticEnums.AttributeEnum attribute = StaticEnums.AttributeEnum.None,
        int times = 1, double tempAddCri = 0f, double tempMulCri = 0f, Action<Damage, BaseRole> dodgedAction = null,
        Action<Damage, BaseRole> criticalAction = null)
    {
        //if (!BattleStatic.isFighting) return AttackResult.Failed;
        Damage damage = new()
        {
            validTag = true,
            value = value,
            from = from,
            targets = targets,
            atrribute = attribute,
            attackType = attackType,
            times = times,
            criticaltimes = 0
        };
        StaticInstance.EventMgr.Dispatch("BeforeAttacked", damage);
        StaticInstance.EventMgr.Dispatch("BeforeDealDamage", damage);
        if (damage.validTag && targets != null)
        {
            foreach (var target in targets)
            {
                Damage tempDamage = new()
                {
                    validTag = damage.validTag,
                    value = damage.value,
                    from = damage.from,
                    targets = [target],
                    atrribute = damage.atrribute,
                    attackType = damage.attackType,
                    times = damage.times,
                    criticaltimes = 0
                };
                GD.Print("伤害结算前：" + tempDamage.value);
                StaticInstance.EventMgr.Dispatch("BeforeAttackedSingle", tempDamage);
                if (tempDamage.validTag)
                {
                    if (target.CurrHealth <= 0) continue;
                    tempDamage.value = (int)Math.Round((tempDamage.value +
                                                        target.GetSymbol(
                                                            StaticEnums.AttributeDic[attribute] + "ResisProp") -
                                                        (from?.GetSymbol(
                                                            StaticEnums.AttributeDic[attribute] + "AtkProp") ?? 1f)
                                                        * (1 + target.GetSymbol(
                                                            StaticEnums.AttributeDic[attribute] + "ResisPerm"))
                                                        * (1 + (from?.GetSymbol(
                                                                    StaticEnums.AttributeDic[attribute] + "AtkPerm") ??
                                                                1f))));
                    StaticInstance.EventMgr.Dispatch("BeforeDealDamageSingle", tempDamage);
                    GD.Print("伤害结算后：" + tempDamage.value);
                    for (var i = 0; i < times; i++)
                    {
                        var random = new Random().Next(0, 100);
                        var needValue = target.CurrDodge;
                        GD.Print("闪避摇点：" + random + "，被击者触发闪避要求的点数不大于：" + needValue);
                        if (random <= needValue)
                        {
                            var log = from?.Name + "对" + target.Name + "的攻击被闪避了。闪避摇点：" + random + "，被击者触发闪避要求的点数不大于：" +
                                      needValue;
                            GD.Print(log);
                            StaticInstance.MainRoot.ShowBanner(log);
                            dodgedAction?.Invoke(damage, target);
                            continue;
                            //return AttackResult.Dodged;
                        }

                        random = new Random().Next(0, 100);
                        needValue = (int)Math.Floor(target.CurrCritical * tempMulCri + tempAddCri);
                        GD.Print($"暴击摇点：{random}，触发暴击要求的点数不大于：{needValue}");
                        if (random <= needValue)
                        {
                            var log = from?.Name + "对" + target.Name + "的攻击暴击了。暴击摇点：" + random + "，触发暴击要求的点数不大于：" +
                                      needValue;
                            GD.Print(log);
                            StaticInstance.MainRoot.ShowBanner(log);
                            tempDamage.value *= 2 + (from != null ? (double)from?.GetSymbol("CriticalDamageAdd") : 0f) -
                                                target.GetSymbol("CriticalDamageResis");
                            tempDamage.criticaltimes += 1;
                            damage.criticaltimes += 1;
                            criticalAction?.Invoke(damage, target);
                        }

                        int diff;
                        if (tempDamage.attackType == StaticEnums.AttackType.Physics)
                        {
                            if (target is EnemyRole er)
                                diff = (int)(tempDamage.value - er.CurrPBlock);
                            else diff = (int)(tempDamage.value - (target as PlayerRole).CurrPBlock);
                        }
                        else
                        {
                            if (target is EnemyRole er)
                                diff = (int)(tempDamage.value - er.CurrMBlock);
                            else diff = (int)(tempDamage.value - (target as PlayerRole).CurrMBlock);
                        }

                        if (diff > 0)
                        {
                            if (tempDamage.attackType == StaticEnums.AttackType.Physics)
                            {
                                if (target is EnemyRole er)
                                    er.CurrPBlock = 0;
                                else
                                    (target as PlayerRole).CurrPBlock = 0;
                            }
                            else
                            {
                                if (target is EnemyRole er)
                                    er.CurrMBlock = 0;
                                else
                                    (target as PlayerRole).CurrMBlock = 0;
                            }

                            target.CurrHealth -= diff;
                        }
                        else
                        {
                            if (tempDamage.attackType == StaticEnums.AttackType.Physics)
                            {
                                if (target is EnemyRole er) er.CurrPBlock -= (int)tempDamage.value;
                                else (target as PlayerRole).CurrPBlock -= (int)tempDamage.value;
                            }
                            else
                            {
                                if (target is EnemyRole er) er.CurrMBlock -= (int)tempDamage.value;
                                else (target as PlayerRole).CurrMBlock -= (int)tempDamage.value;
                            }
                        }

                        GD.Print(from?.Name + "对" + target.Name + "造成了" + tempDamage.value + "点伤害");

                        if (target is EnemyRole ter)
                        {
                            FindEnemyObjectByRole(ter)?.hitFlashAnimationPlayer.Play("hit_flash");
                        }

                        if (target.CurrHealth > 0) continue;
                        {
                            GD.Print(target.GetName() + "死了");
                            StaticInstance.EventMgr.Dispatch("BeforeDie", target);
                            if (target.CurrHealth > 0) continue;
                            switch (target)
                            {
                                case PlayerRole ifp:
                                    PlayerRoleDie(ifp);
                                    break;
                                case EnemyRole er:
                                    EnemyRoleDie(er);
                                    break;
                            }
                            //return AttackResult.Success;
                        }
                    }
                }
                else
                {
                    StaticInstance.EventMgr.Dispatch("AttackInvalided", tempDamage);
                    GD.Print(from?.Name + "对" + target.Name + "的攻击被无效了");
                    //return AttackResult.Invalidated;
                }
            }

            StaticInstance.EventMgr.Dispatch("AfterAttacked", damage);
            //return AttackResult.Success;
        }
        else
        {
            StaticInstance.EventMgr.Dispatch("AttackInvalided", damage);
            GD.Print("攻击被无效了");
            //return AttackResult.Invalidated;
        }

        StaticInstance.EventMgr.Dispatch("EndAttacked", damage);
    }

    public void DrawCard(int num, PlayerRole target)
    {
        for (var i = 0; i < num; i++)
        {
            if (target.InFightHands.Count >= target.HandLimit)
            {
                GD.Print("手牌达到上限");
                return;
            }

            if (target.InFightDeck.Count == 0)
            {
                if (target.InFightGraveCount > 0)
                {
                    GD.Print("墓地数量：" + target.InFightGrave.Count);
                    target.ShuffleGrave();
                    target.InFightDeck = target.InFightGrave.ToList();
                    target.InFightGrave.Clear();
                }
            }

            if (target.InFightDeck.Count == 0)
            {
                GD.Print("洗牌后卡组数量为0");
                return;
            }

            var c = target.InFightDeck.First();
            target.InFightDeck.RemoveAt(0);
            target.InFightHands.Add(c);
            if (target != NowPlayer) continue;
            var cardObject = ResourceLoader.Load<PackedScene>("res://Pages/CardObject.tscn")
                .Instantiate<CardObject>();
            cardObject.InitData(c);
            HandControl.AddChild(cardObject);
        }

        GetTree().CreateTimer(0.1f).Timeout += UpdateCardPosition;
        UpdateCounts();
    }

    public void PlayerRoleDie(PlayerRole role)
    {
        if (role.IsFriendly)
        {
            if (CurrentPlayerRoles != null && CurrentPlayerRoles.Contains(role))
            {
                CurrentPlayerRoles.Remove(role);
                foreach (var roleObject in PlayerContainer?.GetChildren().Cast<PlayerRoleObject>() ?? [])
                {
                    if (roleObject.data != role) continue;
                    roleObject.data = null;
                    roleObject.QueueFree();
                }

                GD.Print("友方角色：" + role.Name + "已战败，从当前数组中移除。当前剩余" + CurrentPlayerRoles.Count + "个队友");
            }
        }

        if (CurrentPlayerRoles != null && CurrentPlayerRoles.Count != 0) return;
        StaticInstance.MainRoot.ShowBanner("友方角色已全部战败，游戏结束");
        GD.Print("友方角色已全部战败，游戏结束");
        EndBattle(false);
    }

    public void EnemyRoleDie(EnemyRole role)
    {
        if (!role.IsFriendly)
        {
            if (CurrentEnemyRoles != null && CurrentEnemyRoles.Contains(role))
            {
                CurrentEnemyRoles.Remove(role);
                foreach (var roleObject in EnemyContainer?.GetChildren().Cast<EnemyRoleObject>() ?? [])
                {
                    if (roleObject.data != role) continue;
                    roleObject.data = null;
                    roleObject.QueueFree();
                }

                GD.Print("敌方角色：" + role.Name + "已战败，从当前数组中移除。当前剩余" + CurrentEnemyRoles.Count + "个敌人");
            }
        }

        if (CurrentEnemyRoles != null && CurrentEnemyRoles.Count != 0) return;
        StaticInstance.MainRoot.ShowBanner("敌方角色已全部战败，游戏结束");
        GD.Print("敌方角色已全部战败，游戏结束");
        EndBattle(true);
    }

    public void EndBattle(bool win)
    {
        StaticInstance.EventMgr.UnregisterIEvent(this);
        StaticInstance.PlayerData.Gsd.MajorRole.RecoverMagic();
        StartPlayerRoles.ForEach(obj =>
        {
            obj.EndFight();
            StaticInstance.EventMgr.UnregisterIEvent(obj);
        });
        StartEnemyRoles.ForEach(obj => StaticInstance.EventMgr.UnregisterIEvent(obj));
        PlayerRoles.Clear();
        CurrentEnemyRoles.Clear();
        CurrentPlayerRoles.Clear();
        EnemyRoles.Clear();
        StartEnemyRoles.Clear();
        StartPlayerRoles.Clear();
        foreach (var node in HandControl.GetChildren())
        {
            node.QueueFree();
        }

        foreach (var i in PlayerContainer?.GetChildren() ?? [])
        {
            i.QueueFree();
        }

        BattleStatic.Reset();
        StaticInstance.PlayerData.Gsd.MajorRole.FightSymbol.Clear();
        if (win && (_battlePreset != null || _isDebugFight))
        {
            if (!_isDebugFight)
            {
                var rewardScene =
                    (RewardScene)ResourceLoader.Load<PackedScene>("res://Pages/RewardScene.tscn").Instantiate();
                List<RewardStruct> datas = [];
                RewardStruct @struct = new()
                {
                    Type = RewardType.Gold
                };
                Random r = new();
                if (_battlePreset != null)
                {
                    var gold = (uint)r.Next(_battlePreset.MinGoldReward, _battlePreset.MaxGoldReward);
                    if (gold > 0)
                    {
                        @struct.Rewards = [gold.ToString()];
                        datas.Add(@struct);
                    }

                    RewardStruct @struct2 = new()
                    {
                        Type = RewardType.Exp,
                        Rewards = [_battlePreset.GainExp.ToString()]
                    };
                    if (_battlePreset.GainExp > 0) datas.Add(@struct2);
                }


                StaticInstance.WindowMgr.AddScene(rewardScene, datas);

                var nextRooms = StaticInstance.PlayerData.Gsd.MapGenerator?.LastRoom?.NextRooms;
                if (nextRooms == null || nextRooms.Count == 0)
                {
                    StaticInstance.PlayerData.Gsd.MapGenerator?.EndMap();
                }
            }

            _battlePreset = null;
        }
        else
        {
            BattleStatic.Reset();
            StaticInstance.PlayerData.Gsd = new GlobalSaveData();
            StaticInstance.WindowMgr.RemoveAllScene();
            StaticInstance.WindowMgr.ChangeScene(ResourceLoader.Load<PackedScene>("res://Pages/menu_scene.tscn")
                .Instantiate());
        }

        _endControl.Visible = true;
    }

    public void UpdateCounts()
    {
        _deckCount.Text = "卡组数量：" + NowPlayer.InFightDeckCount;
        GraveCount.Text = "墓地数量：" + NowPlayer.InFightGraveCount;
        _actionPointLabel.Text = "行动点：" + NowPlayer.CurrentActionPoint + "/" + NowPlayer.TurnActionPoint;
    }

    public EnemyRoleObject FindEnemyObjectByRole(EnemyRole role)
    {
        foreach (var i in EnemyContainer?.GetChildren() ?? [])
        {
            if (i is EnemyRoleObject ero && ero.data == role)
            {
                return ero;
            }
        }

        return null;
    }

    /// <summary>
    /// 选择卡牌，用于弃牌或者别的操作，暂时没想好要怎么实现
    /// </summary>
    /// <param name="max">最大数量</param>
    /// <param name="min">最小数量</param>
    /// <param name="filter">过滤条件</param>
    /// <param name="confirmAction">确定按钮的回调函数</param>
    /// <param name="mustList">必须选择的位置，以列表的形式传递</param>
    public void SelectCard(int max, int min, Func<Card, bool> filter, Action<List<Card>> confirmAction,
        List<int> mustList = null)
    {
        BattleStatic.MaxSelectCount = max;
        BattleStatic.MinSelectCount = min;
        BattleStatic.isDiscarding = true;
        if (mustList != null) BattleStatic.MustList = mustList.ToList();
        _selectConfirmBtn.Visible = true;
        _selectControl.Visible = true;
        BattleStatic.ConfirmAction = confirmAction;
        BattleStatic.SelectFilterFunc = filter;
        _selectConfirmBtn.Pressed += SelectConfirmAction;
        UpdateSelectBtn();
    }

    private void SelectConfirmAction()
    {
        if (BattleStatic.discard_list.Count < BattleStatic.MinSelectCount ||
            BattleStatic.discard_list.Count > BattleStatic.MaxSelectCount) return;
        BattleStatic.ConfirmAction?.Invoke(SelectConfirm());
        BattleStatic.EndSelect();
        DispatchEvent("SelectConfirm");
    }

    public List<Card> SelectConfirm()
    {
        _selectConfirmBtn.Visible = false;
        _selectControl.Visible = false;
        var res = BattleStatic.discard_list.ToList();
        BattleStatic.SelectFilterFunc = null;
        BattleStatic.ConfirmAction = null;
        BattleStatic.discard_list.Clear();
        BattleStatic.isDiscarding = false;
        return res;
    }

    public enum DisCardReason
    {
        EndTurn,
        Effect,
    }

    public void DisCard(List<Card> cards, PlayerRole owner, DisCardReason reason, BaseRole from)
    {
        foreach (var card in cards)
        {
            owner.InFightHands.Remove(card);
            owner.InFightGrave.Add(card);
            card.DiscardAction?.Invoke(owner, reason, from);
            if (owner != NowPlayer) continue;
            foreach (var cObj in HandControl.GetChildren().Cast<CardObject>())
            {
                if (cObj.card != card) continue;
                cObj.QueueFree();
                break;
            }
        }

        GetTree().CreateTimer(0.5f).Timeout += () =>
        {
            _hoveredIndex = -1;
            UpdateCardPosition();
        };
        DispatchEvent("DISCARD", cards, owner, reason);
    }

    private void UpdateSelectBtn()
    {
        if (BattleStatic.discard_list.Count < BattleStatic.MinSelectCount ||
            BattleStatic.discard_list.Count > BattleStatic.MaxSelectCount)
        {
            _selectConfirmBtn.Disabled = true;
        }
        else
        {
            _selectConfirmBtn.Disabled = false;
        }
    }
}