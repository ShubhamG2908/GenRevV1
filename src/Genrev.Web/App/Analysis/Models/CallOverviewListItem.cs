using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Analysis.Models
{
    public class CallPlanOverviewListItem
    {

        public string AccountType { get; set; }
        public double? YearlyCallPlan { get; set; }
        public double? GoalCount { get; set; }

        public double? AvgWeeklyCalls { get; set; }
        public double? MonthlyCalls { get; set; }

        public int NumberOfAccounts { get; set; }
        public double? TotalCalls { get; set; }

        public double? PercentOfTotalCalls { get; set; }
        public decimal? PercentOfTotalSales { get; set; }


        public static CallPlanOverviewListItem FromCallPlanOverviewModel(Domain.DataSets.CallPlanOverview data)
        {
            return new CallPlanOverviewListItem()
            {
                AccountType = data.AccountType,
                YearlyCallPlan = data.YearlyCallPlan,
                GoalCount = data.GoalCount,
                AvgWeeklyCalls = data.AvgWeeklyCalls,
                MonthlyCalls = data.MonthlyCalls,
                NumberOfAccounts = data.NumberOfAccounts,
                TotalCalls = data.TotalCalls,
                PercentOfTotalCalls = data.PercentOfTotalCalls,
                PercentOfTotalSales = data.PercentOfTotalSales
            };
        }
    }
}