namespace Genrev.Domain.Data.Staging
{

    public class AccountTypeStaging
    {
        public int ID { get; set; }
        public string ClientID { get; set; }
        public string Name { get; set; }
        public double? CallsPerMonthGoal { get; set; }
    }


    public class CustomerTypeStaging
    {
        public int ID { get; set; }
        public string ClientID { get; set; }
        public string Name { get; set; }
    }

    public class IndustryTypeStaging
    {
        public int ID { get; set; }
        public string ClientID { get; set; }
        public string Name { get; set; }
    }

}
