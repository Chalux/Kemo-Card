using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using KemoCard.Scripts.Presets;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using static StaticClass.StaticEnums;

public struct RewardStruct
{
    public RewardType type;
    public List<string> rewards;
}

public struct ShopStruct
{
    public Card card;
    public int price;
    public bool isBuyed;
}

public partial class BattleScene : BaseScene, IEvent
{
    [Export] HBoxContainer playerContainer;
    [Export] HBoxContainer enemyContainer;
    [Export] public Control HandControl;
    [Export] Label DeckCount;
    [Export] public Label GraveCount;
    [Export] Label ActionPointLabel;
    [Export] float TweenSpeed = 0.25f;
    [Export] float TotalAngle = 0f;
    [Export] float SpaceX = 200f;
    [Export] float SpaceY = 7f;
    [Export] Godot.Button TurnEndBtn;
    [Export] public Control DragDwonArea;
    [Export] Godot.Button DebugDrawBtn;
    [Export] Label TurnLabel;
    [Export] Godot.Button DeckBtn;
    [Export] Godot.Button GraveBtn;
    [Export] Godot.Button WholeDeckBtn;
    [Export] Godot.Button DebugWinBtn;
    [Export] Godot.Button ReturnBtn;
    [Export] Control EndControl;
    [Export] Godot.Button SelectConfirmBtn;
    [Export] Control SelectControl;

    private bool isDebugFight = false;

    private int HoveredIndex = -1;
    private bool IsDragging = false;
    private Preset BattlePreset = null;
    private Action AfterBattleHandler;

    public PlayerRole nowPlayer;
    public void NewBattle(PlayerRole playerRole, string[] enemyRoles, bool isDebugFight = false, Action afterBattleHandler = null)
    {
        //playerRole.Init();
        List<PlayerRole> players = new()
        {
            playerRole
        };
        List<EnemyRole> enemies = new();
        foreach (var role in enemyRoles)
        {
            enemies.Add(new EnemyRole(role));
        }
        BattleStatic.Reset();
        this.isDebugFight = isDebugFight;
        NewBattleCore(players, enemies);
        AfterBattleHandler = afterBattleHandler;
    }

    public void NewBattleByPreset(string presetId, bool isDebugFight = false, Action afterBattleHandler = null)
    {
        Preset preset = new();
        if (Datas.Ins.PresetPool.ContainsKey(presetId))
        {
            var preset_info = Datas.Ins.PresetPool[presetId];
            var res = ResourceLoader.Load<CSharpScript>($"res://Mods/{preset_info.mod_id}/Scripts/Presets/P{presetId}.cs");
            var PresetScript = res.New().As<BasePresetScript>();
            PresetScript.Init(preset);
            BattlePreset = preset;
            NewBattle(StaticInstance.playerData.gsd.MajorRole, preset.Enemies.ToArray(), isDebugFight, afterBattleHandler);
        }
    }

    public override void _Ready()
    {
        DebugWinBtn.Visible = DebugDrawBtn.Visible = OS.IsDebugBuild();
        DebugDrawBtn.Pressed += new(() =>
        {
            if (nowPlayer != null)
            {
                DrawCard(10, nowPlayer);
            }
        });
        DebugWinBtn.Pressed += new(() =>
        {
            EndBattle(true);
        });
        TurnEndBtn.Pressed += new(() =>
        {
            EndTurn();
        });
        //GD.Print("battleScene Ready");
        DeckBtn.Pressed += new(() =>
        {
            var list = currentPlayerRoles[0].InFightDeck.ToList();
            StaticUtils.ShuffleArray(list);
            StaticInstance.windowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/DeckView.tscn").Instantiate()
                , new[] { list });
        });
        GraveBtn.Pressed += new(() =>
        {
            StaticInstance.windowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/DeckView.tscn").Instantiate()
                , new[] { currentPlayerRoles[0].InFightGrave.ToList() });
        });
        WholeDeckBtn.Pressed += new(() =>
        {
            StaticInstance.windowMgr.AddScene((BaseScene)ResourceLoader.Load<PackedScene>("res://Pages/DeckView.tscn").Instantiate()
                , new[] { currentPlayerRoles[0].Deck.ToList() });
        });
        ReturnBtn.Pressed += new(() =>
        {
            if (!BattleStatic.isFighting)
            {
                if (AfterBattleHandler != null)
                {
                    AfterBattleHandler.Invoke();
                    AfterBattleHandler = null;
                }
                else
                {
                    //StaticInstance.windowMgr.RemoveScene(this);
                    PackedScene res = ResourceLoader.Load<PackedScene>("res://Pages/MainScene.tscn");
                    if (res != null)
                    {
                        StaticInstance.windowMgr.ChangeScene(res.Instantiate<MainScene>());
                    }
                }
                StaticUtils.AutoSave();
            }
        });
    }

    private void UpdateCardPosition()
    {
        if (IsDragging) return;
        if (HandControl == null) return;
        foreach (CardObject i in HandControl.GetChildren().Cast<CardObject>())
        {
            i.StartReposition(CalculateCardPostion(i.GetIndex()), GetCardAngle(i.GetIndex()), TweenSpeed, GetCardScales(i.GetIndex()));
            //GD.Print("序号：" + i.GetIndex() + ",角度：" + GetCardAngle(i.GetIndex()) + ",位置：" + CalculateCardPostion(i.GetIndex()) + ",Pivot：" + i.PivotOffset);
        }
    }

    private Vector2 CalculateCardPostion(int index)
    {
        Vector2 vector = new()
        {
            X = GetCardXPosition(index),
            Y = GetCardYPosition(index)
        };
        return vector;
    }

    private float GetCardAngle(int CardIndex)
    {
        //return 0;
        if (HandControl.GetChildCount() == 0) return 0;
        else return (CardIndex - (HandControl.GetChildCount() - 1f) / 2f) * TotalAngle / HandControl.GetChildCount() / 360f;
    }

    private float GetCardYPosition(int CardIndex)
    {
        //return 0;
        float hoverShift;
        if (HoveredIndex == -1)
        {
            hoverShift = 0f;
        }
        else if (CardIndex == HoveredIndex)
        {
            hoverShift = -50f;
        }
        else
        {
            hoverShift = 50f;
        }

        return Math.Abs(CardIndex - (HandControl.GetChildCount() - 1f) / 2f) * SpaceY + hoverShift;
    }

    private float GetCardXPosition(int CardIndex)
    {
        float hoverShift = 0f;
        if (HoveredIndex == -1)
        {
            hoverShift = 0f;
        }
        else if (CardIndex < HoveredIndex)
        {
            hoverShift = -100f;
        }
        else if (CardIndex == HoveredIndex)
        {
            hoverShift = 0f;
        }
        else if (CardIndex > HoveredIndex)
        {
            hoverShift = 100f;
        }
        return (CardIndex - (HandControl.GetChildCount() - 1f) / 2f) * SpaceX + HandControl.Size.X / 2f - 128 + hoverShift;
    }

    private Vector2 GetCardScales(int CardIndex)
    {
        if (HoveredIndex == CardIndex) return new Vector2(1.2f, 1.2f);
        else return Vector2.One;
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        if (@event == "RepositionHand")
        {
            if (datas != null && datas.Length > 0)
            {
                HoveredIndex = (int)datas[0];
            }
            else
            {
                HoveredIndex = -1;
            }
            UpdateCardPosition();
        }
        else if (@event == "DraggingCard")
        {
            if (datas != null && datas.Length > 0)
            {
                IsDragging = (bool)datas[0];
            }
            else
            {
                IsDragging = false;
            }
            UpdateCardPosition();
        }
        else if (@event == "Attack")
        {
            DealDamage((int)datas[0], (AttackType)datas[1], (BaseRole)datas[2], (List<BaseRole>)datas[3]);
        }
        else if (@event == "PropertiesChanged")
        {
            if (datas != null)
            {
                if (datas[0] is PlayerRole ifp && ifp == nowPlayer)
                {
                    if (ifp == nowPlayer)
                    {
                        UpdateCounts();
                    }
                }
            }
        }
        else if (@event == "StartSelectTarget")
        {
            DragDwonArea.Visible = false;
        }
        else if (@event == "EndSelectTarget")
        {
            DragDwonArea.Visible = true;
        }
        else if (@event == "SelectCard")
        {
            UpdateSelectBtn();
        }
    }

    public List<PlayerRole> playerRoles = new();
    public List<PlayerRole> currentPlayerRoles = new();
    public List<PlayerRole> startPlayerRoles = new();
    public List<EnemyRole> enemyRoles = new();
    public List<EnemyRole> currentEnemyRoles = new();
    public List<EnemyRole> startEnemyRoles = new();
    public int turns = 0;

    public TeamEnum currentController;

    public void NewBattleCore(List<PlayerRole> players, List<EnemyRole> enemies)
    {
        BattleStatic.isFighting = true;
        EndControl.Visible = false;
        foreach (PlayerRole i in players)
        {
            i.Deck.ForEach(card =>
            {
                var c = new Card(card.Id)
                {
                    owner = i
                };
            });
            playerRoles.Add(i);
            currentPlayerRoles.Add(i);
            startPlayerRoles.Add(i);
            PlayerRoleObject po = ResourceLoader.Load<PackedScene>("res://Pages/PlayerRoleObject.tscn").Instantiate<PlayerRoleObject>();
            po.InitByPlayerRole(i);
            i.TurnActionPoint = i.CurrentActionPoint = i.ActionPoint;
            i.InitFighter();
            i.OnBattleStart?.Invoke();
            playerContainer.AddChild(po);
        }
        foreach (EnemyRole i in enemies)
        {
            enemyRoles.Add(i);
            currentEnemyRoles.Add(i);
            startEnemyRoles.Add(i);
            EnemyRoleObject eo = ResourceLoader.Load<PackedScene>("res://Pages/EnemyRoleObject.tscn").Instantiate<EnemyRoleObject>();
            StaticInstance.eventMgr.RegistIEvent(eo);
            eo.Init(i);
            i.OnBattleStart?.Invoke(eo);
            enemyContainer.AddChild(eo);
        }
        turns = 1;
        TurnLabel.Text = "回合数：" + turns;
        currentController = TeamEnum.Player;
        nowPlayer = currentPlayerRoles[0];
        StartBattle();
    }

    void StartBattle()
    {
        foreach (PlayerRole i in currentPlayerRoles)
        {
            StaticUtils.ShuffleArray(i.InFightDeck);
        }
        foreach (var player in playerRoles)
        {
            player.CurrMBlock = (int)Math.Floor(player.OriginMBlock);
            player.CurrPBlock = (int)Math.Floor(player.OriginPBlock);
        }
        foreach (var enemy in enemyRoles)
        {
            enemy.CurrMBlock = (int)Math.Floor(enemy.OriginMBlock);
            enemy.CurrPBlock = (int)Math.Floor(enemy.OriginPBlock);
        }
        nowPlayer = currentPlayerRoles[0];
        foreach (PlayerRole i in currentPlayerRoles)
        {
            DrawCard(5, i);
        }
        object[] objects = new object[] { currentPlayerRoles, currentEnemyRoles, turns };
        StaticInstance.eventMgr.Dispatch("StartBattle", objects);
        UpdateCounts();
    }

    public void NextTurn()
    {
        if (!BattleStatic.isFighting) return;
        turns++;
        TurnLabel.Text = "回合数：" + turns;
        foreach (var player in currentPlayerRoles)
        {
            player.RecoverMagic();
            player.CurrentActionPoint = player.TurnActionPoint;
        }
        foreach (var enemy in currentEnemyRoles)
        {
            enemy.RecoverMagic();
        }
        if (currentController == TeamEnum.Player)
        {
            nowPlayer = currentPlayerRoles[0];
            foreach (PlayerRole i in currentPlayerRoles)
            {
                DrawCard(5, i);
            }
        }
        StaticInstance.eventMgr.Dispatch("NewTurn", new dynamic[] { turns, currentPlayerRoles, currentEnemyRoles });
        UpdateCounts();
    }

    public void EndTurn()
    {
        foreach (var player in currentPlayerRoles)
        {
            //player.InFightHands.ForEach(c => player.InFightGrave.Add(c));
            //player.InFightHands.Clear();
            var list = player.InFightHands.ToList();
            var discardlist = list.Where((Card c) => !c.InGameDict.ContainsKey("KeepInHand") && !c.GlobalDict.ContainsKey("KeepInHand")).ToList();
            DisCard(discardlist, player, DisCardReason.END_TURN);
            //for (int i = 0; i < list.Count; i++)
            //{
            //    var c = list[i];
            //    if (c.InGameDict.ContainsKey("KeepInHand") || c.GlobalDict.ContainsKey("KeepInHand"))
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        player.InFightGrave.Add(c);
            //        player.InFightHands.Remove(c);
            //    }
            //}
        }
        foreach (CardObject i in HandControl.GetChildren().Cast<CardObject>())
        {
            if (i.card.InGameDict.ContainsKey("KeepInHand") || i.card.GlobalDict.ContainsKey("KeepInHand")) continue;
            else i.QueueFree();
        }
        for (int i = 0; i < currentEnemyRoles.Count; i++)
        {
            if (currentEnemyRoles[i] == null || !BattleStatic.isFighting) return;
            currentEnemyRoles[i].script.ActionFunc?.Invoke(turns, currentPlayerRoles, currentEnemyRoles);
        }
        NextTurn();
    }

    public AttackResult DealDamage(double value, AttackType attackType, BaseRole from, List<BaseRole> targets, AttributeEnum atrribute = AttributeEnum.None, int times = 1)
    {
        if (!BattleStatic.isFighting) return AttackResult.Failed;
        Damage damage = new()
        {
            validTag = true,
            value = value,
            from = from,
            targets = targets,
            atrribute = atrribute,
            attackType = attackType,
            times = times
        };
        StaticInstance.eventMgr.Dispatch("BeforeAttacked", damage);
        StaticInstance.eventMgr.Dispatch("BeforeDealDamage", damage);
        if (damage.validTag)
        {
            foreach (BaseRole target in targets)
            {
                Damage tempDamage = new()
                {
                    validTag = damage.validTag,
                    value = damage.value,
                    from = damage.from,
                    targets = new() { target },
                    atrribute = damage.atrribute,
                    attackType = damage.attackType,
                    times = damage.times
                };
                GD.Print("伤害结算前：" + tempDamage.value);
                StaticInstance.eventMgr.Dispatch("BeforeAttackedSingle", tempDamage);
                if (tempDamage.validTag)
                {
                    tempDamage.value = (int)Math.Round((tempDamage.value + target.GetSymbol(AttributeDic[atrribute] + "ResisProp", 0f) - from.GetSymbol(AttributeDic[atrribute] + "AtkProp", 0f)) *
                            (1 + target.GetSymbol(AttributeDic[atrribute] + "ResisPerm", 0f)) * (1 + from.GetSymbol(AttributeDic[atrribute] + "AtkPerm", 0f)));
                    StaticInstance.eventMgr.Dispatch("BeforeDealDamageSingle", tempDamage);
                    GD.Print("伤害结算后：" + tempDamage.value);
                    for (int i = 0; i < times; i++)
                    {
                        int random = new Random().Next(0, 100);
                        GD.Print("闪避摇点：" + random + "，被击者触发闪避要求的点数不大于：" + target.CurrDodge);
                        if (random <= target.CurrDodge)
                        {
                            string log = from.name + "对" + target.name + "的攻击被闪避了。闪避摇点：" + random + "，被击者触发闪避要求的点数不大于：" + target.CurrDodge;
                            GD.Print(log);
                            StaticInstance.MainRoot.ShowBanner(log);
                            continue;
                            //return AttackResult.Dodged;
                        }
                        int diff;
                        if (tempDamage.attackType == AttackType.Physics)
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
                            if (tempDamage.attackType == AttackType.Physics)
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
                            if (tempDamage.attackType == AttackType.Physics)
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
                        if (target is EnemyRole ter)
                        {
                            FindEnemyObjectByRole(ter)?.hitFlashAnimationPlayer.Play("hit_flash");
                        }

                        if (target.CurrHealth <= 0)
                        {
                            GD.Print(target.GetName() + "死了");
                            StaticInstance.eventMgr.Dispatch("BeforeDie", target);
                            if (target.CurrHealth > 0) continue;
                            else
                            {
                                if (target is PlayerRole ifp) PlayerRoleDie(ifp);
                                else if (target is EnemyRole er) EnemyRoleDie(er);
                                return AttackResult.Success;
                            }
                        }
                    }
                }
                else
                {
                    StaticInstance.eventMgr.Dispatch("AttackInvalided", tempDamage);
                    GD.Print(from.name + "对" + target.name + "的攻击被无效了");
                    return AttackResult.Invalidated;
                }
            }
            StaticInstance.eventMgr.Dispatch("AfterAttacked", damage);
            return AttackResult.Success;
        }
        else
        {
            StaticInstance.eventMgr.Dispatch("AttackInvalided", damage);
            GD.Print("攻击被无效了");
            return AttackResult.Invalidated;
        }
    }

    public void DrawCard(int num, PlayerRole target)
    {
        for (int i = 0; i < num; i++)
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
            Card c = target.InFightDeck.First();
            target.InFightDeck.RemoveAt(0);
            target.InFightHands.Add(c);
            if (target == nowPlayer)
            {
                CardObject cardObject = ResourceLoader.Load<PackedScene>("res://Pages/CardObject.tscn").Instantiate<CardObject>();
                cardObject.InitData(c);
                HandControl.AddChild(cardObject);
            }
        }
        GetTree().CreateTimer(0.1f).Timeout += new(() => UpdateCardPosition());
        UpdateCounts();
    }

    public void PlayerRoleDie(PlayerRole role)
    {
        if (role.isFriendly)
        {
            if (currentPlayerRoles.Contains(role))
            {
                currentPlayerRoles.Remove(role);
                foreach (PlayerRoleObject roleObject in playerContainer.GetChildren().Cast<PlayerRoleObject>())
                {
                    if (roleObject.data == role)
                    {
                        roleObject.data = null;
                        roleObject.QueueFree();
                    }
                }
                GD.Print("友方角色：" + role.name + "已战败，从当前数组中移除。当前剩余" + currentPlayerRoles.Count + "个队友");
            }
        }
        if (currentPlayerRoles.Count == 0)
        {
            StaticInstance.MainRoot.ShowBanner("友方角色已全部战败，游戏结束");
            GD.Print("友方角色已全部战败，游戏结束");
            EndBattle(false);
        }
    }

    public void EnemyRoleDie(EnemyRole role)
    {
        if (!role.isFriendly)
        {
            if (currentEnemyRoles.Contains(role))
            {
                currentEnemyRoles.Remove(role);
                foreach (EnemyRoleObject roleObject in enemyContainer.GetChildren().Cast<EnemyRoleObject>())
                {
                    if (roleObject.data == role)
                    {
                        roleObject.data = null;
                        roleObject.QueueFree();
                    }
                }
                GD.Print("敌方角色：" + role.name + "已战败，从当前数组中移除。当前剩余" + currentEnemyRoles.Count + "个敌人");
            }
        }
        if (currentEnemyRoles.Count == 0)
        {
            StaticInstance.MainRoot.ShowBanner("敌方角色已全部战败，游戏结束");
            GD.Print("敌方角色已全部战败，游戏结束");
            EndBattle(true);
        }
    }

    public void EndBattle(bool win)
    {
        StaticInstance.eventMgr.UnregistIEvent(this);
        StaticInstance.playerData.gsd.MajorRole.RecoverMagic();
        startPlayerRoles.ForEach(obj =>
        {
            obj.EndFight();
            StaticInstance.eventMgr.UnregistIEvent(obj);
        });
        startEnemyRoles.ForEach(obj => StaticInstance.eventMgr.UnregistIEvent(obj));
        playerRoles.Clear();
        currentEnemyRoles.Clear();
        currentPlayerRoles.Clear();
        enemyRoles.Clear();
        startEnemyRoles.Clear();
        startPlayerRoles.Clear();
        foreach (Node node in HandControl.GetChildren())
        {
            node.QueueFree();
        }
        foreach (var i in playerContainer.GetChildren())
        {
            i.QueueFree();
        }
        BattleStatic.Reset();
        StaticInstance.playerData.gsd.MajorRole.FightSymbol.Clear();
        if (win && BattlePreset != null)
        {
            RewardScene rewardScene = (RewardScene)ResourceLoader.Load<PackedScene>("res://Pages/RewardScene.tscn").Instantiate();
            List<RewardStruct> datas = new();
            RewardStruct @struct = new()
            {
                type = RewardType.Gold
            };
            Random r = new();
            uint gold = (uint)r.Next(BattlePreset.MinGoldReward, BattlePreset.MaxGoldReward);
            if (gold > 0)
            {
                @struct.rewards = new() { gold.ToString() };
                datas.Add(@struct);
            }
            RewardStruct @struct2 = new()
            {
                type = RewardType.Exp,
                rewards = new() { BattlePreset.GainExp.ToString() }
            };
            if (BattlePreset.GainExp > 0) datas.Add(@struct2);
            StaticInstance.windowMgr.AddScene(rewardScene, datas);
            if (BattlePreset.IsBoss)
            {
                StaticInstance.playerData.gsd.MapGenerator.EndMap();
            }
            BattlePreset = null;
        }
        EndControl.Visible = true;
    }

    public void UpdateCounts()
    {
        DeckCount.Text = "卡组数量：" + nowPlayer.InFightDeckCount;
        GraveCount.Text = "墓地数量：" + nowPlayer.InFightGraveCount;
        ActionPointLabel.Text = "行动点：" + nowPlayer.CurrentActionPoint + "/" + nowPlayer.TurnActionPoint;
    }

    public EnemyRoleObject FindEnemyObjectByRole(EnemyRole role)
    {
        foreach (var i in enemyContainer.GetChildren())
        {
            if (i is EnemyRoleObject ero && ero.data == role)
            {
                return i as EnemyRoleObject;
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
    /// <param name="ConfirmAction">确定按钮的回调函数</param>
    /// <param name="mustlist">必须选择的位置，以列表的形式传递</param>
    public void SelectCard(int max, int min, Func<Card, bool> filter, Action<List<Card>> ConfirmAction, List<int> mustlist = null)
    {
        BattleStatic.MaxSelectCount = max;
        BattleStatic.MinSelectCount = min;
        BattleStatic.isDiscarding = true;
        if (mustlist != null) BattleStatic.MustList = mustlist.ToList();
        SelectConfirmBtn.Visible = true;
        SelectControl.Visible = true;
        BattleStatic.ConfirmAction = ConfirmAction;
        BattleStatic.SelectFilterFunc = filter;
        SelectConfirmBtn.Pressed += SelectConfirmAction;
        UpdateSelectBtn();
    }

    private void SelectConfirmAction()
    {
        if (BattleStatic.discard_list.Count >= BattleStatic.MinSelectCount && BattleStatic.discard_list.Count <= BattleStatic.MaxSelectCount)
        {
            BattleStatic.ConfirmAction?.Invoke(SelectConfirm());
            BattleStatic.EndSelect();
            DispatchEvent("SelectConfirm");
        }
    }

    public List<Card> SelectConfirm()
    {
        SelectConfirmBtn.Visible = false;
        SelectControl.Visible = false;
        var res = BattleStatic.discard_list.ToList();
        BattleStatic.SelectFilterFunc = null;
        BattleStatic.ConfirmAction = null;
        BattleStatic.discard_list.Clear();
        BattleStatic.isDiscarding = false;
        return res;
    }

    public enum DisCardReason
    {
        END_TURN,
        EFFECT,
    }
    public void DisCard(List<Card> cards, PlayerRole owner, DisCardReason reason)
    {
        foreach (var card in cards)
        {
            owner.InFightHands.Remove(card);
            owner.InFightGrave.Add(card);
            card.DiscardAction?.Invoke(owner, reason);
            if (owner == nowPlayer)
            {
                foreach (CardObject cobj in HandControl.GetChildren().Cast<CardObject>())
                {
                    if (cobj.card == card)
                    {
                        cobj.QueueFree();
                        break;
                    }
                }
            }
        }
        GetTree().CreateTimer(0.5f).Timeout += () =>
        {
            HoveredIndex = -1;
            UpdateCardPosition();
        };
        DispatchEvent("DISCARD", cards, owner, reason);
    }

    private void UpdateSelectBtn()
    {
        if (BattleStatic.discard_list.Count < BattleStatic.MinSelectCount || BattleStatic.discard_list.Count > BattleStatic.MaxSelectCount)
        {
            SelectConfirmBtn.Disabled = true;
        }
        else
        {
            SelectConfirmBtn.Disabled = false;
        }
    }
}
