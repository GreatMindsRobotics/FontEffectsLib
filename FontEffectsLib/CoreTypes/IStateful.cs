using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FontEffectsLib.CoreTypes
{
    public interface IStateful
    {
        /// <summary>
        /// An event raised when the state of this object changes.
        /// </summary>
        event EventHandler<StateEventArgs> StateChanged;
    }
}
