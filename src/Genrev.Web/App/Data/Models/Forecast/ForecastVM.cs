using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Data.Models.Forecast
{
    public class ForecastVM
    {

        public int SelectedYear { get; set; }
        public IEnumerable<int> AvailableYears { get; set; }

        public List<CommonListItems.Person> AvailableSalespersons { get; set; }
        public CommonListItems.Person SelectedSalesperson { get; set; }

        public bool HasDownstreamSalespersons {
            get
            {
                return AvailableSalespersons.Count > 1;
            }
        }





    }
}