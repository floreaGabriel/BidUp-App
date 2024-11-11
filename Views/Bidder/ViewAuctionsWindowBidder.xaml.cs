using System;
using System.Linq;
using System.Windows;
using BidUp_App.Models;
using BidUp_App.Models.Users;

namespace BidUp_App.Views.Bidder
{
    public partial class ViewAuctionsWindowBidder : Window
    {
        private readonly DataContextDataContext _dbContext;
        private readonly int _currentBidderId;

        public ViewAuctionsWindowBidder(int currentBidderId)
        {
            InitializeComponent();
            _dbContext = new DataContextDataContext();
            _currentBidderId = currentBidderId;
            LoadAuctions();
        }

        private void LoadAuctions()
        {
            var auctions = _dbContext.Auctions
                .Where(a => a.EndTime > DateTime.Now) // Opțional: Afișează doar licitațiile active
                .Select(a => new AuctionViewModel
                {
                    AuctionID = a.AuctionID,
                    ProductName = a.ProductName,
                    Description = a.Description,
                    StartingPrice = a.StartingPrice,
                    CurrentPrice = a.CurrentPrice,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    ProductImagePath = a.ProductImagePath
                })
                .ToList();

            AuctionsList.ItemsSource = auctions;
        }

        private void BidButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                int auctionId = (int)button.CommandParameter;

                // Găsește licitația selectată
                var selectedAuction = _dbContext.Auctions.FirstOrDefault(a => a.AuctionID == auctionId);

                if (selectedAuction != null)
                {
                    // Deschide fereastra `BidWindow`
                    var bidWindow = new BidWindow(selectedAuction, _currentBidderId);

                    // Așteaptă finalizarea ferestrei `BidWindow`
                    if (bidWindow.ShowDialog() == true) // Se deschide modal
                    {
                        // Reîncarcă lista de licitații după plasarea unui bid
                        LoadAuctions();
                    }
                }
                else
                {
                    MessageBox.Show("Auction not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Reîncarcă lista de licitații
            LoadAuctions();
            MessageBox.Show("Auctions list refreshed successfully!", "Refresh", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
