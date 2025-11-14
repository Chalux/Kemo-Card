using Godot;
using KemoCard.Mods.MainPackage.Scripts.Buffs;
using KemoCard.Mods.MainPackage.Scripts.Cards;
using KemoCard.Mods.MainPackage.Scripts.Enemies;
using KemoCard.Mods.MainPackage.Scripts.Equips;
using KemoCard.Mods.MainPackage.Scripts.Events;
using KemoCard.Mods.MainPackage.Scripts.Maps;
using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using KemoCard.Scripts.Cards;
using KemoCard.Scripts.Enemies;
using KemoCard.Scripts.Equips;
using KemoCard.Scripts.Events;
using KemoCard.Scripts.Map;
using KemoCard.Scripts.Presets;
using Telekinesis = KemoCard.Mods.MainPackage.Scripts.Cards.Telekinesis;

namespace KemoCard.Mods.MainPackage.Scripts
{
    internal partial class ModBoost : BaseModBoost
    {
        public override void OnInit()
        {
            GD.Print("MainPackage装载中");

            CardFactory.RegisterCard("create_book", typeof(CreateBook));
            CardFactory.RegisterCard("create_book_armor", typeof(CreateBookArmor));
            CardFactory.RegisterCard("create_book_attack", typeof(CreateBookAttack));
            CardFactory.RegisterCard("create_equip", typeof(CreateEquip));
            CardFactory.RegisterCard("create_equip_armor", typeof(CreateEquipArmor));
            CardFactory.RegisterCard("create_equip_attack", typeof(CreateEquipAttack));
            CardFactory.RegisterCard("double_hit", typeof(DoubleHit));
            CardFactory.RegisterCard("fire_ball", typeof(FireBall));
            CardFactory.RegisterCard("graceful_charity", typeof(GracefulCharity));
            CardFactory.RegisterCard("infinite", typeof(Infinite));
            CardFactory.RegisterCard("iron_shard", typeof(IronShard));
            CardFactory.RegisterCard("lucky", typeof(Lucky));
            CardFactory.RegisterCard("mag_draw", typeof(MagDraw));
            CardFactory.RegisterCard("magic_missile", typeof(MagicMissile));
            CardFactory.RegisterCard("mana_tide", typeof(ManaTide));
            CardFactory.RegisterCard("no_attack", typeof(NoAttack));
            CardFactory.RegisterCard("no_defense", typeof(NoDefense));
            CardFactory.RegisterCard("punch", typeof(Punch));
            CardFactory.RegisterCard("rock_drill", typeof(RockDrill));
            CardFactory.RegisterCard("self_mblock", typeof(SelfMblock));
            CardFactory.RegisterCard("self_pblock", typeof(SelfPblock));
            CardFactory.RegisterCard("shadow_shot", typeof(ShadowShot));
            CardFactory.RegisterCard("telekinesis", typeof(Telekinesis));
            CardFactory.RegisterCard("water_slash", typeof(WaterSlash));

            BuffFactory.RegisterBuff("angry", typeof(Angry));
            BuffFactory.RegisterBuff("bleed_wolf", typeof(BleedWolf));
            BuffFactory.RegisterBuff("fire_injury", typeof(FireInjury));
            BuffFactory.RegisterBuff("fire_resis", typeof(FireResis));
            BuffFactory.RegisterBuff("get_lucky", typeof(GetLucky));
            BuffFactory.RegisterBuff("ghost_body", typeof(GhostBody));
            BuffFactory.RegisterBuff("telekinesis", typeof(KemoCard.Mods.MainPackage.Scripts.Buffs.Telekinesis));
            BuffFactory.RegisterBuff("water_injury", typeof(WaterInjury));
            BuffFactory.RegisterBuff("water_resis", typeof(WaterResis));

            EnemyFactory.RegisterEnemy("bat", typeof(Bat));
            EnemyFactory.RegisterEnemy("fire_bat", typeof(FireBat));
            EnemyFactory.RegisterEnemy("frog", typeof(Frog));
            EnemyFactory.RegisterEnemy("ghost", typeof(Ghost));
            EnemyFactory.RegisterEnemy("giant", typeof(Giant));
            EnemyFactory.RegisterEnemy("goblin", typeof(Goblin));
            EnemyFactory.RegisterEnemy("skeleton", typeof(Skeleton));
            EnemyFactory.RegisterEnemy("slime", typeof(Slime));
            EnemyFactory.RegisterEnemy("small_gnome", typeof(SmallGnome));
            EnemyFactory.RegisterEnemy("troll", typeof(Troll));
            EnemyFactory.RegisterEnemy("water_bat", typeof(WaterBat));
            EnemyFactory.RegisterEnemy("wolf", typeof(Wolf));
            EnemyFactory.RegisterEnemy("zombie", typeof(Zombie));

            EquipFactory.RegisterEquip("base_attack", typeof(BaseAttack));
            EquipFactory.RegisterEquip("base_create_book", typeof(BaseCreateBook));
            EquipFactory.RegisterEquip("base_create_equip", typeof(BaseCreateEquip));
            EquipFactory.RegisterEquip("base_defense", typeof(BaseDefense));
            EquipFactory.RegisterEquip("magic_book", typeof(MagicBook));
            EquipFactory.RegisterEquip("test_equip_1", typeof(TestEquip1));
            EquipFactory.RegisterEquip("wood_sword", typeof(WoodSword));

            EventFactory.RegisterEvent("empty_event", typeof(EmptyEvent));
            EventFactory.RegisterEvent("heal_event", typeof(HealEvent));

            MapFactory.RegisterMap("cave", typeof(Cave));
            MapFactory.RegisterMap("forest", typeof(Forest));
            MapFactory.RegisterMap("swamp", typeof(Swamp));

            PresetFactory.RegisterPreset("bat", typeof(Presets.Bat));
            PresetFactory.RegisterPreset("fire_bat", typeof(Presets.FireBat));
            PresetFactory.RegisterPreset("frog", typeof(Presets.Frog));
            PresetFactory.RegisterPreset("frog_randombat", typeof(Presets.FrogRandombat));
            PresetFactory.RegisterPreset("frog_zombie", typeof(Presets.FrogZombie));
            PresetFactory.RegisterPreset("ghost", typeof(Presets.Ghost));
            PresetFactory.RegisterPreset("ghost_goblin", typeof(Presets.GhostGoblin));
            PresetFactory.RegisterPreset("giant", typeof(Presets.Giant));
            PresetFactory.RegisterPreset("goblin", typeof(Presets.Goblin));
            PresetFactory.RegisterPreset("goblin_randombat", typeof(Presets.GoblinRandombat));
            PresetFactory.RegisterPreset("skeleton", typeof(Presets.Skeleton));
            PresetFactory.RegisterPreset("skeleton_zombie", typeof(Presets.SkeletonZombie));
            PresetFactory.RegisterPreset("slime", typeof(Presets.Slime));
            PresetFactory.RegisterPreset("slime_bat", typeof(Presets.SlimeBat));
            PresetFactory.RegisterPreset("small_gnome", typeof(Presets.SmallGnome));
            PresetFactory.RegisterPreset("small_gnome_goblin", typeof(Presets.SmallGnomeGoblin));
            PresetFactory.RegisterPreset("three_bats", typeof(Presets.ThreeBats));
            PresetFactory.RegisterPreset("troll", typeof(Presets.Troll));
            PresetFactory.RegisterPreset("troll_bat", typeof(Presets.TrollBat));
            PresetFactory.RegisterPreset("water_bat", typeof(Presets.WaterBat));
            PresetFactory.RegisterPreset("wolf", typeof(Presets.Wolf));
            PresetFactory.RegisterPreset("zombie", typeof(Presets.Zombie));
        }

        public override void OnInitEnd()
        {
            GD.Print("MainPackage已装载");
        }
    }
}