using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PriorityQueueWebAPI.Models
{
    public class TechnicianContext : DbContext
    {
        public TechnicianContext()
                : base("name=TechnicianContext")
        {
        }

        public DbSet<Technician> Technicians { get; set; }
    }
}