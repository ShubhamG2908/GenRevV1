using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Genrev.Data.Importing
{
    class CompaniesImporter
    {

        //List<DTOs.Company> _data;
        List<Domain.Companies.Company> _companies;
        int _accountID;

        public CompaniesImporter(int accountID, List<Domain.Companies.Company> companies) {
            _accountID = accountID;
            _companies = companies;
        }


        public bool Apply() {

            var context = new GenrevContext();

            var updates = from target in context.Companies
                          join source in _companies on target.ID equals source.ID
                          where target.AccountID == _accountID
                          select target;

            var inserts = from target in context.Companies
                          join source in _companies on target.ID equals source.ID into unmatched
                          from x in unmatched.DefaultIfEmpty()
                          select x;

            var deletes = from source in _companies
                          join target in context.Companies on source.ID equals target.ID into unmatched
                          from x in unmatched.DefaultIfEmpty()
                          select x;

            var updatesList = updates.ToList();
            var insertsList = inserts.ToList();
            var deletesList = deletes.ToList();

            context.Companies.AddRange(insertsList);
            context.Companies.RemoveRange(deletesList);
            foreach (var u in updatesList) {
                var d = context.Companies.Find(u.ID);
                d = u;
            }

            context.SaveChanges();

            return true;
            
        }


        //public bool LoadCsv(string path, bool containsHeaders) {
        //    var table = CommonServices.GetTableFromCsv(path, containsHeaders);
        //    return LoadDataTable(table);
        //}

        //public bool LoadDataTable(DataTable table) {

        //    _data = new List<DTOs.Company>();

        //    foreach (DataRow row in table.Rows) {

        //        var item = new DTOs.Company();
                
        //        item.ClientID = row[0].ToString();
        //        item.ParentClientID = row[1].ToString();
        //        item.FullName = row[2].ToString();
        //        item.Name = row[3].ToString();
        //        item.Code = row[4].ToString();
        //        item.Code = row[5].ToString();
        //        item.FiscalMonthEnd = int.Parse(row[6].ToString());

        //        _data.Add(item);
        //    }

        //    return true;
        //}

        //public bool LoadEntities(List<Domain.Companies.Company> companies) {
        //    throw new NotImplementedException();
        //}
        
        
                
    }
}

