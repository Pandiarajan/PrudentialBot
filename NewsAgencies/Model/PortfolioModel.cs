using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAgencies.Model
{
    public class PortfolioModel
    {
        public string? Name { get; set; }
        public List<Investments>? Investments { get; set; }

    }
    public class Investments
    {
        public string? Symbol { get; set; }
        public long NumberOfShares { get; set; }
        public long InvestedAmount { get; set; }
        public long Profit { get; set; }
    }
}
