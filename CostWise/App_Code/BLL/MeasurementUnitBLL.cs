using CostWise.App_Code.DAL;
using System.Collections.Generic;

namespace CostWise.App_Code.BLL
{
    public static class MeasurementUnitBLL
    {
        public static List<MeasurementUnit> GetSystemUnits()
        {
            return MeasurementUnitDAL.GetSystemUnits();
        }
    }
}