using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DrivingEasy.Pages
{
    /// <summary>
    /// Логика взаимодействия для TicketsPage.xaml
    /// </summary>
    public partial class TicketsPage : Page
    {
        User contextUser;
        public TicketsPage(User user)
        {
            InitializeComponent();
            contextUser = user;
            TicketsLV.ItemsSource = App.DB.Tickets.ToList();
        }

        private void TicketsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigationService.Navigate(new TicketQuestionsPage(contextUser,TicketsLV.SelectedItem as Tickets));
        }
    }
}
