using Godot;
using System.Collections.Generic;
using System.Linq;
using KemoCard.Scripts;

public partial class EndBattleRewardItem : Control
{
    private readonly static Dictionary<RewardType, string> TypeIconPath = new() {
        { RewardType.Card, "res://Mods/MainPackage/Resources/Icons/icons_231.png" },
        { RewardType.Equip, "res://Mods/MainPackage/Resources/Icons/icons_079.png" },
        { RewardType.Gold, "res://Mods/MainPackage/Resources/Icons/icons_204.png" },
        { RewardType.Exp, "res://Mods/MainPackage/Resources/Icons/icons_032.png" }
    };
    [Export] Button btn;
    private RewardType _rewardType;
    private string[] data;

    public RewardType RType
    {
        get => _rewardType;
        set
        {
            _rewardType = value;
            btn.Icon = ResourceLoader.Load<CompressedTexture2D>(TypeIconPath[value]);
        }
    }
    public void SetData(RewardType rewardType, string[] data)
    {
        RType = rewardType;
        this.data = data;
    }

    public override void _Ready()
    {
        base._Ready();
        btn.Pressed += new(() =>
        {
            if (data != null)
            {
                switch (RType)
                {
                    case RewardType.Card:
                        if (data.Length > 0)
                        {
                            PackedScene res = ResourceLoader.Load<PackedScene>("res://Pages/SelectCardScene.tscn");
                            if (res != null)
                            {
                                KemoCard.Pages.SelectCardScene scs = res.Instantiate<KemoCard.Pages.SelectCardScene>();
                                scs.Init(data.ToList());
                                StaticInstance.WindowMgr.AddScene(scs);
                            }
                        }
                        break;
                    case RewardType.Equip:
                        if (data.Length > 0)
                        {
                            StaticInstance.PlayerData.Gsd.MajorRole.AddEquipToBag(new(data[0]));
                        }
                        break;
                    case RewardType.Exp:
                        if (data.Length > 0)
                        {
                            StaticInstance.PlayerData.Gsd.MajorRole.Exp += (uint)data[0].ToInt();
                        }
                        break;
                }
                Dispose();
            }
        });
    }
}
