using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Data.Models.Management
{
    public class MatrixVM
    {

        public enum ViewModes
        {
            GroupedByMonth = 0,
            ListAll = 1
        }

        public int SelectedYear { get; set; }
        public IEnumerable<int> AvailableYears { get; set; }

        public ViewModes ViewMode { get; set; } = ViewModes.GroupedByMonth;
        
        public List<MatrixByCostGridItem> MatrixGridItems { get; set; }
        public List<CommonListItems.Customer> CustomersList { get; set; }
        public List<CommonListItems.Person> PersonnelList { get; set; }
        public List<CommonListItems.Product> ProductsList { get; set; }
        public List<CommonListItems.Period> FiscalYearPeriodsList { get; set; }

    }
}