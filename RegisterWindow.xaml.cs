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
            DatabaseHelper dbHelper = new DatabaseHelper();

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
            string passwordHash = dbHelper.HashPassword(password);

            // Default profile picture path
            string defaultProfilePicturePath = @"C:\Users\Florea\source\repos\BidUp-App\Resources\profil2.png";

            // SQL query for inserting new user with ProfilePicturePath
            string query = "INSERT INTO Users (FullName, PasswordHash, Role, Email, BirthDate, ProfilePicturePath, CreatedAt) " +
                           "VALUES (@FullName, @PasswordHash, @Role, @Email, @BirthDate, @ProfilePicturePath, GETDATE())";

            // Define parameters
            SqlParameter[] parameters = {
                new SqlParameter("@FullName", fullName),
                new SqlParameter("@PasswordHash", passwordHash),
                new SqlParameter("@Role", role),
                new SqlParameter("@Email", email),
                new SqlParameter("@BirthDate", birthDate.Value),
                new SqlParameter("@ProfilePicturePath", defaultProfilePicturePath)
            };

            // Insert the user into the database
            try
            {
                int result = dbHelper.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
