using System.Collections.Generic;

namespace KemoCard.Scripts
{
    public class EventMgr
    {
        private readonly HashSet<IEvent> _events = [];

        ~EventMgr()
        {
            Reset();
        }

        public void Dispatch(string @event, params object[] datas)
        {
            foreach (var i in _events)
            {
                i.ReceiveEvent(@event, datas);
            }
        }

        public void RegisterIEvent(IEvent obj)
        {
            _events.Add(obj);
        }

        private void Reset()
        {
            _events.Clear();
        }

        public void UnregisterIEvent(IEvent obj)
        {
            _events.Remove(obj);
        }
    }

    public interface IEvent
    {
        public void ReceiveEvent(string @event, params object[] datas);
    }
}