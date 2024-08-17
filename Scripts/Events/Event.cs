using Godot;
using System;

namespace KemoCard.Scripts.Events
{
    public class Event
    {
        public string Id { get; set; }
        public Action StartEvent;
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
    }

    public partial class BaseEventScript : RefCounted
    {
        public virtual void Init(Event e)
        {
        }
    }
}
