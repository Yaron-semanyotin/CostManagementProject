using System.Collections.Generic;

namespace CostWise.Models
{
    public class ProductBuilderDataDto
    {
        public List<IngredientAutocompleteDto> Ingredients { get; set; }
        public List<MeasurementUnitAutocompleteDto> MeasurementUnits { get; set; }
    }

    public class IngredientAutocompleteDto
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; }
        public decimal PackagePrice { get; set; }
        public decimal PackageQuantity { get; set; }
        public int PackageUnitId { get; set; }
        public bool IsActive { get; set; }
    }

    public class MeasurementUnitAutocompleteDto
    {
        public int MeasurementUnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitFamily { get; set; }
    }
    public class IngredientCostPreviewRequestDto
    {
        public int? ProductId { get; set; }
        public int IngredientId { get; set; }

        public decimal Quantity { get; set; }

        public int MeasurementUnitId { get; set; }
    }

    public class IngredientCostPreviewResponseDto
    {
        public decimal CalculatedCost { get; set; }
    }
}