using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using KemoCard.Scripts.Cards;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class B10003 : BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase b)
        {
            b.BuffId = 10003;
            b.BuffName = "get_lucky";
            b.BuffShowname = "幸运之人";
            b.IsInfinite = true;
            b.Desc = "每次战斗开始时，将2张幸运(0行动力,获得1点行动力,保留,耗尽)加入手牌。";
            b.IconPath = "res://Mods/MainPackage/Resources/Icons/Skillicon14_25.png";
            b.AddEvent("StartBattle", new((datas) =>
            {
                if (StaticInstance.currWindow is BattleScene bs)
                {
                    if (bs.isFighting)
                    {
                        if (b.Binder is InFightPlayer inFightPlayer)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Card card = new(10007)
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
