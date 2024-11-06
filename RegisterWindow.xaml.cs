using BidUp_App.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BidUp_App
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Toggle the visibility of the password placeholder based on the PasswordBox content
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
        }


        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Toggle the visibility of the confirm password placeholder based on the PasswordBox content
            ConfirmPasswordPlaceholder.Visibility = string.IsNullOrEmpty(ConfirmPasswordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the input values from the fields
            string fullName = FullNameTextBox.Text;  // Assuming FullNameTextBox is your input field name
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            DateTime? birthDate = DateOfBirthPicker.SelectedDate;

            // Validate fields
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || !birthDate.HasValue)
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check if passwords match
            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check if a role is selected
            if (RoleComboBox.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a role.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Retrieve selected role
            string role = (RoleComboBox.SelectedItem as ComboBoxItem).Content.ToString();

            // Hash the password
            string passwordHash = DatabaseHelper.HashPassword(password);

            // SQL query for inserting new user
            string query = "INSERT INTO Users (FullName, PasswordHash, Role, Email, BirthDate, CreatedAt) " +
                           "VALUES (@FullName, @PasswordHash, @Role, @Email, @BirthDate, GETDATE())";

            // Define parameters
            SqlParameter[] parameters = {
            new SqlParameter("@FullName", fullName),
            new SqlParameter("@PasswordHash", passwordHash),
            new SqlParameter("@Role", role),
            new SqlParameter("@Email", email),
            new SqlParameter("@BirthDate", birthDate.Value)
            };

            // Insert the user into the database
            try
            {
                DatabaseHelper dbHelper = new DatabaseHelper();
                int result = dbHelper.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Navigate to the appropriate dashboard based on the role
                    Window dashboard;
                    switch (role)
                    {
                        case "Bidder":
                            dashboard = new Views.Bidder.BidderDashboard();
                            break;
                        case "Seller":
                            dashboard = new Views.Seller.SellerDashboard();
                            break;
                        case "Admin":
                            dashboard = new Views.Admin.AdminDashboard();
                            break;
                        default:
                            MessageBox.Show("Invalid role selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                    }

                    dashboard.Show();
                    this.Close();  // Close the register window
                }
                else
                {
                    MessageBox.Show("Registration failed. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BackToSignInButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the MainWindow
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

    }
}
