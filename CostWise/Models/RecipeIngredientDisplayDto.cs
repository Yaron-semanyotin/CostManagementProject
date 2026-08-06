using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CostWise.Models
{
    public class RecipeIngredientDisplayDto
    {
        public int RecipeIngredientId { get; set; }

        public int IngredientId { get; set; }

        public string IngredientName { get; set; }

        public decimal Quantity { get; set; }

        public int MeasurementUnitId { get; set; }

        public string MeasurementUnitName { get; set; }
    }
}