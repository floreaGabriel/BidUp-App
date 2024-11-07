using BidUp_App.Helpers;
using BidUp_App.Models.Users;
using System;
using System.Data;
using System.Data.Common;
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

            DatabaseHelper dbHelper = new DatabaseHelper();
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Text;

            // Hash the password (assuming you have a method to hash it)
            string passwordHash = dbHelper.HashPassword(password);

            // Retrieve the user data from the database
            DataRow userData = dbHelper.GetUserByEmailAndPassword(email, passwordHash);

            if (userData != null)
            {
                // Extract the role from the database
                string role = userData["Role"].ToString();

                // Use the factory to create the appropriate user object based on the role
                User user = UserFactory.CreateUser(role);

                // Populate the user object with data from the database
                user.m_userID = Convert.ToInt32(userData["UserID"]);
                user.m_fullName = userData["FullName"].ToString();
                user.m_email = userData["Email"].ToString();
                user.m_BirthDate = Convert.ToDateTime(userData["BirthDate"]);
                user.ProfilePicturePath = userData["ProfilePicturePath"].ToString();
                user.m_password = userData["PasswordHash"].ToString();
                // Display the appropriate dashboard
                user.displayDasboard();

                // Close the SignIn window
                this.Close();
            }
            else
            {
                // Show error if the user is not found
                MessageBox.Show("Invalid email or password. Please try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_SignUp(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private bool AuthenticateUser(string email, string password, string role, DatabaseHelper dbHelper)
        {
            // Hash the password for comparison


            string passwordHash = dbHelper.HashPassword(password); // Ensure you have HashPassword in DatabaseHelper

            // SQL query to check if the user exists with the provided email, password, and role
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND PasswordHash = @PasswordHash AND Role = @Role";
            SqlParameter[] parameters = {
                new SqlParameter("@Email", email),
                new SqlParameter("@PasswordHash", passwordHash),
                new SqlParameter("@Role", role)
            };

            int count = (int)dbHelper.ExecuteScalar(query, parameters);
            return count > 0; // Returns true if user exists with specified role
        }
    }
}
