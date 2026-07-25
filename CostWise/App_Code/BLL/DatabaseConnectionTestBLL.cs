using CostWise.App_Code.DAL;

namespace CostWise.App_Code.BLL
{
    public static class DatabaseConnectionTestBLL
    {
        public static void TestConnection()
        {
            DatabaseConnectionTestDAL.TestConnection();
        }
    }
}