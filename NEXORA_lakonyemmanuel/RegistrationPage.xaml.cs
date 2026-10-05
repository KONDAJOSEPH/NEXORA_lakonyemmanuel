using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NEXORA_lakonyemmanuel
{
    public sealed partial class RegistrationPage : Page
    {

        string connectionString = "Data Source=R103-PC02;Initial Catalog=EcommerceDB_LakonyEmmanuel;User ID=sa;Password=sap;Encrypt=False";



        public RegistrationPage()
        {
            this.InitializeComponent();
        }


        
        public async Task MessageBox(string message)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "Registration required",
                Content = message,
                CloseButtonText = "OK"
            };

            await dialog.ShowAsync();
        }


       
        private async void btncreateaccount_Click(object sender, RoutedEventArgs e)
        {
            string firstname = txtfirstname.Text;
            string lastname = txtlastname.Text;
            string phone = txtphonenumber.Text;
            string email = txtemailaddress.Text;
            string address = txtdeliveryaddress.Text;
            string city = txtcity.Text;
            string country = txtcountry.Text;
            string username = txtusername.Text;
            string password = txtpassword.Text;
            string confirmPassword = txtconfirmpassword.Text;


            
            if (string.IsNullOrWhiteSpace(firstname) ||
                string.IsNullOrWhiteSpace(lastname) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(city) ||
                string.IsNullOrWhiteSpace(country) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                await MessageBox("Please complete all registration fields");
                return;
            }


            
            if (password != confirmPassword)
            {
                await MessageBox("Password Error \n\n"+
                    "Passwords and confirm password do not match");
                return;
            }


            try
            {
                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();


                  
                    string checkQuery = @"SELECT COUNT(*) FROM Customer_tbl WHERE Username = @Username OR Email = @Email";


                    SqlCommand checkCommand =
                        new SqlCommand(checkQuery, connection);


                    checkCommand.Parameters.AddWithValue(
                        "@Username",
                        username
                    );

                    checkCommand.Parameters.AddWithValue(
                        "@Email",
                        email
                    );


                    int accountExists = (int)await checkCommand.ExecuteScalarAsync();


                    if (accountExists > 0)
                    {
                        await MessageBox(
                            "Username or email already exists."
                        );

                        return;
                    }


                    
                    string customerQuery =
                        @"INSERT INTO Customer_tbl
                        (
                            FirstName,
                            LastName,
                            PhoneNumber,
                            Email,
                            DeliveryAddress,
                            City,
                            Country,
                            Username,
                            UserPassword,
                            ConfirmPassword,
                            RegistrationDate
                        )
                        VALUES
                        (
                            @FirstName,
                            @LastName,
                            @PhoneNumber,
                            @Email,
                            @DeliveryAddress,
                            @City,
                            @Country,
                            @Username,
                            @UserPassword,
                            @ConfirmPassword,
                            @RegistrationDate
                        );

                        SELECT SCOPE_IDENTITY();";


                    SqlCommand customerCommand =
                        new SqlCommand(customerQuery, connection);


                    customerCommand.Parameters.AddWithValue("@FirstName",firstname);

                    customerCommand.Parameters.AddWithValue("@LastName",lastname);

                    customerCommand.Parameters.AddWithValue("@PhoneNumber",
                        phone
                    );

                    customerCommand.Parameters.AddWithValue("@Email",
                        email
                    );

                    customerCommand.Parameters.AddWithValue("@DeliveryAddress",
                        address
                    );

                    customerCommand.Parameters.AddWithValue("@City",
                        city
                    );

                    customerCommand.Parameters.AddWithValue("@Country",
                        country
                    );

                    customerCommand.Parameters.AddWithValue("@Username",
                        username
                    );

                    customerCommand.Parameters.AddWithValue("@UserPassword",
                        password
                    );

                    customerCommand.Parameters.AddWithValue("@ConfirmPassword",
                        confirmPassword
                    );

                    customerCommand.Parameters.AddWithValue( "@RegistrationDate",
                        DateTime.Now
                    );


                   
                    int customerID =
                        Convert.ToInt32(
                            await customerCommand.ExecuteScalarAsync()
                        );


                   
                    string loginQuery =
                        @"INSERT INTO Login_tbl
                        (
                            UserName,
                            UserPassword,
                            CustomerID,
                            LastLogin,
                            LoginStatus
                        )
                        VALUES
                        (
                            @UserName,
                            @UserPassword,
                            @CustomerID,
                            @LastLogin,
                            @LoginStatus
                        )";


                    SqlCommand loginCommand =
                        new SqlCommand(loginQuery, connection);


                    loginCommand.Parameters.AddWithValue(
                        "@UserName",
                        username
                    );

                    loginCommand.Parameters.AddWithValue(
                        "@UserPassword",
                        password
                    );

                    loginCommand.Parameters.AddWithValue(
                        "@CustomerID",
                        customerID
                    );

                    loginCommand.Parameters.AddWithValue(
                        "@LastLogin",
                        DateTime.Now
                    );

                    loginCommand.Parameters.AddWithValue(
                        "@LoginStatus",
                        "Active"
                    );


                    await loginCommand.ExecuteNonQueryAsync();


                    await MessageBox(
                        "Your NEXORA customer account has been created successfully.\n\n"+
                        "Customer ID: \n\n"+
                        "You can now login"
                    );


                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                await MessageBox(
                    "Database error: " + ex.Message
                );
            }
            Frame.Navigate(typeof(MainPage));
        }



        
        private void ClearFields()
        {
            txtfirstname.Text = "";
            txtlastname.Text = "";
            txtphonenumber.Text = "";
            txtemailaddress.Text = "";
            txtdeliveryaddress.Text = "";
            txtcity.Text = "";
            txtcountry.Text = "";
            txtusername.Text = "";
            txtpassword.Text = "";
            txtconfirmpassword.Text = "";
        }

        private void btnbacktologin_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}


 