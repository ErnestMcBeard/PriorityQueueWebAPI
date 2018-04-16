using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PriorityQueueWebAPI.Models
{
    public class JobContext : DbContext
    {
        public JobContext()
                : base("name=JobContext5")
        {
        }

        public DbSet<Job> Jobs { get; set; }
    }
}