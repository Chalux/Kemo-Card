using System;
using KemoCard.Scripts.Presets;

namespace KemoCard.Mods.MainPackage.Scripts.Presets;

internal partial class Bat : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["bat"];
        preset.MaxGoldReward = 70;
        preset.MinGoldReward = 50;
        preset.GainExp = 7;
    }
}

internal partial class FireBat : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["fire_bat"];
        preset.MaxGoldReward = 70;
        preset.MinGoldReward = 50;
        preset.GainExp = 7;
    }
}

internal partial class Frog : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["frog"];
        preset.MinGoldReward = 100;
        preset.MaxGoldReward = 150;
        preset.GainExp = 30;
    }
}

internal partial class FrogRandombat : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["frog"];
        Random r = new();
        var i = r.Next(0, 2);
        switch (i)
        {
            case 0:
                preset.Enemies.Add("bat");
                break;
            case 1:
                preset.Enemies.Add("fire_bat");
                break;
            case 2:
                preset.Enemies.Add("water_bat");
                break;
        }

        preset.MinGoldReward = 120;
        preset.MaxGoldReward = 170;
        preset.GainExp = 35;
    }
}

internal partial class FrogZombie : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["frog", "zombie"];
        preset.MinGoldReward = 200;
        preset.MaxGoldReward = 300;
        preset.GainExp = 50;
    }
}

internal partial class Ghost : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["ghost"];
        preset.MinGoldReward = 30;
        preset.MaxGoldReward = 50;
        preset.GainExp = 10;
    }
}

internal partial class GhostGoblin : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["ghost", "goblin"];
        preset.MinGoldReward = 80;
        preset.MaxGoldReward = 120;
        preset.GainExp = 20;
    }
}

internal partial class Giant : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["giant"];
        preset.MinGoldReward = 150;
        preset.MaxGoldReward = 200;
        preset.GainExp = 50;
    }
}

internal partial class Goblin : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["goblin"];
        preset.MinGoldReward = 20;
        preset.MaxGoldReward = 30;
        preset.GainExp = 5;
    }
}

internal partial class GoblinRandombat : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["goblin"];
        Random r = new();
        var i = r.Next(0, 2);
        switch (i)
        {
            case 0:
                preset.Enemies.Add("bat");
                break;
            case 1:
                preset.Enemies.Add("fire_bat");
                break;
            case 2:
                preset.Enemies.Add("water_bat");
                break;
        }

        preset.MinGoldReward = 50;
        preset.MaxGoldReward = 80;
        preset.GainExp = 10;
    }
}

internal partial class Skeleton : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["skeleton"];
        preset.MinGoldReward = 30;
        preset.MaxGoldReward = 50;
        preset.GainExp = 10;
    }
}

internal partial class SkeletonZombie : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["skeleton", "zombie"];
        preset.MinGoldReward = 250;
        preset.MaxGoldReward = 400;
        preset.GainExp = 75;
    }
}

internal partial class Slime : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["slime"];
        preset.MinGoldReward = 30;
        preset.MaxGoldReward = 50;
        preset.GainExp = 10;
    }
}

internal partial class SlimeBat : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["slime", "bat"];
        preset.MinGoldReward = 60;
        preset.MaxGoldReward = 80;
        preset.GainExp = 15;
    }
}

internal partial class SmallGnome : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["small_gnome"];
        preset.MinGoldReward = 40;
        preset.MaxGoldReward = 75;
        preset.GainExp = 15;
    }
}

internal partial class SmallGnomeGoblin : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["small_gnome", "goblin"];
        preset.MinGoldReward = 60;
        preset.MaxGoldReward = 80;
        preset.GainExp = 15;
    }
}

internal partial class ThreeBats : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["bat", "fire_bat", "water_bat"];
        preset.MinGoldReward = 70;
        preset.MaxGoldReward = 120;
        preset.GainExp = 15;
    }
}

internal partial class Troll : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["troll"];
        preset.MinGoldReward = 100;
        preset.MaxGoldReward = 150;
        preset.GainExp = 50;
    }
}

internal partial class TrollBat : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["bat", "troll"];
        preset.MinGoldReward = 80;
        preset.MaxGoldReward = 130;
        preset.GainExp = 30;
    }
}

internal partial class WaterBat : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["water_bat"];
        preset.MaxGoldReward = 70;
        preset.MinGoldReward = 50;
        preset.GainExp = 7;
    }
}

internal partial class Wolf : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["wolf"];
        preset.MinGoldReward = 100;
        preset.MaxGoldReward = 150;
        preset.GainExp = 30;
    }
}

internal partial class Zombie : BasePresetScript
{
    public override void Init(Preset preset)
    {
        preset.Enemies = ["zombie"];
        preset.MinGoldReward = 100;
        preset.MaxGoldReward = 150;
        preset.GainExp = 20;
    }
}