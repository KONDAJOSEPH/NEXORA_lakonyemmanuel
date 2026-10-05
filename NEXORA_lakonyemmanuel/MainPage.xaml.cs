using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NEXORA_lakonyemmanuel
{
    public sealed partial class MainPage : Page
    {
        string connectionString = "Data Source=R103-PC02;Initial Catalog = EcommerceDB_LakonyEmmanuel; User ID = sa; Password=sap;Encrypt=False";

        public MainPage()
        {
            this.InitializeComponent();
        }

        public async Task MessageBox(string message)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "Login Failed",
                Content = message,
                CloseButtonText = "OK"
            };

            await dialog.ShowAsync();
        }

        private async void btnsignin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtusername.Text;
            string password = txtpassword.Password;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                await MessageBox("Enter your username and password.");
                return;
            }

            try
            {
                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string loginQuery =
                        @"SELECT CustomerID
                          FROM Login_tbl
                          WHERE UserName = @UserName
                          AND UserPassword = @UserPassword";

                    SqlCommand command =
                        new SqlCommand(loginQuery, connection);

                    command.Parameters.AddWithValue(
                        "@UserName",
                        username
                    );

                    command.Parameters.AddWithValue(
                        "@UserPassword",
                        password
                    );

                    object result =
                        await command.ExecuteScalarAsync();

                    if (result != null)
                    {
                        int customerID =
                            Convert.ToInt32(result);

                        string updateQuery =
                            @"UPDATE Login_tbl
                              SET LastLogin = @LastLogin,
                                  LoginStatus = @LoginStatus
                              WHERE CustomerID = @CustomerID";

                        SqlCommand updateCommand =
                            new SqlCommand(updateQuery, connection);

                        updateCommand.Parameters.AddWithValue(
                            "@LastLogin",
                            DateTime.Now
                        );

                        updateCommand.Parameters.AddWithValue(
                            "@LoginStatus",
                            "Active"
                        );

                        updateCommand.Parameters.AddWithValue(
                            "@CustomerID",
                            customerID
                        );

                        await updateCommand.ExecuteNonQueryAsync();

                        await MessageBox("Welcome Login successful");
                        Frame.Navigate(typeof(MenuPage));
                    }
                    else
                    {
                        await MessageBox(
                            "Inalid username or password"
                        );
                    }
                    
                }
            }
            catch (Exception ex)
            {
                await MessageBox(
                    "Database error: " + ex.Message
                );
            }
        }

        private void btncreateaccount_Click(
            object sender,
            RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegistrationPage));
        }
    }
}