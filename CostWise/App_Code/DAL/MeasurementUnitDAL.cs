using System.Data.SqlClient;
using System.Collections.Generic;
using CostWise.App_Code.BLL;

namespace CostWise.App_Code.DAL
{
    public static class MeasurementUnitDAL
    {
        public static List<MeasurementUnit> GetSystemUnits()
        {
            List<MeasurementUnit> systemUnits = new List<MeasurementUnit>(); // Empty list to save the selected cols
            // Select query
            const string query = @"SELECT
            MeasurementUnitId,
            BusinessId,
            UnitName,
            UnitFamily,
            ConversionFactorToBase,
            CreateAtUtc AS CreatedAtUtc,
            UpdatedAtUtc
            FROM dbo.T_MeasurementUnits
            WHERE BusinessId IS NULL;";
            using (SqlConnection connection = DatabaseHelper.GetConnection()) // connection to DB
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open(); // Opens the connection
                    using (SqlDataReader reader = command.ExecuteReader()) // data reader to return the select query
                    {
                        while (reader.Read()) // loop that loops until Read() returns false
                        {
                            MeasurementUnit unit = new MeasurementUnit(); // creating MeasurementUnit object to hold the info
                            unit.MeasurementUnitId = reader.GetInt32(reader.GetOrdinal("MeasurementUnitId")); // finding the index by the name
                            int businessIdOrdinal = reader.GetOrdinal("BusinessId"); // reading from Int column
                            if (reader.IsDBNull(businessIdOrdinal))
                                unit.BusinessId = null;
                            else
                                unit.BusinessId = reader.GetInt32(businessIdOrdinal);
                            unit.UnitName = reader.GetString(reader.GetOrdinal("UnitName")); // finding the unitname
                            unit.UnitFamily = reader.GetString(reader.GetOrdinal("UnitFamily")); // finding the unitfamily
                            unit.ConversionFactorToBase = reader.GetDecimal(reader.GetOrdinal("ConversionFactorToBase")); // finding ConversionFactorToBase
                            unit.CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")); // finding CreatedAtUtc
                            unit.UpdatedAtUtc = reader.GetDateTime(reader.GetOrdinal("UpdatedAtUtc")); // finding UpdatedAtUtc
                            systemUnits.Add(unit); // every time the loop enters it adds into the object the unit types
                        }
                    }
                }
            }
            return systemUnits; // return the object systemUnits
        }
    }
}