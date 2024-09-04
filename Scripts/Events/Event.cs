using Godot;
using System;
using System.Collections.Generic;

namespace KemoCard.Scripts.Events
{
    public class Event
    {
        public string Id { get; set; }
        public Action StartEvent;
        /// <summary>
        /// 这三个属性的数量必须相等
        /// </summary>
        public List<Action> EventActions = new();
        public List<string> EventDesc = new();
        public List<string> EventIconPath = new();
        public string EventTitle = "";
        public string EventImgPath = "";
        public Event(string id)
        {
            Id = id;
            if (Datas.Ins.EventPool.ContainsKey(id))
            {
                Datas.EventStruct e = Datas.Ins.EventPool[id];
                using CSharpScript script = ResourceLoader.Load<CSharpScript>($"res://Mods/{e.mod_id}/Scripts/Events/E{e.event_id}.cs");
                if (script != null)
                {
                    using BaseEventScript baseEventScript = script.New().As<BaseEventScript>();
                    baseEventScript.Init(this);
                }
            }
        }

        public void AddEvent(string Desc, string IconPath, Action action)
        {
            EventDesc.Add(Desc);
            EventIconPath.Add(IconPath);
            EventActions.Add(action);
        }
    }

    public partial class BaseEventScript : RefCounted
    {
        public virtual void Init(Event e)
        {
        }
    }
}
