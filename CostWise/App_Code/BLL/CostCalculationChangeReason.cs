namespace CostWise.App_Code.BLL
{
    public class CostCalculationChangeReason
    {
        public int IngredientId { get; set; }

        public string IngredientName { get; set; }

        public bool WasAdded { get; set; }

        public bool WasRemoved { get; set; }

        public bool PackagePriceChanged { get; set; }

        public bool PackageQuantityChanged { get; set; }

        public bool RecipeQuantityChanged { get; set; }

        public bool ManualOverrideChanged { get; set; }

        public decimal? PreviousPackagePrice { get; set; }

        public decimal? CurrentPackagePrice { get; set; }
        public decimal? PackagePriceChange { get; set; }

        public decimal? PreviousPackageQuantityInBaseUnit { get; set; }

        public decimal? CurrentPackageQuantityInBaseUnit { get; set; }

        public decimal? PreviousRecipeQuantityInBaseUnit { get; set; }

        public decimal? CurrentRecipeQuantityInBaseUnit { get; set; }

        public decimal? PreviousManualIngredientCostOverride { get; set; }

        public decimal? CurrentManualIngredientCostOverride { get; set; }

        public decimal? PreviousIngredientCost { get; set; }

        public decimal? CurrentIngredientCost { get; set; }

        public decimal IngredientCostChange { get; set; }

        public string BaseUnitName { get; set; }
    }
}