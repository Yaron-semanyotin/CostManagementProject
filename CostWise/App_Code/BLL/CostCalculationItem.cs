namespace CostWise.App_Code.BLL
{
    public class CostCalculationItem
    {
        public int CostCalculationItemId { get; set; }
        public int CostCalculationId { get; set; }
        public int IngredientId { get; set; }
        public string IngredientNameSnapshot { get; set; }
        public decimal PackagePriceSnapshot { get; set; }
        public decimal PackageQuantitySnapshot { get; set; }
        public string PackageUnitNameSnapshot { get; set; }
        public string PackageUnitFamilySnapshot { get; set; }
        public decimal PackageUnitConversionFactorSnapshot { get; set; }
        public decimal RecipeQuantitySnapshot { get; set; }
        public string RecipeUnitNameSnapshot { get; set; }
        public string RecipeUnitFamilySnapshot { get; set; }
        public decimal RecipeUnitConversionFactorSnapshot { get; set; }
        public string BaseUnitNameSnapshot { get; set; }
        public decimal PackageQuantityInBaseUnitSnapshot { get; set; }
        public decimal RecipeQuantityInBaseUnitSnapshot { get; set; }
        public decimal PricePerBaseUnitSnapshot { get; set; }
        public decimal IngredientCostSnapshot { get; set; }
        public int SortOrderSnapshot { get; set; }
    }
}