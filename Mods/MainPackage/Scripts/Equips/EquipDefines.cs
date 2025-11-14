using KemoCard.Scripts;
using KemoCard.Scripts.Equips;

namespace KemoCard.Mods.MainPackage.Scripts.Equips;

public partial class BaseAttack : BaseEquipScript
{
    public override void OnEquipInit(EquipImplBase eq)
    {
        eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_078.png";
        eq.CardDic.TryAdd("punch", 1);
        eq.CardDic.TryAdd("magic_missile", 1);
        eq.Desc = "基础攻击。\n为卡组添加一张无属性物攻(消耗1行动力，对敌方单体造成5点无属性物理伤害.)和一张无属性魔攻(消耗1行动力，对敌方单体造成5点无属性魔法伤害.)";
        eq.Name = "基础攻击";
    }
}

public partial class BaseCreateBook : BaseEquipScript
{
    public override void OnEquipInit(EquipImplBase eq)
    {
        eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_223.png";
        eq.Desc = "基础创术包。\n为卡组添加两张创术(消耗。从2张牌中选择1张牌临时加入你的牌组。敌方单体造成5 + (1*书写)点无属性魔法伤害/获得2 + (1*书写)点魔甲)";
        eq.CardDic.TryAdd("create_book", 2);
        eq.Name = "基础创术包";
    }
}

public partial class BaseCreateEquip : BaseEquipScript
{
    public override void OnEquipInit(EquipImplBase eq)
    {
        eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_222.png";
        eq.Desc = "基础创物包。\n为卡组添加两张创物(消耗。从2张牌中选择1张牌临时加入你的牌组。敌方单体造成5 + (1*工艺)点无属性物理伤害/获得2 + (1*工艺)点护甲)";
        eq.CardDic.TryAdd("create_equip", 2);
        eq.Name = "基础创物包";
    }
}

public partial class BaseDefense : BaseEquipScript
{
    public override void OnEquipInit(EquipImplBase eq)
    {
        eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_094.png";
        eq.CardDic.TryAdd("self_pblock", 1);
        eq.CardDic.TryAdd("self_mblock", 1);
        eq.Desc = "基础防御。\n为卡组添加一张重整魔防(消耗6魔力，自身提高5点魔甲)和重整物防(消耗6魔力，自身提高5点物甲)。";
        eq.Name = "基础防御";
    }
}

public partial class MagicBook : BaseEquipScript
{
    public override void OnEquipInit(EquipImplBase eq)
    {
        eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_225.png";
        eq.Desc = "增加3点效率。添加2张暗影魔弹和1张法力狂潮到卡组中。";
        eq.Name = "魔导书";
        eq.CardDic.TryAdd("shadow_shot", 2);
        eq.CardDic.TryAdd("mana_tide", 1);
        eq.Binder.Symbol.TryAdd("EffeciencyAddProp", 3);
    }
}

public partial class TestEquip1 : BaseEquipScript
{
    public override void OnEquipInit(EquipImplBase eq)
    {
        eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_094.png";
    }
}

public partial class WoodSword : BaseEquipScript
{
    public override void OnEquipInit(EquipImplBase eq)
    {
        eq.TextureUrl = $"res://Mods/MainPackage/Resources/Icons/icons_078.png";
        eq.Desc = "增加5点力量";
        eq.Binder.Symbol.TryAdd("StrengthAddProp", 5);
        eq.Name = "木剑";
    }
}