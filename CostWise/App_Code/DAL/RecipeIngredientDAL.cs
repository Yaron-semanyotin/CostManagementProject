using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CostWise.App_Code.BLL;

namespace CostWise.App_Code.DAL
{
    public static class RecipeIngredientDAL
    {
        public static List<RecipeIngredient> GetRecipeIngredientsForProduct(int userId, int productId)
        {
            List<RecipeIngredient> recipeIngredients = new List<RecipeIngredient>();
            return recipeIngredients;
        }
    }
}