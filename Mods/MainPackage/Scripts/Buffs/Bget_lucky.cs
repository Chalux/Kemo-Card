using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using KemoCard.Scripts.Cards;
using StaticClass;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class Bget_lucky : BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase b)
        {
            b.AddEvent("StartBattle", new((datas) =>
            {
                if (StaticInstance.currWindow is BattleScene bs)
                {
                    if (BattleStatic.isFighting)
                    {
                        if (b.Binder is PlayerRole inFightPlayer)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Card card = new("lucky")
                                {
                                    owner = inFightPlayer
                                };
                                inFightPlayer.InFightHands.Add(card);
                                if (inFightPlayer == bs.nowPlayer)
                                {
                                    CardObject cardObject = ResourceLoader.Load<PackedScene>("res://Pages/CardObject.tscn").Instantiate<CardObject>();
                                    cardObject.InitData(card);
                                    bs.HandControl.AddChild(cardObject);
                                }
                            }
                        }
                    }
                }
            }));
        }
    }
}
