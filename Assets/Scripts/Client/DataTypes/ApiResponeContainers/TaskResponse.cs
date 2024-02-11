using System.Collections.Generic;


public class TaskApiResponse
{
    public string Res { get; private set; }
    public string Error { get; private set; }
    public List<TaskContainer> Tasks { get; private set; }

    public TaskApiResponse(string res, List<TaskContainer> tasks, string error = "")
    {
        Res = res;
        Tasks = tasks;
        Error = error;
    }
}