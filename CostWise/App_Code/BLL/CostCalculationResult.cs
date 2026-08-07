using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CostWise.App_Code.BLL
{
    public class CostCalculationResult
    {
        public CostCalculation Calculation { get; set; }
        public List<CostCalculationItem> Items { get; set; }
    }
}