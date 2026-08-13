using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Http;
using CostWise.App_Code.BLL;
using CostWise.Models;

namespace CostWise.Controllers
{
    [RoutePrefix("api/product-builder-data")]
    public class ProductBuilderDataController : ApiController
    {
        [HttpPost]
        [Route("ingredient-cost-preview")]
        public IHttpActionResult CalculateIngredientCostPreview(IngredientCostPreviewRequestDto request)
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }
            object userIdValue = HttpContext.Current.Session["UserId"];
            int userId;
            if (userIdValue == null || !int.TryParse(userIdValue.ToString(), out userId) || userId <= 0)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }
            if (request == null)
            {
                return BadRequest("נתוני חישוב המחיר חסרים.");
            }
            try
            {
                decimal calculatedCost = CostCalculationBLL.CalculateIngredientCostPreview(userId, request.IngredientId, request.Quantity, request.MeasurementUnitId, request.ProductId); IngredientCostPreviewResponseDto response = new IngredientCostPreviewResponseDto();
                response.CalculatedCost = calculatedCost;
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get(int? productId = null)
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }
            object userIdValue = HttpContext.Current.Session["UserId"];
            int userId;
            if (userIdValue == null || !int.TryParse(userIdValue.ToString(), out userId) || userId <= 0)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }
            try
            {
                List<Ingredient> ingredients = ProductBLL.GetIngredientsForProductBuilder(userId, productId);
                List<MeasurementUnit> measurementUnits = MeasurementUnitBLL.GetAvailableUnits(userId);
                ProductBuilderDataDto response = new ProductBuilderDataDto();
                response.Ingredients = new List<IngredientAutocompleteDto>();
                response.MeasurementUnits = new List<MeasurementUnitAutocompleteDto>();
                foreach (Ingredient ingredient in ingredients)
                {
                    IngredientAutocompleteDto ingredientDto = new IngredientAutocompleteDto();
                    ingredientDto.IngredientId = ingredient.IngredientId;
                    ingredientDto.IngredientName = ingredient.IngredientName;
                    ingredientDto.PackagePrice = ingredient.PackagePrice;
                    ingredientDto.PackageQuantity = ingredient.PackageQuantity;
                    ingredientDto.PackageUnitId = ingredient.PackageUnitId;
                    ingredientDto.IsActive = ingredient.IsActive;
                    response.Ingredients.Add(ingredientDto);
                }
                foreach (MeasurementUnit measurementUnit in measurementUnits)
                {
                    MeasurementUnitAutocompleteDto measurementUnitDto = new MeasurementUnitAutocompleteDto();
                    measurementUnitDto.MeasurementUnitId = measurementUnit.MeasurementUnitId;
                    measurementUnitDto.UnitName = measurementUnit.UnitName;
                    measurementUnitDto.UnitFamily = measurementUnit.UnitFamily;
                    response.MeasurementUnits.Add(measurementUnitDto);
                }
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}