using System;

namespace PriorityQueueWebAPI.Models
{
    public class Job
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public int Hours { get; set; }
        public bool Completed { get; set; }
        public DateTimeOffset Entered { get; set; }
        public DateTimeOffset Started { get; set; }
        public DateTimeOffset Finished { get; set; }
        public Guid NextJob { get; set; }
        public Guid PreviousJob { get; set; }
        public Guid Customer { get; set; }
        public Guid AssignedBy { get; set; }
        public Guid Technician { get; set; }
    }
}