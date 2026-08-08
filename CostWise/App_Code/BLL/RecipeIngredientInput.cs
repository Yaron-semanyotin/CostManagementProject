using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CostWise.App_Code.BLL
{
    public class RecipeIngredientInput
    {
        public int IngredientId { get; set; }

        public decimal Quantity { get; set; }

        public int MeasurementUnitId { get; set; }
    }
}