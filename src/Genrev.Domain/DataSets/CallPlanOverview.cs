using System.Collections.Generic;

namespace Genrev.Domain.DataSets
{
    public class CallPlanOverview
    {

        //List of Person objects set manually

        private CallPlanOverviewDataset _parent;

        public CallPlanOverview(CallPlanOverviewDataset parent)
        {
            _parent = parent;
        }

        public string AccountType { get; set; }
        public int? YearlyCallPlan { get; set; }
        public int? GoalCount { get; set; }

        // Calculate Weeks as Person.DaysAvailable / 5
        // average? multiple persons.
        public decimal? AvgWeeklyCalls
        {
            get
            {
                return YearlyCallPlan / (decimal)52;
                //return _parent.WeeksAvailable == 0 ? null : (YearlyCallPlan / _parent.WeeksAvailable);
            }
        }


        public decimal? MonthlyCalls
        {
            get
            {
                return YearlyCallPlan / (decimal)12;
                //return YearlyCallPlan / ((decimal)12 * _parent.PersonnelCount);
            }
        }

        public int NumberOfAccounts { get; set; }
        public int? TotalCalls { get; set; }
        public decimal? SalesForecast { get; set; }


        public decimal? PercentOfTotalCalls
        {
            get
            {
                if (_parent.TotalCalls == 0) {
                    return 0;
                }
                return (TotalCalls / (decimal)_parent.TotalCalls) * 100;
            }
        }


        public decimal? PercentOfTotalSales
        {
            get
            {
                if (_parent.TotalSales == 0) {
                    return 0;
                }
                return (SalesForecast / _parent.TotalSales) * 100;
            }
        }

    }


    public class CallPlanOverviewDataset
    {
        public int TotalCalls { get; private set; }

        public decimal TotalSales { get; private set; }

        List<CallPlanOverview> _items;
        public IReadOnlyCollection<CallPlanOverview> Items { get { return _items.AsReadOnly(); } }

        //private decimal _weeksAvailable;
        //public decimal WeeksAvailable { get { return _weeksAvailable; } }

        //private int _personnelCount;
        //public int PersonnelCount { get { return _personnelCount; } }

        public CallPlanOverviewDataset(int fiscalYear, IEnumerable<Companies.Person> personnel)
        {
            _items = new List<CallPlanOverview>();
            TotalCalls = 0;
            TotalSales = 0;

            //int daysAvailable = 0;
            //foreach(var person in personnel)
            //{
            //    daysAvailable += person.GetAvailability(fiscalYear) == null ? 0 : person.GetAvailability(fiscalYear).DaysAvailable;
            //}

            //_personnelCount = personnel.Count();
            //_weeksAvailable = daysAvailable / 5;

        }

        public void Add(
            string accountType,
            int? yearlyCallPlan,
            int? goalCount,
            int numberOfAccounts,
            decimal? salesForecast)
        {

            _items.Add(new CallPlanOverview(this)
            {
                AccountType = accountType,
                YearlyCallPlan = yearlyCallPlan,
                GoalCount = goalCount,
                NumberOfAccounts = numberOfAccounts,
                TotalCalls = yearlyCallPlan,
                SalesForecast = salesForecast
            });

            TotalCalls += yearlyCallPlan ?? 0;
            TotalSales += salesForecast ?? 0;
        }


    }


}
