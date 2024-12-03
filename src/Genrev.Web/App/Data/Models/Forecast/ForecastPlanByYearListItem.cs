using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Data.Models.Forecast
{
    public class ForecastPlanByYearListItem
    {

        public int AccountTypeID { get; set; }
        public string AccountType { get; set; }
        public int PlannedCalls { get; set; }
        public int? GoalCalls { get; set; }
        public int AccountTypeCount { get; set; }
        

    }
}