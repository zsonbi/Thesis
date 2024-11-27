using System;

namespace DataTypes
{
    /// <summary>
    /// Event for if the task was closed
    /// </summary>
    public class TaskClosedEventArgs : EventArgs
    {
        public bool DoesItNeedUpdating;

        public TaskClosedEventArgs(bool doesItNeedUpdating)
        {
            this.DoesItNeedUpdating = doesItNeedUpdating;
        }
    }
}