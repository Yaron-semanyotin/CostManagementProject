
namespace CostWise.App_Code.BLL
{
    public class RecipeIngredient
    {
        public int RecipeIngredientId { get; set; }
        public int ProductId { get; set; }
        public int IngredientId { get; set; }
        public decimal Quantity { get; set; }
        public int MeasurementUnitId { get; set; }
        public int SortOrder { get; set; }
    }
}