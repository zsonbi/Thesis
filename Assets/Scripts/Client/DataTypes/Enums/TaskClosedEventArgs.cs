using System;

namespace DataTypes
{
    public class TaskClosedEventArgs : EventArgs
    {
        public bool DoesItNeedUpdating;

        public TaskClosedEventArgs(bool doesItNeedUpdating)
        {
            this.DoesItNeedUpdating = doesItNeedUpdating;
        }
    }
}