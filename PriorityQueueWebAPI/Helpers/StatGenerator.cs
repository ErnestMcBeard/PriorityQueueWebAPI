using MODELPriorityQueue.Models;
using PriorityQueueWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PriorityQueueWebAPI.Helpers
{
    public static class StatGenerator
    {
        public enum StatPeriod { Day, Month }
        public static StatPeriod Period = StatPeriod.Day;

        private static DateTimeOffset BeginningOfDay(DateTimeOffset date)
        {
            return new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, default(TimeSpan));
        }

        private static DateTimeOffset BeginningOfMonth(DateTimeOffset date)
        {
            return new DateTimeOffset(date.Year, date.Month, 1, 0, 0, 0, default(TimeSpan));
        }

        private static string FilterFormatDTO(DateTimeOffset dto)
        {
            return dto.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        //Average wait time before job started
        public static async Task<double> AverageWaitTime(DateTimeOffset givenDate)
        {
            string jobQuery;

            if (Period == StatPeriod.Day)
            {
                givenDate = BeginningOfDay(givenDate);
                jobQuery = string.Format("$filter=Entered gt {0} and Entered lt {1} and Started ne {2}", FilterFormatDTO(givenDate), FilterFormatDTO(givenDate.AddDays(1)), FilterFormatDTO(default(DateTimeOffset)));
            }
            else
            {
                givenDate = BeginningOfMonth(givenDate);
                jobQuery = string.Format("$filter=Entered gt {0} and Entered lt {1} and Started ne {2}", FilterFormatDTO(givenDate), FilterFormatDTO(givenDate.AddMonths(1)), FilterFormatDTO(default(DateTimeOffset)));
            }

            var jobs = await WebApiHelper.Get<Job>(jobQuery);

            if (jobs.Count > 0)
            {
                double total = 0;
                foreach (Job job in jobs)
                {
                    TimeSpan difference = (job.Started.DateTime - job.Entered.DateTime);
                    total += difference.TotalHours;
                }
                return total / jobs.Count;
            }

            return 0;
        }

        //Average queue size
        public static async Task<double> AverageQueueSize(DateTimeOffset givenDate)
        {
            Dictionary<int, TimeSpan> sizeByTime = await GetQueueSizeTimes(givenDate);

            double maxSize = sizeByTime.Max(x => x.Key);

            if (maxSize == 0)
            {
                return 0;
            }

            double totalHours = sizeByTime.Sum(x => x.Value.TotalHours);
            double averageSize = 0;

            foreach (var pair in sizeByTime)
            {
                averageSize += pair.Key / maxSize * pair.Value.TotalHours / totalHours;
            }

            return averageSize;
        }

        public static async Task<Dictionary<int, TimeSpan>> GetQueueSizeTimes(DateTimeOffset givenDate)
        {
            int lastQueueSize;
            string jobQuery;

            if (Period == StatPeriod.Day)
            {
                givenDate = BeginningOfDay(givenDate);
                DailyStatistic previousStat = (await WebApiHelper.Get<DailyStatistic>(string.Format("$filter=Date lt {0}&$top=1&$orderby=Date desc", FilterFormatDTO(givenDate)))).FirstOrDefault();
                lastQueueSize = previousStat == null ? 0 : previousStat.LastQueueLength;
                jobQuery = string.Format("$filter=(Entered gt {0} and Entered lt {1}) or (Finished gt {0} and Finished lt {1})", FilterFormatDTO(givenDate), FilterFormatDTO(givenDate.AddDays(1)));
            }
            else
            {
                givenDate = BeginningOfMonth(givenDate);
                DailyStatistic previousStat = (await WebApiHelper.Get<DailyStatistic>(string.Format("$filter=Date lt {0}&$top=1&$orderby=Date desc", FilterFormatDTO(givenDate)))).FirstOrDefault();
                lastQueueSize = previousStat == null ? 0 : previousStat.LastQueueLength;
                jobQuery = string.Format("$filter=(Entered gt {0} and Entered lt {1}) or (Finished gt {0} and Finished lt {1})", FilterFormatDTO(givenDate), FilterFormatDTO(givenDate.AddMonths(1)));
            }

            var jobs = await WebApiHelper.Get<Job>(jobQuery);
            var entered = jobs.Where(x => givenDate <= x.Entered && x.Entered < givenDate.AddDays(1));
            var removed = jobs.Where(x => givenDate <= x.Finished && x.Finished < givenDate.AddDays(1));

            List<QueueChange> changes = new List<QueueChange>();
            foreach (var entry in entered)
            {
                changes.Add(new QueueChange()
                {
                    type = QueueChange.ChangeType.Entered,
                    date = entry.Entered
                });
            }

            foreach (var removal in removed)
            {
                changes.Add(new QueueChange()
                {
                    type = QueueChange.ChangeType.Removed,
                    date = removal.Finished
                });
            }

            var orderedChanges = changes.OrderBy(x => x.date).ToList();
            if (DateTimeOffset.Now < givenDate.AddDays(1))
            {
                orderedChanges.Add(new QueueChange() { date = DateTimeOffset.Now });
            }
            else
            {
                orderedChanges.Add(new QueueChange() { date = givenDate.AddDays(1) });
            }


            Dictionary<int, TimeSpan> sizeByTime = new Dictionary<int, TimeSpan>();
            int prevSize = lastQueueSize;
            DateTimeOffset prevDate = givenDate;

            for (int i = 0; i < orderedChanges.Count(); i++)
            {
                if (!sizeByTime.ContainsKey(prevSize))
                    sizeByTime[prevSize] = new TimeSpan();
                sizeByTime[prevSize] += orderedChanges[i].date.DateTime - prevDate.DateTime;

                if (orderedChanges[i].type == QueueChange.ChangeType.Entered)
                    prevSize++;
                else
                    prevSize--;
            }

            return sizeByTime;
        }

        private struct QueueChange
        {
            public enum ChangeType { Entered, Removed };
            public ChangeType type;
            public DateTimeOffset date;
        }

        //The percentage of time the queue was completely empty this day/month
        public static async Task<double> QueueEmptyTime(DateTimeOffset givenDate)
        {
            Dictionary<int, TimeSpan> sizeByTime = await GetQueueSizeTimes(givenDate);

            double summedTotalHours = sizeByTime.Sum(x => x.Value.TotalHours);
            if (sizeByTime.ContainsKey(0))
            {
                double emptyHours = sizeByTime[0].TotalHours;
                return emptyHours / summedTotalHours * 100;
            }

            return 0;
        }

        //Number of jobs entered on this day (or this month) not started on the entry date
        public static async Task<int> JobResponseRate(DateTimeOffset givenDate)
        {
            string jobQuery;

            if (Period == StatPeriod.Day)
            {
                givenDate = BeginningOfDay(givenDate);
                jobQuery = string.Format("$filter=Entered gt {0} and Entered lt {1} and Started gt {1}", FilterFormatDTO(givenDate), FilterFormatDTO(givenDate.AddDays(1)));
            }
            else
            {
                givenDate = BeginningOfMonth(givenDate);
                jobQuery = string.Format("$filter=Entered gt {0} and Entered lt {1} and Started gt {1}", FilterFormatDTO(givenDate), FilterFormatDTO(givenDate.AddMonths(1)));
            }

            var jobs = await WebApiHelper.Get<Job>(jobQuery);
            return jobs.Where(x => x.Started >= EarliestPointNextDay(x.Entered)).Count();
        }

        private static DateTimeOffset EarliestPointNextDay(DateTimeOffset date)
        {
            date = date.AddDays(1);
            return new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, default(TimeSpan));
        }

        //The number of hours out of 8 that each technician is idle this day(s)
        public static async Task<List<Technician>> TechnicianIdleTime(DateTimeOffset givenDate)
        {
            throw new NotImplementedException();
        }
    }
}