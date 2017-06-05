using System;

namespace foundation
{
    public interface ISpecialEventInterests:IEventInterester
    {
        ASDictionary<string,Action<EventX>> specialEventInterests
        {
            get;
        }
    }
}
