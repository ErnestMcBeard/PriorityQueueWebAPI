using PriorityQueueWebAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PriorityQueueWebAPI
{
    public partial class Statistics : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime now = DateTime.Now;
                Month.Text = now.Month.ToString();
                Day.Text = now.Day.ToString();
                Year.Text = now.Year.ToString();
            }
        }

        //http://localhost:51578/Statistics.aspx
        protected async void GenerateDay_Click(object sender, EventArgs e)
        {
            int month, day, year;

            if (!(IsMonthValid(out month) && IsDayValid(out day) && IsYearValid(out year)))
            {
                DateError.Visible = true;
                return;
            }

            DateTime date;
            try
            {
                date = new DateTime(year, month, day);
            }
            catch
            {
                DateError.Visible = true;
                return;
            }

            DateError.Visible = false;

            StatGenerator.Period = StatGenerator.StatPeriod.Day;
            var averageQueueSize = await StatGenerator.AverageQueueSize(date);
            var averageWaitTime = await StatGenerator.AverageWaitTime(date);
            var jobResponseRate = await StatGenerator.JobResponseRate(date);
            var queueEmptyTime = await StatGenerator.QueueEmptyTime(date);
            //var technicianIdleTime = await StatGenerator.TechnicianIdleTime(date);

            DateLabel.Text = date.ToString();
            AverageWaitTime.Text = averageWaitTime.ToString() + " hour(s)" ;
            AverageQueueSize.Text = averageQueueSize.ToString() + " job(s)";
            JobResponseRate.Text = jobResponseRate.ToString() + " hour(s)";
            EmptyQueueTime.Text = queueEmptyTime.ToString() + "%";
        }

        protected async void GenerateMonth_Click(object sender, EventArgs e)
        {
            int month, year;
            if (!(IsMonthValid(out month) && IsYearValid(out year)))
            {
                DateError.Visible = true;
                return;
            }

            DateTime date;
            try
            {
                date = new DateTime(year, month, 1);
            }
            catch
            {
                DateError.Visible = true;
                return;
            }

            DateError.Visible = false;

            StatGenerator.Period = StatGenerator.StatPeriod.Month;
            var averageQueueSize = await StatGenerator.AverageQueueSize(date);
            var averageWaitTime = await StatGenerator.AverageWaitTime(date);
            var jobResponseRate = await StatGenerator.JobResponseRate(date);
            var queueEmptyTime = await StatGenerator.QueueEmptyTime(date);
            //var technicianIdleTime = await StatGenerator.TechnicianIdleTime(date);
        }

        private bool IsMonthValid(out int month)
        {
            if (int.TryParse(Month.Text, out month))
            {
                if (month > 0 && month < 13)
                    return true;
            }
            return false;
        }

        private bool IsDayValid(out int day)
        {
            if (int.TryParse(Day.Text, out day))
            {
                if (day > 0 && day < 31)
                    return true;
            }
            return false;
        }

        private bool IsYearValid(out int year)
        {
            if (int.TryParse(Year.Text, out year))
            {
                if (year > 1900 && year < 3000)
                    return true;
            }
            return false;
        }
    }
}