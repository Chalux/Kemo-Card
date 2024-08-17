using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Clucky : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.Cost = 0;
            c.Alias = "幸运";
            c.Desc = "获得1点行动力,耗尽,保留";
            c.TargetType = StaticEnums.TargetType.SELF;
            c.CostType = StaticEnums.CostType.ACTIONPOINT;
            c.FunctionUse = new((user, targets, datas) =>
            {
                if (c.owner is PlayerRole inFightPlayer)
                {
                    inFightPlayer.CurrentActionPoint += 1;
                }
                GD.Print($"卡牌C{c.Id}已使用");
            });
            c.InGameDict.Add("Exhaust", 1);
            c.InGameDict.Add("KeepInHand", 1);
        }
    }
}
