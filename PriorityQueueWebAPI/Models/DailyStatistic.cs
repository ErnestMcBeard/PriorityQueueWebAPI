using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriorityQueueWebAPI.Models
{
    public class DailyStatistic
    {
        public Guid Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public int LastQueueLength { get; set; }
    }
}