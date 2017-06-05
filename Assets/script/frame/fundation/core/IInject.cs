using System;
using System.Collections.Generic;

namespace foundation
{
    public interface IInject
    {
        object inject(object target);
    }
}
