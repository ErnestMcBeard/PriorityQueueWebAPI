using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PriorityQueueWebAPI.Models
{
    public class DailyStatisticContext : DbContext
    {
        public DailyStatisticContext () : base("name=DailyStatisticContext1")
        {

        }

        public DbSet<DailyStatistic> DailyStatistics { get; set; }
    }
}