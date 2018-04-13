using System;

namespace PriorityQueueWebAPI.Models
{
    public class Technician
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset StartDate { get; set; }
    }
}