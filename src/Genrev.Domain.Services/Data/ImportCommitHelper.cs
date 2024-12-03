using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Genrev.Domain.Data;
using Genrev.Data;

namespace Genrev.DomainServices.Data
{
    public class ImportCommitHelper
    {


        GenrevContext context;

        public ImportCommitHelper(GenrevContext context) {
            this.context = context;
        }        


        public void UpsertStagingToLive(ImportType type, int accountID, bool reloadContextWhenDone = true) {

            switch (type) {

                case ImportType.AccountTypes:
                    context.UpsertAccountTypesStagingToLive(accountID);
                    break;

                case ImportType.Companies:
                    context.UpsertCompaniesStagingToLive(accountID);
                    break;

                case ImportType.Customers:
                    context.UpsertCustomerStagingToLive(accountID);
                    break;

                case ImportType.CustomerTypes:
                    context.UpsertCustomerTypesStagingToLive(accountID);
                    break;

                case ImportType.IndustryTypes:
                    context.UpsertIndustryTypesStagingToLive(accountID);
                    break;

                case ImportType.MonthlyData:
                    context.UpsertMonthlyDataStagingToLive(accountID);
                    break;

                case ImportType.Personnel:
                    context.UpsertPersonnelStagingToLive(accountID);
                    break;

                default:
                    throw new InvalidOperationException("ImportType not registered");
            }

            if (reloadContextWhenDone) {
                context.Dispose();
                context = new GenrevContext();
            }

        }

    }
}
