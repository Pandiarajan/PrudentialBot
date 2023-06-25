namespace NewsAgenciesAPI.Model
{
    public class PortfolioModel
    {
        public string? Name { get; set; }
        public List<Investments>? Investments { get; set; }

    }
    public class Investments
    {
        public string? Symbol{get; set;}
        public long NumOfShares { get; set;}
        public long InvestedAmount { get; set;}
        public long Profit { get; set;}
    }
}
