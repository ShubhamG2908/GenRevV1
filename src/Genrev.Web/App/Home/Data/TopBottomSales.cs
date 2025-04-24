using Genrev.Data.DTOs;

using System.Collections.Generic;

namespace Genrev.Web.App.Home.Data
{
    public class DashboardViewModel
    {        
            public List<CRMRecordDTO> CRMRecords { get; set; }
        public List<SalesByCustomer> SalesByCustomer { get; set; }
        public List<SalesBySalesperson> SalesBySalesperson { get; set; }
    }

    public class SalesByCustomer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public decimal SalesAmount { get; set; }
    }

    public class SalesBySalesperson
    {
        public int SalespersonID { get; set; }
        public string SalespersonName { get; set; }
        public decimal SalesAmount { get; set; }
    }

}