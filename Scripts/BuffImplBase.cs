using Godot;
using KemoCard.Scripts.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts
{
    public class BuffImplBase : IEvent
    {
        public BuffImplBase()
        {
        }

        public BuffImplBase(string id)
        {
            if (!Datas.Ins.BuffPool.TryGetValue(id, out var modInfo))
            {
                var hint = $"读取id为{id}的Buff时出错。";
                GD.PrintErr(hint);
                StaticInstance.MainRoot.ShowBanner(hint);
                return;
            }

            BuffId = id;
            BuffShowname = modInfo.ShowName;
            IsInfinite = modInfo.IsInfinite;
            Desc = modInfo.Desc;
            IconPath = modInfo.IconPath;
            IsDebuff = modInfo.IsDebuff;
            BuffCount = modInfo.BuffCount;
            BuffValue = modInfo.BuffValue;
            var script = BuffFactory.CreateBuff(id);
            script?.OnBuffInit(this);
        }

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
        private double _buffValue;
        [JsonIgnore] public object Creator { get; set; } = null;
        public string CreatorId { get; set; } = "";

        public double BuffValue
        {
            get => _buffValue;
            set
            {
                _buffValue = value;
                BuffObj?.Update();
            }
        }

        public bool CanStack { get; set; } = false;
        public bool IsInfinite { get; set; }
        public bool IsDebuff { get; set; }
        public HashSet<string> tags = [];
        public EffectTriggerTiming CountTriTiming { get; set; } = EffectTriggerTiming.OnTurnEnd;
        [JsonIgnore] public object Binder;
        [JsonIgnore] public BuffObject BuffObj;
        public string BuffShowname { get; set; } = "未命名";

        public void ReceiveEvent(string @event, params object[] datas)
        {
            if (EventDic.TryGetValue(@event, out var value))
                value?.ForEach(function => function.Invoke(this, datas));
            CheckCountNeedMinus(datas);
        }

        [JsonIgnore] private Dictionary<string, List<Action<BuffImplBase, dynamic>>> EventDic { get; set; } = new();

        [JsonIgnore] public Action<BuffImplBase> OnBuffAdded;
        [JsonIgnore] public Action<BuffImplBase> OnBuffRemoved;

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
            if (CustomBuffCountCalculate || !(datas is { Length: > 0 } &&
                                              datas[0] is EffectTriggerTiming timing && (timing & CountTriTiming) > 0))
            {
                return;
            }

            BuffCount -= 1;
            if (BuffCount <= 0)
            {
                RemoveThisFromBinder();
            }
        }

        public void RemoveThisFromBinder()
        {
            var tempBinder = Binder;
            switch (Binder)
            {
                case BaseRole br when br.Buffs.Contains(this):
                    Binder = null;
                    br.Buffs.Remove(this);
                    break;
                case PlayerRole ifp:
                {
                    ifp.InFightBuffs.Remove(this);
                    if (ifp.RoleObject != null)
                    {
                        foreach (var i in ifp.RoleObject?.buffContainer?.GetChildren()?.Cast<BuffObject>() ?? [])
                        {
                            if (i?.data != this) continue;
                            i.data = null;
                            Binder = null;
                            i.QueueFree();
                            break;
                        }
                    }

                    break;
                }
                case EnemyRole em:
                {
                    em.InFightBuffs.Remove(this);
                    if (em.RoleObject != null)
                    {
                        foreach (var i in em?.RoleObject?.buffContainer?.GetChildren().Cast<BuffObject>() ?? [])
                        {
                            if (i?.data != this) continue;
                            i.data = null;
                            Binder = null;
                            i.QueueFree();
                            break;
                        }
                    }

                    break;
                }
            }

            StaticInstance.EventMgr.Dispatch("BeforeBuffChange", Binder, this, "Remove");
            OnBuffRemoved?.Invoke(this);
            StaticInstance.EventMgr.Dispatch("BuffChanged", tempBinder);
        }

        public string Desc { get; set; } = "什么都没有";

        public void AddEvent(string @event, Action<BuffImplBase, dynamic> func)
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
    }
}