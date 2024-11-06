using BidUp_App.Helpers;
using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace BidUp_App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve email and password input
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Text;  // Assuming PasswordTextBox is a PasswordBox

            // Check if a role has been selected
            if (RoleComboBox.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a role.", "Role Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Retrieve the selected role
            string selectedRole = (RoleComboBox.SelectedItem as ComboBoxItem).Content.ToString();

            // Authenticate the user based on the selected role
            if (AuthenticateUser(email, password, selectedRole))
            {
                // Open the appropriate dashboard based on the selected role
                Window dashboard;
                switch (selectedRole)
                {
                    case "Bidder":
                        dashboard = new Views.Bidder.BidderDashboard(); // Make sure BidderDashboard exists in the Bidder namespace
                        break;
                    case "Seller":
                        dashboard = new Views.Seller.SellerDashboard(); // Make sure SellerDashboard exists in the Seller namespace
                        break;
                    case "Admin":
                        dashboard = new Views.Admin.AdminDashboard(); // Make sure AdminDashboard exists in the Admin namespace
                        break;
                    default:
                        MessageBox.Show("Invalid role selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                dashboard.Show();
                this.Close(); // Close the sign-in window
            }
            else
            {
                MessageBox.Show("Invalid credentials. Please try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_SignUp(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private bool AuthenticateUser(string email, string password, string role)
        {
            // Hash the password for comparison
            string passwordHash = DatabaseHelper.HashPassword(password); // Ensure you have HashPassword in DatabaseHelper

            // SQL query to check if the user exists with the provided email, password, and role
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND PasswordHash = @PasswordHash AND Role = @Role";
            SqlParameter[] parameters = {
                new SqlParameter("@Email", email),
                new SqlParameter("@PasswordHash", passwordHash),
                new SqlParameter("@Role", role)
            };

            DatabaseHelper dbHelper = new DatabaseHelper();
            int count = (int)dbHelper.ExecuteScalar(query, parameters);
            return count > 0; // Returns true if user exists with specified role
        }
    }
}
