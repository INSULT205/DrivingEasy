using DrivingEasy.Windows;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DrivingEasy.Pages.Admin
{
    /// <summary>
    /// Логика взаимодействия для UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();

            var role = new List<Role>(App.DB.Role.ToList());
            RoleCB.ItemsSource = role;
            role.Insert(0, new Role() { Name = "Все" });
            RoleCB.SelectedIndex = 0;

            var group = new List<Group>(App.DB.Group.ToList());
            GroupCB.ItemsSource = group;
            group.Insert(0, new Group() { Name = "Все" });
            GroupCB.SelectedIndex = 0;

            Refresh();
        }

        public void Refresh()
        {
            var filterProduct = App.DB.User.ToList();
            if (SearchTB.Text.Length > 0)
            {
                filterProduct = filterProduct
                    .Where(i => i.FullName.ToLower().Contains(SearchTB.Text.Trim().ToLower()))
                    .ToList();
            }

            var role = RoleCB.SelectedItem as Role;
            if (RoleCB.SelectedIndex > 0 && role != null)
                filterProduct = filterProduct.Where(x => x.RoleId == role.Id).ToList();

            var group = GroupCB.SelectedItem as Group;
            if (GroupCB.SelectedIndex > 0 && group != null)
                filterProduct = filterProduct.Where(x => x.GroupId == group.Id).ToList();

            UserRecordingLV.ItemsSource = filterProduct;
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            Refresh();
        }

        private void RoleCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void GroupCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void DeleteBT_Click(object sender, RoutedEventArgs e)
        {
            var users = UserRecordingLV.SelectedItem as User;
            if (users == null)
                MessageBox.Show("Вы не выбрали пользователя для удаления!");
            else
            {
                App.DB.User.Remove(users);
                App.DB.SaveChanges();
                Refresh();
            }
        }

        private void EditBT_Click(object sender, RoutedEventArgs e)
        {
            var users = UserRecordingLV.SelectedItem as User;
            if (users == null)
                MessageBox.Show("Вы не выбрали пользователя для редактирования");
            else
            {
                new EditUserWindow(users).ShowDialog();
                Refresh();
            }
        }

        private void AddUserBT_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow addUserWindow = new AddUserWindow();
            bool? result = addUserWindow.ShowDialog();

            if (result == true)
            {
                Refresh();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}
