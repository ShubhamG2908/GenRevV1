using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Genrev.Api.App.Customers.Models;

namespace Genrev.Api.App.Customers
{
    public class CustomersService
    {


        internal CustomerPostResponseModel SubmitCustomers(CustomerPostRequestModel request) {

            var stagingHelper = new DomainServices.Data.ImportStagingHelper(_context);

            var table = stagingHelper.GetCustomerStagingTable();

            foreach (var item in request.Customers) {

                var row = table.NewRow();
                table.Rows.Add(
                    item.ID, item.Name, 
                    item.CustomerTypeID, item.AccountTypeID, item.IndustryTypeID,
                    item.Address1, item.Address2, item.City, item.State, 
                    item.Country, item.Phone, item.PostalCode);

            }

            var errors = stagingHelper.ImportToCustomersStaging(table);

            if (errors.Count > 0) {
                var response = new CustomerPostResponseModel(
                    (int)System.Net.HttpStatusCode.BadRequest,
                    string.Join("; ", errors.ToList()));
                return response;
            }

            var importHelper = new DomainServices.Data.ImportCommitHelper(_context);
            importHelper.UpsertStagingToLive(Domain.Data.ImportType.Customers, AppService.Current.AccountID);

            return new CustomerPostResponseModel(System.Net.HttpStatusCode.OK);

        }




        private Data.GenrevContext _context;

        public CustomersService() {
            _context = AppService.Current.DataContext;
        }
    }
}