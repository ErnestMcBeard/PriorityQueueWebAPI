using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PriorityQueueWebAPI.Models
{
    public class CustomerContext : DbContext
    {
        public CustomerContext()
                : base("name=CustomerContext1")
        {
        }
        public DbSet<Customer> Customers { get; set; }
    }
}