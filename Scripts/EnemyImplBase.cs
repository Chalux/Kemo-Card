using NLua;
using System.Collections.Generic;

namespace KemoCard.Scripts
{
    public partial class EnemyImplBase : IEvent
    {
        public EnemyRole Binder;
        public Dictionary<string, float> GlobalDict { get; set; } = new();
        public Dictionary<string, float> InGameDict { get; set; } = new();

        public int Speed { get; set; } = 1;
        public int Strength { get; set; } = 1;
        public int Effeciency { get; set; } = 1;
        public int Mantra { get; set; } = 1;
        public int CraftBook { get; set; } = 1;
        public int CraftEquip { get; set; } = 1;
        public int Critical { get; set; } = 1;
        public int Dodge { get; set; } = 1;
        public string Name { get; set; } = "未命名";
        public LuaFunction Action;
        public Dictionary<string, List<LuaFunction>> EventDic { get; set; } = new();

        public virtual void ReceiveEvent(string @event, dynamic datas)
        {
            if (EventDic.ContainsKey(@event))
            {
                EventDic[@event].ForEach(function =>
                {
                    function.Call(datas);
                });
            }
        }

        public void AddEvent(string @event, LuaFunction func)
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
                list.ForEach(function =>
                {
                    function.Dispose();
                });
                list.Clear();
            }
            EventDic = null;
            Binder = null;
        }
    }
}
