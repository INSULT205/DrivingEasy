using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using System;
using System.Diagnostics;
using System.Windows.Documents;
using System.Linq;

namespace DrivingEasy.Pages
{
    public partial class NewsPage : Page
    {
        public NewsPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadNewsAsync();
        }

        private async void BUpdateData_Click(object sender, RoutedEventArgs e)
        {
            await LoadNewsAsync();
        }

        private async Task LoadNewsAsync()
        {
            if (!IsInternetAvailable())
            {
                Dispatcher.Invoke(() =>
                {
                    ShowError();
                });
                return;
            }

            try
            {
                Dispatcher.Invoke(() =>
                {
                    ShowError();
                });


                await Task.Delay(2000);

                var newsItems = await Task.Run(() => LoadNewsItemsFromFeed());

                Dispatcher.Invoke(() =>
                {
                    LVExternalNews.ItemsSource = newsItems;
                    PBLoaded.Visibility = Visibility.Collapsed;
                    PStatus.Visibility = Visibility.Visible;
                    TBErrorConnect.Visibility = Visibility.Collapsed;
                });
            }
            catch
            {
                Dispatcher.Invoke(() =>
                {
                    ShowError();
                });
            }
        }

        private List<NewsItem> LoadNewsItemsFromFeed()
        {
            XDocument doc = XDocument.Load("https://www.drom.ru/cached_files/xml/news.rss");
            return doc.Descendants("item").Select(item => new NewsItem
            {
                Title = (string)item.Element("title"),
                Author = (string)item.Element("author"),
                Description = (string)item.Element("description"),
                PubDate = DateTime.Parse((string)item.Element("pubDate")),
                Link = (string)item.Element("link")
            }).ToList();
        }

        public void ShowError()
        {
            TBErrorConnect.Visibility = Visibility.Visible;
            PBLoaded.Visibility = Visibility.Visible;
            PStatus.Visibility = Visibility.Collapsed;
        }

        private bool IsInternetAvailable()
        {
            try
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
            catch
            {
                return false;
            }
        }

        private void HLToSite_Click(object sender, RoutedEventArgs e)
        {
            var newsItem = (sender as Hyperlink).DataContext as NewsItem;
            if (newsItem != null)
            {
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = newsItem.Link
                });
            }
        }

        public class NewsItem
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public string Description { get; set; }
            public string Link { get; set; }
            public DateTime PubDate { get; set; }
        }
    }

}
