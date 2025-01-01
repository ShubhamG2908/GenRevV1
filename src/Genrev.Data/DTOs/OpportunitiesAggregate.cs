namespace Genrev.Data.DTOs
{
    class OpportunitiesAggregate
    {
        public int GroupEntityID { get; set; }
        public string GroupEntityName { get; set; }
        public decimal Potential { get; set; }
        public decimal CurrentOpportunity { get; set; }
        public decimal FutureOpportunity { get; set; }
        public decimal MarketShare { get; set; }
    }
}
