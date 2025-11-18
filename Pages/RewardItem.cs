using System;
using System.Collections.Generic;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public enum RewardType
{
    None = 0,
    Gold = 1,
    Card = 2,
    Equip = 3,
    Exp = 4,
}

public partial class RewardItem : Control
{
    [Export] private Button _btn;
    private RewardType _type = RewardType.None;
    private List<string> _rewards = [];

    private RewardType Type
    {
        get => _type;
        set
        {
            _type = value;
            if (_type == RewardType.None) return;
            switch (value)
            {
                case RewardType.Gold:
                    _btn.Pressed += GoldAction;
                    _btn.Icon = ResourceLoader.Load<CompressedTexture2D>(
                        "res://Mods/MainPackage/Resources/Icons/icons_204.png");
                    _btn.Text = _rewards.Count > 0 ? $"金币({_rewards[0]})" : "金币(出错)";
                    break;
                case RewardType.Card:
                    _btn.Pressed += CardAction;
                    _btn.Icon = ResourceLoader.Load<CompressedTexture2D>(
                        "res://Mods/MainPackage/Resources/Icons/icons_231.png");
                    _btn.Text = _rewards.Count > 0 ? "选择卡牌" : "选择卡牌(出错)";
                    break;
                case RewardType.Equip:
                    _btn.Pressed += EquipAction;
                    if (_rewards.Count > 0)
                    {
                        Equip e = new(_rewards[0]);
                        _btn.Icon = ResourceLoader.Load<CompressedTexture2D>(e.EquipScript.TextureUrl);

                        _btn.Text = $"装备({e.EquipScript.Name})";
                        _btn.MouseEntered += () => { StaticInstance.MainRoot.ShowRichHint(e.EquipScript.Desc); };
                        _btn.MouseExited += StaticInstance.MainRoot.HideRichHint;
                    }
                    else _btn.Text = "装备(出错)";

                    break;
                case RewardType.Exp:
                    _btn.Pressed += ExpAction;
                    _btn.Icon = ResourceLoader.Load<CompressedTexture2D>(
                        "res://Mods/MainPackage/Resources/Icons/icons_032.png");
                    _btn.Text = _rewards.Count > 0 ? $"经验({_rewards[0]})" : "经验(出错)";
                    break;
                case RewardType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }

    public void SetReward(RewardType type, List<string> rewards)
    {
        _rewards = rewards;
        Type = type;
    }

    private void GoldAction()
    {
        StaticInstance.PlayerData.Gsd.MajorRole.Gold += _rewards.Count > 0 ? _rewards[0].ToInt() : 0;
        GetParent().RemoveChild(this);
        QueueFree();
    }

    private void CardAction()
    {
        var selectCardScene = ResourceLoader.Load<PackedScene>("res://Pages/SelectCardScene.tscn")
            .Instantiate<SelectCardScene>();
        selectCardScene.Init(_rewards);
        StaticInstance.WindowMgr.AddScene(selectCardScene, this);
    }

    private void EquipAction()
    {
        if (_rewards.Count == 0) return;
        var id = _rewards[0];
        StaticInstance.PlayerData.Gsd.MajorRole.AddEquipToBag(new Equip(id));
        GetParent().RemoveChild(this);
        QueueFree();
    }

    private void ExpAction()
    {
        StaticInstance.PlayerData.Gsd.MajorRole.Exp += _rewards.Count > 0 ? (uint)_rewards[0].ToInt() : 0;
        GetParent().RemoveChild(this);
        QueueFree();
    }
}