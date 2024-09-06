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
            c.FunctionUse = new((user, targets, datas) =>
            {
                if (c.owner is PlayerRole inFightPlayer)
                {
                    inFightPlayer.CurrentActionPoint += 1;
                }
            });
            c.GlobalDict.Add("Exhaust", 1);
            c.GlobalDict.Add("KeepInHand", 1);
        }
    }
}
