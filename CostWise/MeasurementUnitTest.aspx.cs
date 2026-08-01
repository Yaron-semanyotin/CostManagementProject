using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CostWise.App_Code.BLL;

namespace CostWise
{
    public partial class MeasurementUnitTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<MeasurementUnit> systemUnits = MeasurementUnitBLL.GetSystemUnits();
                ResultLabel.Text = "מספר יחידות המערכת: " + systemUnits.Count;
                SystemUnitsGrid.DataSource = systemUnits;
                SystemUnitsGrid.DataBind();
            }
        }
    }
}