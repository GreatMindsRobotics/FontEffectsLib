using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace FontEffectsLib.CoreTypes
{
    /// <summary>
    /// A game component that allows scheduling of tasks to execute at a later time than the method call. Relies on consistent updates with accurate GameTimes.
    /// </summary>
    public class TaskSchedulerComponent : Microsoft.Xna.Framework.GameComponent
    {
        public TaskSchedulerComponent(Game game)
            : base(game)
        {
        }

        private struct ScheduledTaskInfo
        {
            public Delegate Delegate;
            public Object UserState;
            public Boolean HasUserState;
            public bool IsRepeating;

            public void RunDelegate()
            {
                if (HasUserState)
                {
                    Delegate.DynamicInvoke(UserState);
                }
                else
                {
                    Delegate.DynamicInvoke();
                }
            }
        }

        private Dictionary<DateTime, List<ScheduledTaskInfo>> _scheduledTasks = new Dictionary<DateTime, List<ScheduledTaskInfo>>();

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        private void ScheduleTask(DateTime time, ScheduledTaskInfo task)
        {
            List<ScheduledTaskInfo> list;
            if(!_scheduledTasks.TryGetValue(time, out list))
            {
                list = new List<ScheduledTaskInfo>();
            }

            list.Add(task);

            _scheduledTasks[time] = list;
        }

        /// <summary>
        /// Schedule a task to execute at a future time once.
        /// </summary>
        /// <param name="time">The time from now at which to execute the task.</param>
        /// <param name="task">The task to execute.</param>
        public void ScheduleFutureTask(TimeSpan time, Delegate task)
        {
            ScheduleTask(DateTime.Now + time, new ScheduledTaskInfo() { Delegate = task, HasUserState = false, IsRepeating = false });
        }

        /// <summary>
        /// Schedule a task to execute at a future time once.
        /// </summary>
        /// <param name="time">The time from now at which to execute the task.</param>
        /// <param name="task">The task to execute.</param>
        /// <param name="userState">The user state to pass to the delegate task.</param>
        public void ScheduleFutureTask(TimeSpan time, Delegate task, object userState)
        {
            ScheduleTask(DateTime.Now + time, new ScheduledTaskInfo() { Delegate = task, HasUserState = true, UserState = userState, IsRepeating = false });
        }

        /// <summary>
        /// Allows the game component to update itself, running any scheduled tasks that are due to be ran.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            List<DateTime> removedTimes = new List<DateTime>();
            foreach (var scheduledList in _scheduledTasks)
            {
                if (scheduledList.Key < DateTime.Now)
                {
                    return;
                }

                // If the time at which it was scheduled is either now or after now (late, but not intended), execute it

                foreach (ScheduledTaskInfo info in scheduledList.Value)
                {
                    info.RunDelegate();

                    if (info.IsRepeating)
                    {
                        // [TODO] schedule task at future interval
                        throw new NotImplementedException("Repeating tasks are not yet implemented.");
                    }
                }

                removedTimes.Add(scheduledList.Key);
                scheduledList.Value.Clear();
            }

            removedTimes.ForEach(dt => _scheduledTasks.Remove(dt));

            base.Update(gameTime);
        }
    }
}
