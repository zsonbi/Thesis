
namespace Thesis_backend.Data_Structures
{
    /// <summary>
    /// Task create and update request object
    /// </summary>
    public struct TaskRequest
    {
        /// <summary>
        /// The name of the task
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// The task description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The task period rate
        /// </summary>
        public int PeriodRate { get; set; }
        /// <summary>
        /// The task's type
        /// </summary>
        public bool TaskType { get; set; }
    }
}