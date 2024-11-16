using DrivingEasy.Pages.Admin;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DrivingEasy.Pages
{
    /// <summary>
    /// Логика взаимодействия для NavMenu.xaml
    /// </summary>
    public partial class NavMenu : Page
    {
        User contextUser;
        public NavMenu(User user)
        {
            InitializeComponent();
            App.NavMenu = this;
            contextUser = user;
            if (contextUser.RoleId == 1)
            {
                UserST.Visibility = Visibility.Collapsed;
            }
            if (contextUser.RoleId == 2)
            {
                UserST.Visibility = Visibility.Collapsed;
                TicketST.Visibility = Visibility.Collapsed;
             
            }
            if (contextUser.RoleId == 3)
            {
                TicketST.Visibility = Visibility.Collapsed;
                StaticticST.Visibility = Visibility.Collapsed;
            }
            MenuBT.Checked += MenuBT_Checked;
            MenuBT.Unchecked += MenuBT_Unchecked;
            ContentFrame.Navigate(new NewsPage());
        }

        private void MenuBT_Checked(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 60,
                To = 200,
                Duration = new Duration(TimeSpan.FromSeconds(0.3))
            };
            gridMenu.BeginAnimation(WidthProperty, animation);
        }
        private void MenuBT_Unchecked(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 200,
                To = 60,
                Duration = new Duration(TimeSpan.FromSeconds(0.3))
            };
            gridMenu.BeginAnimation(WidthProperty, animation);
        }

        private void AccountBT_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new ProfilePage(contextUser));
        }

        private void LessontBT_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new LessonPage(contextUser));
        }

        private void NewsPageBT_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new NewsPage());
        }

        private void NewsPageTB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Navigate(new NewsPage());
        }

        private void UserPageTB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Navigate(new UserPage());
        }

        private void LessontTB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Navigate(new LessonPage(contextUser));
        }

        private void AccountTB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Navigate(new ProfilePage(contextUser));
        }

        private void UsersPageBT_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new UserPage());
        }

        private void TicketBT_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new StudyPage(contextUser));
        }

        private void TicketBT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Navigate(new StudyPage(contextUser));
        }

        private void StaticticTB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void StaticticBT_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
