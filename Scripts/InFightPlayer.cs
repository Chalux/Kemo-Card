using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;
using System.Linq;

namespace KemoCard.Scripts
{
    public partial class InFightPlayer : PlayerRole
    {
        private int _CurrPBlock = 0;
        private int _CurrMBlock = 0;

        public int CurrPBlock
        {
            get => _CurrPBlock;
            set
            {
                var oldValue = _CurrPBlock;
                if (value < 0)
                {
                    _CurrPBlock = 0;
                }
                else
                {
                    _CurrPBlock = value;
                }
                object[] param = new object[] { this, "CurrPBlock", oldValue, _CurrPBlock };
                StaticInstance.eventMgr.Dispatch("PropertiesChanged", param);
            }
        }

        public int CurrMBlock
        {
            get => _CurrMBlock;
            set
            {
                var oldValue = _CurrMBlock;
                if (value < 0)
                {
                    _CurrMBlock = 0;
                }
                else
                {
                    _CurrMBlock = value;
                }
                object[] param = new object[] { this, "CurrMBlock", oldValue, _CurrPBlock };
                StaticInstance.eventMgr.Dispatch("PropertiesChanged", param);
            }
        }
        public InFightPlayer(PlayerRole playerRole) : base(playerRole.OriginSpeed, playerRole.OriginStrength, playerRole.OriginEffeciency, playerRole.OriginMantra, playerRole.OriginCraftEquip, playerRole.OriginCraftBook, playerRole.OriginCritical, playerRole.OriginDodge, playerRole.name)
        {
            Deck = playerRole.Deck.ToList();
        }

        public List<Card> InFightDeck = new();
        public List<Card> InFightHands = new();
        public List<Card> InFightGrave = new();
        public List<BuffImplBase> InFightBuffs = new();
        public int HandLimit = 10;
        public bool isIFPInited = false;
        public bool isDead = false;
        public int InFightDeckCount => InFightDeck.Count;
        public int InFightGraveCount => InFightGrave.Count;
        public PlayerRoleObject roleObject;

        public int _turnActionPoint = 0;
        public int TurnActionPoint
        {
            get => _turnActionPoint;
            set
            {
                var oldValue = _turnActionPoint;
                _turnActionPoint = value;
                OnPropertyChanged(nameof(TurnActionPoint), oldValue, _turnActionPoint);
            }
        }

        public int _currActionPoint = 0;
        public int CurrentActionPoint
        {
            get => _currActionPoint;
            set
            {
                var oldValue = _currActionPoint;
                _currActionPoint = value;
                OnPropertyChanged(nameof(CurrentActionPoint), oldValue, _currActionPoint);
            }
        }

        public void AddCardToDeck(Card card)
        {
            InFightDeck.Add(card);
            if (StaticInstance.currWindow is BattleScene bs)
            {
                if (bs.nowPlayer == this)
                {
                    bs.UpdateCounts();
                }
            }
        }

        public void RemoveCardInDeck(Card card)
        {
            InFightDeck.Remove(card);
            if (StaticInstance.currWindow is BattleScene bs)
            {
                if (bs.nowPlayer == this)
                {
                    bs.UpdateCounts();
                }
            }
        }

        public void AddCardToGrave(Card card)
        {
            InFightGrave.Add(card);
            if (StaticInstance.currWindow is BattleScene bs)
            {
                if (bs.nowPlayer == this)
                {
                    bs.UpdateCounts();
                }
            }
        }

        public void RemoveCardInGrave(Card card)
        {
            InFightGrave.Remove(card);
            if (StaticInstance.currWindow is BattleScene bs)
            {
                if (bs.nowPlayer == this)
                {
                    bs.UpdateCounts();
                }
            }
        }

        public void ShuffleGrave()
        {
            StaticUtils.ShuffleArray(InFightGrave);
        }

        public void ShuffleDeck()
        {
            StaticUtils.ShuffleArray(InFightDeck);
        }

        public void InitFighter()
        {
            InFightDeck = Deck.ToList();
            InFightDeck.ForEach(card =>
            {
                card.InGameDict.Clear();
                card.owner = this;
            });
            StaticUtils.ShuffleArray(InFightDeck);
            InFightGrave = new();
            isIFPInited = true;
        }

        public void EndFight()
        {
            InFightDeck?.Clear();
            InFightHands?.Clear();
            InFightGrave?.Clear();
            isIFPInited = false;
        }
    }
}
