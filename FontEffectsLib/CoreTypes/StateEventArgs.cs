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

        public Type DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

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
