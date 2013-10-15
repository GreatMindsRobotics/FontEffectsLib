using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FontEffectsLib.CoreTypes
{
    public interface IStateful
    {
        event EventHandler<StateEventArgs> StateChanged;
    }
}
