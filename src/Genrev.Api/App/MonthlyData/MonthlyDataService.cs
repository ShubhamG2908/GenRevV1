using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Genrev.Api.App.MonthlyData.Models;

namespace Genrev.Api.App.MonthlyData
{
    public class MonthlyDataService
    {

        
        internal MonthlyDataPostResponseModel SubmitMonthlyData(MonthlyDataPostRequestModel request) {

            var stagingHelper = new DomainServices.Data.ImportStagingHelper(_context);

            var table = stagingHelper.GetMonthlyDataStagingTable();

            foreach (var item in request.DataEntries) {
                var row = table.NewRow();
                table.Rows.Add(item.CustomerID, item.PersonnelID, item.Period, item.ActualSales, item.ActualGPP, item.ActualCalls);
            }

            var errors = stagingHelper.ImportToMonthlyDataStaging(table);

            if (errors.Count > 0) {
                var response = new MonthlyDataPostResponseModel(
                    (int)System.Net.HttpStatusCode.BadRequest,
                    string.Join("; ", errors.ToList()));
                return response;
            }

            var importHelper = new DomainServices.Data.ImportCommitHelper(_context);
            importHelper.UpsertStagingToLive(Domain.Data.ImportType.MonthlyData, AppService.Current.AccountID);

            return new MonthlyDataPostResponseModel(System.Net.HttpStatusCode.OK);
        }



        private Data.GenrevContext _context;

        public MonthlyDataService() {
            _context = AppService.Current.DataContext;
        }

    }
}