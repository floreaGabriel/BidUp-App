using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
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
using BidUp_App.Helpers;
using BidUp_App.Models.Users;

using Microsoft.Win32;
namespace BidUp_App.Views.Bidder
{
    /// <summary>
    /// Interaction logic for BidderDashboard.xaml
    /// </summary>
    public partial class BidderDashboard : Window
    {
        private readonly Models.Users.User _user;
        public BidderDashboard(User user)
        {
            InitializeComponent();
            _user = user;
            LoadProfile();
        }

        private void LoadProfile()
        {
            
            if (!string.IsNullOrEmpty(_user.ProfilePicturePath))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_user.ProfilePicturePath, UriKind.Absolute);
                bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ProfileImage.Source = bitmap;
            }

            FullNameTextBlock.Text = _user.m_fullName;
            EmailTextBlock.Text = _user.m_email;
            DateOfBirthTextBlock.Text = _user.m_BirthDate.ToString("d"); // Format as needed
            RoleTextBlock.Text = _user.m_role;
        }

        private void SeeNewAuctionsButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void SeeLastBidsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ViewCardButton_Click(object sender, RoutedEventArgs e)
        {
            // Prompt the user to enter their password
            string password = PromptForPassword();

            // Verify password (this is just a placeholder logic; adjust for your real validation)
            if (IsPasswordValid(password))
            {
                // Display card information
                ShowCardInfo();
            }
            else
            {
                MessageBox.Show("Incorrect password. Please try again.", "Authentication Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string PromptForPassword()
        {
            // You can use a custom input dialog here. For now, let's use a simple InputBox equivalent.
            return Microsoft.VisualBasic.Interaction.InputBox("Enter your password to view card details:", "Password Required", "");
        }

        private bool IsPasswordValid(string enteredPassword)
        {
            // Here, use the actual password hash check or other secure validation
            string storedPasswordHash = _user.m_password;
            DatabaseHelper dbHelper = new DatabaseHelper();
            string enteredPasswordHash = dbHelper.HashPassword(enteredPassword);
            return enteredPasswordHash == storedPasswordHash;
        }

        private void ShowCardInfo()
        {
            Models.Users.Bidder _bidder = _user as Models.Users.Bidder;
            // Set card information from the _bidder.Card object
            CardInfoPanel.Visibility = Visibility.Visible;
            CardHolderTextBlock.Text = _bidder.card.CardHolderName.ToString();
            CardNumberTextBlock.Text = "**** **** **** " + _bidder.card.CardNumber.Substring(_bidder.card.CardNumber.Length - 4);
            ExpiryDateTextBlock.Text = _bidder.card.ExpiryDate.ToString("MM/yy");
        }
        private void ChangeProfilePicture_Click(object sender, RoutedEventArgs e)
        {
            // Open file dialog to select a new image
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                // Update the profile picture
                string selectedFilePath = openFileDialog.FileName;

                // Display the new image
                ProfileImage.Source = new BitmapImage(new Uri(selectedFilePath));

                // Update user's profile picture path
                _user.ProfilePicturePath = selectedFilePath;

                // Optionally, save this path to the database if you want it to persist
                SaveProfilePicturePathToDatabase(_user.m_userID, selectedFilePath);
            }
        }

        private void SaveProfilePicturePathToDatabase(int userId, string profilePicturePath)
        {
            DatabaseHelper dbHelper = new DatabaseHelper();
            string query = "UPDATE Users SET ProfilePicturePath = @ProfilePicturePath WHERE UserID = @UserID";
            SqlParameter[] parameters = {
            new SqlParameter("@ProfilePicturePath", profilePicturePath),
            new SqlParameter("@UserID", userId)
            };
            dbHelper.ExecuteNonQuery(query, parameters);
        }
    }
        
}

