using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CostWise.App_Code.BLL
{
    public class CostCalculationResult
    {
        public CostCalculationResult()
        {
            Items = new List<CostCalculationItem>();

            ChangeReasons = new List<CostCalculationChangeReason>();
        }

        public CostCalculation Calculation { get; set; }

        public List<CostCalculationItem> Items { get; set; }

        public CostCalculation PreviousCalculation { get; set; }

        public List<CostCalculationChangeReason> ChangeReasons { get; set; }
    }
}