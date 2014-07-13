using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FontEffectsLib.CoreTypes
{
    public class StateEventArgs : EventArgs
    {
        protected Type _dataType;
        protected object _data;

        /// <summary>
        /// Gets or sets the type of the conveyed state information.
        /// </summary>
        public Type DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        /// <summary>
        /// Gets or sets the new state data represented by this event.
        /// </summary>
        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public StateEventArgs(Type stateType, object data)
        {
            _dataType = stateType;
            _data = data;
        }

    }
}
