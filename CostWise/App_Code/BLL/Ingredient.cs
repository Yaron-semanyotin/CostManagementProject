using System;

namespace CostWise.App_Code.BLL
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        public int BusinessId { get; set; }
        public string IngredientName { get; set; }
        public decimal PackagePrice { get; set; }
        public decimal PackageQuantity { get; set; }
        public int PackageUnitId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}