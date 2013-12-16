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
    /// <typeparam name="T">An object type that supports the <see cref="IStateful"/> interface.</typeparam>
    public class StatefulSequence<T> : List<T> where T : IStateful
    {
        /// <summary>
        /// This event delegate is raised whenever any of the <see cref="IStateful"/> objects in this <see cref="T:StatefulSequence"/> reach the monitored state.
        /// </summary>
        /// <typeparam name="T">An object type that supports the <see cref="IStateful"/> interface.</typeparam>
        /// <param name="item">The <see cref="IStateful"/> object that caused the event to be raised.</param>
        public delegate void MonitoredStateReached<T>(T item);
        
        /// <summary>
        /// This event delegate is raised when all <see cref="IStateful"/> objects in this <see cref="T:StatefulSequence"/> reached their monitored states.
        /// </summary>
        public delegate void MonitoredStateReached();

        /// <summary>
        /// This event is fired whenever any of the <see cref="IStateful"/> objects in this <see cref="T:StatefulSequence"/> reach the monitored state.
        /// </summary>
        public event MonitoredStateReached<T> ItemReachedMonitoredState;

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
        /// Gets a dictionary which stores monitored states and their types. Using a <see cref="T:Dictionary"/> object ensures only one state being monitored per type.
        /// </summary>
        public Dictionary<Type, object> MonitoredStates
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

        #region List Overrides

        /// <summary>
        /// Adds an <see cref="IStateful"/> object to the end of this <see cref="T:StatefulSequence"/>
        /// </summary>
        /// <param name="item"><see cref="IStateful"/> object to add to this <see cref="T:StatefulSequence"/></param>
        public new void Add(T item)
        {
            base.Add(item);

            _sequenceCompleted.Add(false);
            item.StateChanged += new EventHandler<StateEventArgs>(item_StateChanged);
        }

        /// <summary>
        /// Adds all <see cref="IStateful"/> objects in the specified <see cref="T:IEnumerable"/> collection to the end of this <see cref="T:StatefulSequence"/>
        /// </summary>
        /// <param name="items"></param>
        public new void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
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
        public new void Insert(int index, T item)
        {
            base.Insert(index, item);

            _sequenceCompleted.Insert(index, false);
            item.StateChanged += new EventHandler<StateEventArgs>(item_StateChanged);
        }

        /// <summary>
        /// Inserts all <see cref="IStateful"/> objects in the specified <see cref="T:IEnumerable"/> collection into this <see cref="T:StatefulSequence"/> at the specified index.
        /// Current item at this position, and all subsequent items, will be shifted up by total number of newly insterted items.</param>
        /// </summary>
        /// <param name="index">The index at which to insert the new items.</param>
        /// <param name="items">The <see cref="IEnumerable"/> <see cref="Collection"/> of <see cref="IStateful"/> items to add to this <see cref="T:StatefulSequence"/> <see cref="T:List"/>.</param>
        public new void InsertRange(int index, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                this.Insert(index, item);
                index++;
            }
        }

        /// <summary>
        /// Removes the specified <see cref="IStateful"/> object from this <see cref="T:StatefulSequence"/> <see cref="T:List"/>.
        /// </summary>
        /// <param name="item">The <see cref="IStateful"/> item to remove from this <see cref="T:StatefulSequence"/> <see cref="T:List"/>.</param>
        public new void Remove(T item)
        {
            //Unsubscribe event handler
            item.StateChanged -= item_StateChanged;

            //Find index and remove from sequenceCompleted List
            int index = IndexOf((T)item);
            _sequenceCompleted.RemoveAt(index);
            
            base.Remove(item);
        }

        public new void RemoveAt(int index)
        {
            this.Remove(this[index]);            
        }

        public new void RemoveRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                this.Remove(item);
            }
        }

        public new void RemoveAll(Predicate<T> match)
        {
            foreach (T item in this.FindAll(match))
            {
                this.Remove(item);
            }
        }
        
        #endregion List Overrides

        protected void item_StateChanged(object sender, StateEventArgs e)
        {
            foreach (KeyValuePair<Type, object> monitoredState in _monitoredStates)
            {
                if (e.DataType == monitoredState.Key && e.Data.ToString() == monitoredState.Value.ToString())
                {
                    //Mark this item in SequenceCompleted List
                    int index = IndexOf((T)sender);
                    _sequenceCompleted[index] = true;

                    //Raise event notifying subscribers this item has completed the sequence
                    if (ItemReachedMonitoredState != null)
                    {
                        ItemReachedMonitoredState((T)sender);
                    }

                    //If all items completed the sequence, raise SequenceReachedMonitoredState event
                    if (SequenceReachedMonitoredState != null && _sequenceCompleted.All<bool>(b => b))
                    {
                        SequenceReachedMonitoredState();
                    }
                }
            }
        }
    }
}
