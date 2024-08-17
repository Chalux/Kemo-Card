using Godot;
using KemoCard.Scripts;
using StaticClass;
using System.Collections.Generic;

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
    [Export] Godot.Button btn;
    private RewardType _type = RewardType.None;
    public List<string> Rewards = new();
    public RewardType Type
    {
        get => _type;
        set
        {
            _type = value;
            if (_type == RewardType.None) return;
            switch (value)
            {
                case RewardType.Gold:
                    btn.Pressed += GoldAction;
                    btn.Icon = ResourceLoader.Load<CompressedTexture2D>("res://Mods/MainPackage/Resources/Icons/icons_204.png");
                    if (Rewards.Count > 0) btn.Text = $"金币({Rewards[0]})";
                    else btn.Text = $"金币(出错)";
                    break;
                case RewardType.Card:
                    btn.Pressed += CardAction;
                    btn.Icon = ResourceLoader.Load<CompressedTexture2D>("res://Mods/MainPackage/Resources/Icons/icons_231.png");
                    if (Rewards.Count > 0) btn.Text = $"选择卡牌";
                    else btn.Text = $"选择卡牌(出错)";
                    break;
                case RewardType.Equip:
                    btn.Pressed += EquipAction;
                    btn.Icon = ResourceLoader.Load<CompressedTexture2D>("res://Mods/MainPackage/Resources/Icons/icons_041.png");
                    if (Rewards.Count > 0)
                    {
                        Equip e = new(Rewards[0]);

                        btn.Text = $"装备()";
                    }
                    else btn.Text = $"装备(出错)";
                    break;
                case RewardType.Exp:
                    btn.Pressed += ExpAction;
                    btn.Icon = ResourceLoader.Load<CompressedTexture2D>("res://Mods/MainPackage/Resources/Icons/icons_032.png");
                    if (Rewards.Count > 0) btn.Text = $"经验({Rewards[0]})";
                    else btn.Text = $"经验(出错)";
                    break;
            }
        }
    }

    public void SetReward(RewardType type, List<string> rewards)
    {
        Rewards = rewards;
        Type = type;
    }

    private void GoldAction()
    {
        StaticInstance.playerData.gsd.MajorRole.Gold += Rewards.Count > 0 ? Rewards[0].ToInt() : 0;
        GetParent().RemoveChild(this);
        QueueFree();
    }
    private void CardAction()
    {
        SelectCardScene selectCardScene = ResourceLoader.Load<PackedScene>("res://Pages/SelectCardScene.tscn").Instantiate<SelectCardScene>();
        selectCardScene.Init(Rewards);
        StaticInstance.windowMgr.AddScene(selectCardScene, this);
    }
    private void EquipAction()
    {
        if (Rewards.Count == 0) return;
        string id = Rewards[0];
        StaticInstance.playerData.gsd.MajorRole.AddEquipToBag(new(id));
        GetParent().RemoveChild(this);
        QueueFree();
    }
    private void ExpAction()
    {
        StaticInstance.playerData.gsd.MajorRole.Exp += Rewards.Count > 0 ? (uint)Rewards[0].ToInt() : 0;
        GetParent().RemoveChild(this);
        QueueFree();
    }
}
