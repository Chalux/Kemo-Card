using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class C10004 : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.Cost = 6;
            c.CardName = "重整魔甲";
            c.Desc = "自身/魔甲+8";
            c.TargetType = StaticClass.StaticEnums.TargetType.SELF;
            c.CostType = StaticClass.StaticEnums.CostType.MAGIC;
            c.FunctionUse = new((user, targets, datas) =>
            {
                foreach (var target in targets)
                {
                    if (target != null && target is InFightPlayer ifp)
                    {
                        ifp.CurrMBlock += 5;
                    }
                }
                GD.Print($"卡牌C{c.Id}已使用");
            });
        }
    }
}
