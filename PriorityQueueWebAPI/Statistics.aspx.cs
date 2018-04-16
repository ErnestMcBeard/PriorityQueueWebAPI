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
        protected void GenerateDay_Click(object sender, EventArgs e)
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
        }

        protected void GenerateMonth_Click(object sender, EventArgs e)
        {
            int month;
            if (!IsMonthValid(out month))
            {
                DateError.Visible = true;
                return;
            }
            DateError.Visible = false;
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