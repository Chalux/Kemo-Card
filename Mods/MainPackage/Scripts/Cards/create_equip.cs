using KemoCard.Scripts.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class create_equip : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.Cost = 1;
            c.Alias = "创物";
            c.Desc = $"消耗。从2张牌中选择1张牌临时加入你的牌组。敌方单体造成(4*工艺)点无属性物理伤害/获得(3*工艺)点护甲";
            c.TargetType = StaticClass.StaticEnums.TargetType.SELF;
            c.CostType = StaticClass.StaticEnums.CostType.ACTIONPOINT;
            c.FunctionUse = new((user, targets, datas) =>
            {

            });
        }
    }
}
