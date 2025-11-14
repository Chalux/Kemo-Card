using System;
using System.Collections.Generic;

namespace KemoCard.Scripts
{
    public class EnemyImplBase : IEvent
    {
        public EnemyRole Binder;
        public Dictionary<string, float> GlobalDict { get; set; } = new();
        public Dictionary<string, float> InGameDict { get; set; } = new();
        public string AnimationResourcePath { get; set; } = "";
        public int Speed { get; set; } = 1;
        public int Strength { get; set; } = 1;
        public int Effeciency { get; set; }
        public int Mantra { get; set; }
        public int CraftBook { get; set; }
        public int CraftEquip { get; set; }
        public int Critical { get; set; }
        public int Dodge { get; set; }
        public string Name { get; set; } = "未命名";
        public string Intent { get; set; } = "";
        public Action<int, List<PlayerRole>, List<EnemyRole>, EnemyImplBase> ActionFunc;
        public Dictionary<string, List<Action<EnemyImplBase, dynamic>>> EventDic { get; set; } = new();

        public virtual void ReceiveEvent(string @event, params object[] datas)
        {
            if (EventDic.TryGetValue(@event, out var value))
            {
                value.ForEach(function => { function.Invoke(this, datas); });
            }
        }

        public void AddEvent(string @event, Action<EnemyImplBase, dynamic> func)
        {
            if (EventDic.TryGetValue(@event, out var value))
            {
                value.Add(func);
            }
            else
            {
                EventDic[@event] = [func];
            }
        }

        ~EnemyImplBase()
        {
            GlobalDict?.Clear();
            InGameDict?.Clear();
            GlobalDict = null;
            InGameDict = null;
            foreach (var list in EventDic.Values)
            {
                list.Clear();
            }

            EventDic = null;
            Binder = null;
        }

        public void ChangeIntent(string intent)
        {
            if (Binder is { RoleObject: not null })
                Binder.RoleObject.IntentRichLabel.Text = StaticUtils.MakeBBCodeString(intent);
            this.Intent = intent;
        }
    }
}