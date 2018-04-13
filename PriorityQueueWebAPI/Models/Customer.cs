using System;

namespace PriorityQueueWebAPI.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int TimesServiced { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public int Zip { get; set; }
        public string State { get; set; }
    }
}