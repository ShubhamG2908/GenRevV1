using System.Collections.Generic;

namespace Genrev.Web.App.Data.Models.Actual
{
    public class ActualVM
    {
        public int SelectedYear { get; set; }
        public IEnumerable<int> AvailableYears { get; set; }

        public List<ActualGridItem> ActualGridItems { get; set; }
    }
}