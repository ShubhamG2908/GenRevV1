using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Companies
{
    public class PersonnelAvailability
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int PersonnelID { get; set; }
        public int AvailabilityYear { get; set; }
        public int Weekdays { get; set; }
        public int? Holidays { get; set; }
        public int? VacationDays { get; set; }
        public int? AdministrationDays { get; set; }
        public int? OtherDays { get; set; }
        public int? PlannedCallsPerDay { get; set; }



        public int? CallCommitment {
            get
            {
                if (!PlannedCallsPerDay.HasValue) {
                    return null;
                }

                return DaysAvailable * PlannedCallsPerDay.Value;
            }
        }


        public int DaysAvailable {
            get
            {
                var holdidays = Holidays ?? 0;
                var vaca = VacationDays ?? 0;
                var admin = AdministrationDays ?? 0;
                var other = OtherDays ?? 0;

                return Weekdays - holdidays - vaca - admin - other;
            }
        }

        public virtual Person Person { get; set; }


    }
}
