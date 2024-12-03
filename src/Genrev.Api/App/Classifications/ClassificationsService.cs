using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Genrev.Api.App.Classifications.Models;

namespace Genrev.Api.App.Classifications
{
    public class ClassificationsService
    {



        internal IndustryTypesPostResponseModel SubmitIndustryTypes(IndustryTypePostRequestModel request) {
            
            var stagingHelper = new DomainServices.Data.ImportStagingHelper(_context);

            var table = stagingHelper.GetIndustryTypeStagingTable();

            foreach (var item in request.IndustryTypes) {
                var row = table.NewRow();
                table.Rows.Add(item.ID, item.Name);
            }

            var errors = stagingHelper.ImportToIndustryTypesStaging(table);

            if (errors.Count > 0) {
                var response = new IndustryTypesPostResponseModel(
                    (int)System.Net.HttpStatusCode.BadRequest,
                    string.Join("; ", errors.ToList()));
                return response;
            }

            var importHelper = new DomainServices.Data.ImportCommitHelper(_context);
            importHelper.UpsertStagingToLive(Domain.Data.ImportType.IndustryTypes, AppService.Current.AccountID);

            return new IndustryTypesPostResponseModel(System.Net.HttpStatusCode.OK);

        }


        internal CustomerTypesPostResponseModel SubmitCustomerTypes(CustomerTypePostRequestModel request) {
            
            var stagingHelper = new DomainServices.Data.ImportStagingHelper(_context);

            var table = stagingHelper.GetCustomerTypeStagingTable();

            foreach (var item in request.CustomerTypes) {
                var row = table.NewRow();
                table.Rows.Add(item.ID, item.Name);
            }

            var errors = stagingHelper.ImportToCustomerTypesStaging(table);

            if (errors.Count > 0) {
                var response = new CustomerTypesPostResponseModel(
                    (int)System.Net.HttpStatusCode.BadRequest,
                    string.Join("; ", errors.ToList()));
                return response;
            }

            var importHelper = new DomainServices.Data.ImportCommitHelper(_context);
            importHelper.UpsertStagingToLive(Domain.Data.ImportType.CustomerTypes, AppService.Current.AccountID);

            return new CustomerTypesPostResponseModel(System.Net.HttpStatusCode.OK);

        }




        internal AccountTypesPostResponseModel SubmitAccountTypes(AccountTypesPostRequestModel request) {

            var stagingHelper = new DomainServices.Data.ImportStagingHelper(_context);

            var table = stagingHelper.GetAccountTypeStagingTable();

            foreach (var item in request.AccountTypes) {
                var row = table.NewRow();
                table.Rows.Add(item.ID, item.Name, item.CallsPerMonthGoal);
            }

            var errors = stagingHelper.ImportToAccountTypesStaging(table);

            if (errors.Count > 0) {
                var response = new AccountTypesPostResponseModel(
                    (int)System.Net.HttpStatusCode.BadRequest,
                    string.Join("; ", errors.ToList()));
                return response;
            }

            var importHelper = new DomainServices.Data.ImportCommitHelper(_context);
            importHelper.UpsertStagingToLive(Domain.Data.ImportType.AccountTypes, AppService.Current.AccountID);

            return new AccountTypesPostResponseModel(System.Net.HttpStatusCode.OK);
        }

        

        private Data.GenrevContext _context;

        public ClassificationsService() {
            _context = AppService.Current.DataContext;
        }

        
    }
}