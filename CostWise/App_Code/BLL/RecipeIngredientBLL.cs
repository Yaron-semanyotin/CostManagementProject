using System;
using System.Collections.Generic;
using CostWise.App_Code.DAL;

namespace CostWise.App_Code.BLL
{
    public static class RecipeIngredientBLL
    {
        public static List<RecipeIngredient> GetRecipeIngredientsForProduct(int userId, int productId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("זהות המשתמש אינה תקינה.");
            }
            if (productId <= 0)
            {
                throw new ArgumentException("המוצר שנבחר אינו תקין.");
            }
            List<Product> products = ProductBLL.GetProductsForUser(userId);
            Product product = products.Find(item => item.ProductId == productId);
            if (product == null || !product.IsActive)
            {
                throw new InvalidOperationException("המוצר לא נמצא, אינו פעיל או אינו שייך לעסק שלך.");
            }
            return RecipeIngredientDAL.GetRecipeIngredientsForProduct(userId, productId);
        }
        
        
    }
}