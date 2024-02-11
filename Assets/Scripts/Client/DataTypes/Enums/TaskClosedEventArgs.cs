using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskClosedEventArgs : EventArgs
{
    public bool DoesItNeedUpdating;

    public TaskClosedEventArgs(bool doesItNeedUpdating)
    {
        this.DoesItNeedUpdating = doesItNeedUpdating;
    }
}