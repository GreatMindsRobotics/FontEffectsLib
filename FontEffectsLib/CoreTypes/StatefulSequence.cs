using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FontEffectsLib.CoreTypes
{
    /// <summary>
    /// A <see cref="T:List"/> of <see cref="IStateful"/> objects that can notify via events when any object in the list reached a particular monitored state.
    /// This class will also notify subscribers when all objects in the list reached their monitored states.
    /// </summary>
    /// <typeparam name="TTracked">An object type that supports the <see cref="IStateful"/> interface, which this is a collection of.</typeparam>
    public class StatefulSequence<TTracked> : List<TTracked> where TTracked : IStateful
    {
        /// <summary>
        /// This event delegate is raised whenever any of the <see cref="IStateful"/> objects in this <see cref="T:StatefulSequence"/> reach the monitored state.
        /// </summary>
        /// <typeparam name="T">An object type that supports the <see cref="IStateful"/> interface.</typeparam>
        /// <param name="item">The <see cref="IStateful"/> object that caused the event to be raised.</param>
        public delegate void MonitoredStateReached<T>(T item) where T : IStateful;

        /// <summary>
        /// This event delegate is raised when all <see cref="IStateful"/> objects in this <see cref="T:StatefulSequence"/> reached their monitored states.
        /// </summary>
        public delegate void MonitoredStateReached();

        /// <summary>
        /// This event is fired whenever any of the <see cref="IStateful"/> objects in this <see cref="T:StatefulSequence"/> reach the monitored state.
        /// </summary>
        public event MonitoredStateReached<TTracked> ItemReachedMonitoredState;

        /// <summary>
        /// This event is fired when all <see cref="IStateful"/> objects in this <see cref="T:StatefulSequence"/> reached their monitored states.
        /// </summary>
        public event MonitoredStateReached SequenceReachedMonitoredState;

        /// <summary>
        /// Stores <see cref="T:KeyValuePair"/>s of monitored states and their types. Using a <see cref="T:Dictionary"/> object ensures only one state being monitored per type.
        /// </summary>
        protected Dictionary<Type, object> _monitoredStates = new Dictionary<Type, object>();

        /// <summary>
        /// Stores boolean values in corresponding positions to the <see cref="T:List"/> of <see cref="IStateful"/> objects.
        /// </summary>
        protected List<bool> _sequenceCompleted = new List<bool>();

        /// <summary>
        /// Gets a dictionary which stores monitored states and their types.
        /// Using an <see cref="T:IDictionary"/> implementation ensures only one state being monitored per type.
        /// </summary>
        public IDictionary<Type, object> MonitoredStates
        {
            get { return _monitoredStates; }
        }

        /// <summary>
        /// Creates a new <see cref="T:StatefulSequence"/> <see cref="T:List"/>. At least one pair of monitored State and Type is required.
        /// More can be added to the <see cref="MonitoredStates"/> <see cref="T:Dictionary"/>
        /// </summary>
        /// <param name="monitorState">State value to monitor for. Best choice is <see cref="T:enum"/> values.</param>
        /// <param name="stateType">Type of value being monitored, Example: typeof([enum]).</param>
        public StatefulSequence(object monitorState, Type stateType)
            : base()
        {
            _monitoredStates.Add(stateType, monitorState);
        }

        /// <summary>
        /// Creates a new <see cref="T:StatefulSequence"/> <see cref="T:List"/>, specifying multiple monitored states.
        /// </summary>
        /// <param name="stateTypes">State values to monitor for. Best choice is <see cref="T:enum"/> values.</param>
        public StatefulSequence(params KeyValuePair<Type, object>[] stateTypes)
            : base()
        {
            if (stateTypes == null || stateTypes.Length < 1)
            {
                throw new ArgumentException("At least one state monitor target must be specified.");
            }

            foreach (var monitor in stateTypes)
            {
                _monitoredStates.Add(monitor.Key, monitor.Value);
            }
        }

        #region List Overrides

        /// <summary>
        /// Adds an <see cref="IStateful"/> object to the end of this <see cref="T:StatefulSequence"/>
        /// </summary>
        /// <param name="item"><see cref="IStateful"/> object to add to this <see cref="T:StatefulSequence"/></param>
        public new void Add(TTracked item)
        {
            base.Add(item);

            _sequenceCompleted.Add(false);
            item.StateChanged += new EventHandler<StateEventArgs>(item_StateChanged);
        }

        /// <summary>
        /// Adds all <see cref="IStateful"/> objects in the specified <see cref="T:IEnumerable"/> collection to the end of this <see cref="T:StatefulSequence"/>
        /// </summary>
        /// <param name="items"></param>
        public new void AddRange(IEnumerable<TTracked> items)
        {
            foreach (TTracked item in items)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Inserts a new <see cref="IStateful"/> object into this <see cref="T:StatefulSequence"/> <see cref="T:List"/> at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index at which to insert this <see cref="IStateful"/> item. 
        /// Current item at this position, and all subsequent items, will be shifted up by one.</param>
        /// <param name="item">The <see cref="IStateful"/> item to insert. </param>
        public new void Insert(int index, TTracked item)
        {
            base.Insert(index, item);

            _sequenceCompleted.Insert(index, false);
            item.StateChanged += new EventHandler<StateEventArgs>(item_StateChanged);
        }

        /// <summary>
        /// Inserts all <see cref="IStateful"/> objects in the specified <see cref="T:IEnumerable"/> collection into this <see cref="T:StatefulSequence"/> at the specified index.
        /// Current item at this position, and all subsequent items, will be shifted up by total number of newly insterted items.
        /// </summary>
        /// <param name="index">The index at which to insert the new items.</param>
        /// <param name="items">The <see cref="IEnumerable"/> <see cref="Collection"/> of <see cref="IStateful"/> items to add to this <see cref="T:StatefulSequence"/> <see cref="T:List"/>.</param>
        public new void InsertRange(int index, IEnumerable<TTracked> items)
        {
            foreach (TTracked item in items)
            {
                this.Insert(index, item);
                index++;
            }
        }

        /// <summary>
        /// Removes the specified <see cref="IStateful"/> object from this <see cref="T:StatefulSequence"/> <see cref="T:List"/>.
        /// </summary>
        /// <param name="item">The <see cref="IStateful"/> item to remove from this <see cref="T:StatefulSequence"/> <see cref="T:List"/>.</param>
        public new void Remove(TTracked item)
        {
            //Unsubscribe event handler
            item.StateChanged -= item_StateChanged;

            //Find index and remove from sequenceCompleted List
            int index = IndexOf((TTracked)item);
            _sequenceCompleted.RemoveAt(index);

            base.Remove(item);
        }

        public new void RemoveAt(int index)
        {
            this.Remove(this[index]);
        }

        public void RemoveRange(IEnumerable<TTracked> items)
        {
            foreach (TTracked item in items)
            {
                this.Remove(item);
            }
        }

        public new void RemoveAll(Predicate<TTracked> match)
        {
            foreach (TTracked item in this.FindAll(match))
            {
                this.Remove(item);
            }
        }

        #endregion List Overrides

        private sealed class StringEqualityComparer : IEqualityComparer<Object>
        {
            static StringEqualityComparer()
            {
                _inst = new StringEqualityComparer();
            }

            private StringEqualityComparer() { }

            private static StringEqualityComparer _inst;

            public static StringEqualityComparer Instance
            {
                get
                {
                    return _inst;
                }
            }

            public new bool Equals(object x, object y)
            {
                if (x == null)
                {
                    return y == null;
                }

                if (y == null)
                {
                    return x == null;
                }

                return x.ToString() == y.ToString();
            }

            public int GetHashCode(object obj)
            {
                return obj == null ? 0 : obj.GetHashCode();
            }
        }

        private IEqualityComparer<object> _stateEqualityComparer = StringEqualityComparer.Instance;

        /// <summary>
        /// Gets or sets an equality comparer that is used for comparing object states.
        /// When this comparer determines that an object's state has changed to its monitored state, the appropriate events will be fired and actions will be taken.
        /// If this value is set to null, the default state equality comparer will be used.
        /// The default state equality comparer compares state objects by comparing their string implementations and delegating the hash code computation to the objects themselves.
        /// The hash code functionality of this object is not used internally.
        /// </summary>
        public IEqualityComparer<object> StateEqualityComparer
        {
            get { return _stateEqualityComparer; }
            set { _stateEqualityComparer = value == null ? StringEqualityComparer.Instance : value; }
        }


        protected void item_StateChanged(object sender, StateEventArgs e)
        {
            object monitorData;

            if (!_monitoredStates.TryGetValue(e.DataType, out monitorData) || !StateEqualityComparer.Equals(e.Data, monitorData))
            {
                return;
            }

            //Mark this item in SequenceCompleted List
            int index = IndexOf((TTracked)sender);
            _sequenceCompleted[index] = true;

            //Raise event notifying subscribers this item has completed the sequence
            MonitoredStateReached<TTracked> monitoredStateReached = ItemReachedMonitoredState;
            if (monitoredStateReached != null)
            {
                monitoredStateReached((TTracked)sender);
            }

            //If all items completed the sequence, raise SequenceReachedMonitoredState event
            MonitoredStateReached sequenceReachedMonitoredState = SequenceReachedMonitoredState;
            if (sequenceReachedMonitoredState != null && _sequenceCompleted.All<bool>(b => b))
            {
                sequenceReachedMonitoredState();
            }
        }
    }
}
