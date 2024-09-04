using System.Collections.Generic;

namespace KemoCard.Scripts
{
    public class EventMgr
    {
        private HashSet<IEvent> events;

        public EventMgr()
        {
            events = new HashSet<IEvent>();
        }

        ~EventMgr()
        {
            Reset();
        }

        public void Dispatch(string @event, params object[] datas)
        {
            foreach (var i in events)
            {
                i.ReceiveEvent(@event, datas);
            }
        }

        public void RegistIEvent(IEvent obj)
        {
            events.Add(obj);
        }

        public void Reset()
        {
            events.Clear();
        }

        public void UnregistIEvent(IEvent obj)
        {
            events.Remove(obj);
        }
    }

    public interface IEvent
    {
        public abstract void ReceiveEvent(string @event, params object[] datas);
    }
}
