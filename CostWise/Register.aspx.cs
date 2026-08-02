using System;
using CostWise.App_Code.BLL;

namespace CostWise
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void RegisterButton_Click(object sender, EventArgs e)
        {
            string businessName = BusinessNameTextBox.Text.Trim();
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordTextBox.Text;
            string confirmPassword = ConfirmPasswordTextBox.Text;
            try
            {
                RegistrationBLL.Register(businessName, username, password, confirmPassword);
                RegistrationResultLabel.Text = "ההרשמה הושלמה בהצלחה.";
            }
            catch (ArgumentException ex)
            {
                RegistrationResultLabel.Text = ex.Message;
            }
            catch (InvalidOperationException)
            {
                RegistrationResultLabel.Text = "שם המשתמש כבר קיים במערכת.";
            }
            catch (Exception)
            {
                RegistrationResultLabel.Text = "אירעה שגיאה בעת ההרשמה. נסה שוב.";
            }
        }
    }
}