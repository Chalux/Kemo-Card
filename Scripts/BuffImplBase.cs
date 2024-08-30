using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using static StaticClass.StaticEnums;

namespace KemoCard.Scripts
{
    public partial class BuffImplBase : IEvent
    {
        public string BuffId { get; set; }
        private int _buffCount = 1;
        public string IconPath { get; set; } = "";
        // 临时的属性修改都放在这里面，只有永久修改的才写代码改属性值
        public Dictionary<string, float> Symbol { get; set; } = new();
        public int BuffCount
        {
            get => _buffCount;
            set
            {
                _buffCount = value;
                if (value <= 0 && !CustomBuffCountCalculate)
                {
                    RemoveThisFromBinder();
                }
                else
                    BuffObj?.Update();
            }
        }
        public bool CustomBuffCountCalculate { get; set; } = false;
        private int _buffValue = 0;
        public int BuffValue
        {
            get => _buffValue;
            set
            {
                _buffValue = value;
                BuffObj?.Update();
            }
        }
        public bool CanStack { get; set; } = false;
        public bool IsInfinite { get; set; } = false;
        public HashSet<string> tags = new();
        public StaticEnums.EffectTriggerTiming CountTriTiming { get; set; } = StaticEnums.EffectTriggerTiming.ON_TURN_END;
        [JsonIgnore]
        public object Binder;
        [JsonIgnore]
        public BuffObject BuffObj;
        public string BuffShowname { get; set; } = "未命名";
        public void ReceiveEvent(string @event, params object[] datas)
        {
            if (EventDic.ContainsKey(@event)) EventDic[@event]?.ForEach(function => function.Invoke(datas));
            CheckCountNeedMinus(datas);
        }
        [JsonIgnore]
        public Dictionary<string, List<Action<dynamic>>> EventDic { get; set; } = new();

        [JsonIgnore]
        public Action<BuffImplBase> OnBuffAdded;
        [JsonIgnore]
        public Action<BuffImplBase> OnBuffRemoved;
        ~BuffImplBase()
        {
            OnBuffAdded = OnBuffRemoved = null;
            foreach (var list in EventDic.Values)
            {
                list.Clear();
            }
            if (Binder is BaseRole br) br?.Buffs.Remove(this);
            Binder = null;
            if (BuffObj != null) BuffObj.data = null;
            BuffObj = null;
        }
        public void CheckCountNeedMinus(params object[] datas)
        {
            if (CustomBuffCountCalculate || !(datas != null && datas.Length > 0 && datas[0] is EffectTriggerTiming timing && timing == CountTriTiming)) { return; }
            BuffCount -= 1;
            if (BuffCount <= 0)
            {
                RemoveThisFromBinder();
            }
        }

        public void RemoveThisFromBinder()
        {
            object tempBinder = Binder;
            if (Binder is BaseRole br) br.Buffs.Remove(this);
            if (Binder is PlayerRole ifp)
            {
                ifp.InFightBuffs.Remove(this);
                if (ifp.roleObject != null)
                {
                    foreach (BuffObject i in ifp.roleObject.buffContainer.GetChildren().Cast<BuffObject>())
                    {
                        if (i.data == this)
                        {
                            i.data = null;
                            Binder = null;
                            i.QueueFree();
                            break;
                        }
                    }
                }
            }
            else if (Binder is EnemyRole em)
            {
                em.InFightBuffs.Remove(this);
                if (em.roleObject != null)
                {
                    foreach (BuffObject i in em.roleObject.buffContainer.GetChildren().Cast<BuffObject>())
                    {
                        if (i.data == this)
                        {
                            i.data = null;
                            Binder = null;
                            i.QueueFree();
                            break;
                        }
                    }
                }
            }
            OnBuffRemoved?.Invoke(this);
            StaticInstance.eventMgr.Dispatch("BuffChanged", tempBinder);
        }

        public string Desc { get; set; } = "什么都没有";

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
    }
}
