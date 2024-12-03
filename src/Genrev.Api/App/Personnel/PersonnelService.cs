using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Genrev.Api.App.Personnel.Models;



namespace Genrev.Api.App.Personnel
{
    public class PersonnelService
    {


        internal PersonnelPostResponseModel SubmitPersonnel(PersonnelPostRequestModel request) {
            
            var stagingHelper = new DomainServices.Data.ImportStagingHelper(_context);

            var table = stagingHelper.GetPersonnelStagingTable();
            
            foreach (var item in request.Personnel) {
                var row = table.NewRow();
                table.Rows.Add(item.ID, item.FirstName, item.LastName);
            }

            var errors = stagingHelper.ImportToPersonnelStaging(table);

            if (errors.Count > 0) {
                var response = new PersonnelPostResponseModel(
                    (int)System.Net.HttpStatusCode.BadRequest, 
                    string.Join("; ", errors.ToList()));
                return response;
            }

            var importHelper = new DomainServices.Data.ImportCommitHelper(_context);
            importHelper.UpsertStagingToLive(Domain.Data.ImportType.Personnel, AppService.Current.AccountID);

            return new PersonnelPostResponseModel(System.Net.HttpStatusCode.OK);
        }


        private Data.GenrevContext _context;

        public PersonnelService() {
            _context = AppService.Current.DataContext;
        }
    }
}