using System;
using System.Collections.Generic;
using Godot;

namespace KemoCard.Scripts.Events
{
    public class EventScript
    {
        public string Id { get; set; }
        public Action StartEvent;

        /// <summary>
        /// 这三个属性的数量必须相等
        /// </summary>
        public readonly List<Action> EventActions = [];

        public readonly List<string> EventDesc = [];
        public readonly List<string> EventIconPath = [];
        public string EventTitle = "";
        public string EventImgPath = "";

        public EventScript(string id)
        {
            Id = id;
            if (!Datas.Ins.EventPool.TryGetValue(id, out var value)) return;
            var script = EventFactory.CreateEvent(value.EventId);
            script?.Init(this);
        }

        public void AddEvent(string desc, string iconPath, Action action)
        {
            EventDesc.Add(desc);
            EventIconPath.Add(iconPath);
            EventActions.Add(action);
        }
    }

    public partial class BaseEventScript : RefCounted
    {
        public virtual void Init(EventScript e)
        {
        }
    }
}