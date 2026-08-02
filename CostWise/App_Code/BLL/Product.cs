using System;

namespace CostWise.App_Code.BLL
{
    public class Product
    {
        public int ProductId { get; set; }
        public int BusinessId { get; set; }
        public string ProductName { get; set; }
        public decimal YieldQuantity { get; set; }
        public string YieldUnitLabel { get; set; }
        public string InstructionsHtml { get; set; }
        public string ImagePath { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}