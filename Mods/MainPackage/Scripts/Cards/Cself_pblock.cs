using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cself_pblock : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.Cost = 6;
            c.Alias = "重整护甲";
            c.Desc = "自身/护甲+8";
            c.TargetType = StaticClass.StaticEnums.TargetType.SELF;
            c.CostType = StaticClass.StaticEnums.CostType.MAGIC;
            c.FunctionUse = new((user, targets, datas) =>
            {
                foreach (var target in targets)
                {
                    if (target != null && target is PlayerRole ifp)
                    {
                        ifp.CurrPBlock += 5;
                    }
                }
                GD.Print($"卡牌C{c.Id}已使用");
            });
        }
    }
}
