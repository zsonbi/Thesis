using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thesis_backend.Data_Structures
{
    public struct TaskRequest
    {
        public string TaskName { get; set; }
        public string Description { get; set; }
        public int PeriodRate { get; set; }
        public bool TaskType { get; set; }
    }
}