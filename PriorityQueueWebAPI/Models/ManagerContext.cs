using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace PriorityQueueWebAPI.Models
{
    public class ManagerContext : DbContext
    {
        public ManagerContext() : base("name=ManagerContext")
        {

        }

        public DbSet<Manager> Managers { get; set; }
    }
}