using StaticClass;
using System;
using System.Collections.Generic;

namespace KemoCard.Scripts
{
    public partial class EnemyImplBase : IEvent
    {
        public EnemyRole Binder;
        public Dictionary<string, float> GlobalDict { get; set; } = new();
        public Dictionary<string, float> InGameDict { get; set; } = new();
        public string AnimationResourcePath { get; set; } = "";
        public int Speed { get; set; } = 1;
        public int Strength { get; set; } = 1;
        public int Effeciency { get; set; } = 0;
        public int Mantra { get; set; } = 0;
        public int CraftBook { get; set; } = 0;
        public int CraftEquip { get; set; } = 0;
        public int Critical { get; set; } = 0;
        public int Dodge { get; set; } = 0;
        public string Name { get; set; } = "未命名";
        public string Intent { get; set; } = "";
        public Action<int, List<PlayerRole>, List<EnemyRole>, EnemyImplBase> ActionFunc;
        public Dictionary<string, List<Action<dynamic>>> EventDic { get; set; } = new();

        public virtual void ReceiveEvent(string @event, params object[] datas)
        {
            if (EventDic.ContainsKey(@event))
            {
                EventDic[@event].ForEach(function =>
                {
                    function.Invoke(datas);
                });
            }
        }

        public void AddEvent(string @event, Action<dynamic> func)
        {
            if (EventDic.ContainsKey(@event))
            {
                EventDic[@event].Add(func);
            }
            else
            {
                EventDic[@event] = new()
                {
                    func
                };
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

        public void ChangeIntent(string Intent)
        {
            if (Binder != null && Binder.roleObject != null) Binder.roleObject.IntentRichLabel.Text = StaticUtils.MakeBBCodeString(Intent);
            this.Intent = Intent;
        }
    }
}
