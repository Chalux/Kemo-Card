using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class EndBattleRewardItem : Control
{
    private static readonly Dictionary<RewardType, string> TypeIconPath = new()
    {
        { RewardType.Card, "res://Mods/MainPackage/Resources/Icons/icons_231.png" },
        { RewardType.Equip, "res://Mods/MainPackage/Resources/Icons/icons_079.png" },
        { RewardType.Gold, "res://Mods/MainPackage/Resources/Icons/icons_204.png" },
        { RewardType.Exp, "res://Mods/MainPackage/Resources/Icons/icons_032.png" }
    };

    [Export] private RwdButton _btn;
    private RewardType _rewardType;
    private string[] _data;

    private RewardType RType
    {
        get => _rewardType;
        set
        {
            _rewardType = value;
            _btn.Icon = ResourceLoader.Load<CompressedTexture2D>(TypeIconPath[value]);
        }
    }

    private void SetData(RewardType rewardType, string[] data)
    {
        RType = rewardType;
        _data = data;
    }

    public override void _Ready()
    {
        base._Ready();
        _btn.Pressed += () =>
        {
            if (_data == null) return;
            switch (RType)
            {
                case RewardType.Card:
                    if (_data.Length > 0)
                    {
                        var res = ResourceLoader.Load<PackedScene>("res://Pages/SelectCardScene.tscn");
                        if (res != null)
                        {
                            var scs = res.Instantiate<SelectCardScene>();
                            scs.Init(_data.ToList());
                            StaticInstance.WindowMgr.AddScene(scs);
                        }
                    }

                    break;
                case RewardType.Equip:
                    if (_data.Length > 0)
                    {
                        StaticInstance.PlayerData.Gsd.MajorRole.AddEquipToBag(new(_data[0]));
                    }

                    break;
                case RewardType.Exp:
                    if (_data.Length > 0)
                    {
                        StaticInstance.PlayerData.Gsd.MajorRole.Exp += (uint)_data[0].ToInt();
                    }

                    break;
                case RewardType.None:
                case RewardType.Gold:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Dispose();
        };
    }
}