using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
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
    /// Логика взаимодействия для StudyPage.xaml
    /// </summary>
    public partial class StudyPage : Page
    {
        User contextUser;
        public StudyPage(User user)
        {
            InitializeComponent();
            contextUser = user;
        }

        private void TicketsBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TicketsPage(contextUser));
        }
    }
}
