using System.Collections.Generic;

namespace CostWise.App_Code.BLL
{
    public class ProductCalculationHistoryGroup
    {
        public ProductCalculationHistoryGroup()
        {
            Calculations = new List<CostCalculation>();
        }

        public int ProductId { get; set; }

        public string ProductName { get; set; }
        public decimal LatestTotalCost { get; set; }

        public decimal? PeriodStartTotalCost { get; set; }

        public decimal? PeriodChangePercentage { get; set; }

        public List<CostCalculation> Calculations { get; set; }
    }
}