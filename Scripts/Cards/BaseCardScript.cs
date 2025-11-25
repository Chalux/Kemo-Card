using System.Collections.Generic;
using Godot;
using KemoCard.Pages;

namespace KemoCard.Scripts.Cards
{
    public partial class BaseCardScript : RefCounted
    {
        public virtual void OnCardScriptInit(Card self)
        {
        }

        public virtual void UseFunction(Card self, BaseRole owner, List<BaseRole> targets, params object[] datas)
        {
        }

        public virtual void DiscardAction(Card self, BaseRole owner, BattleScene.DisCardReason reason, BaseRole from)
        {
        }

        public virtual bool UseFilter(Card self, BaseRole owner, List<BaseRole> targets, params dynamic[] datas)
        {
            return true;
        }
    }
}