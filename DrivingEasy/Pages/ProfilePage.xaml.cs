using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        User contextUser;
        List<Role> roles { get; set; }
        List<Group> groups { get; set; }
        public ProfilePage(User user)
        {
            InitializeComponent();
            contextUser = user;
            DataContext = contextUser;
            Refresh();
        }

        public void Refresh()
        {
            SurnameTB.Text = contextUser.Surname;
            NameTB.Text = contextUser.Name;
            PatronymicTB.Text = contextUser.Patronymic;
            PhoneTB.Text = contextUser.Phone;
            EmailTB.Text = contextUser.Email;
            PasswordTB.Password = contextUser.Password;

            roles = App.DB.Role.ToList();
            RoleCB.ItemsSource = roles;
            RoleCB.SelectedIndex = (int)(contextUser.RoleId - 1);

            if (contextUser.RoleId == 2)
            {
                SurnameTB.IsReadOnly = true;
                NameTB.IsReadOnly = true;
                PatronymicTB.IsReadOnly = true;
                PhoneTB.IsReadOnly = true;
                RoleCB.IsHitTestVisible = false;

                GroupTB.Visibility = Visibility.Collapsed;
                GroupCB.Visibility = Visibility.Collapsed;
            }

            if (contextUser.RoleId == 1)
            {
                SurnameTB.IsReadOnly = true;
                NameTB.IsReadOnly = true;
                PatronymicTB.IsReadOnly = true;
                PhoneTB.IsReadOnly = true;
                RoleCB.IsHitTestVisible = false;
                GroupCB.IsHitTestVisible = false;

                groups = App.DB.Group.ToList();
                GroupCB.ItemsSource = groups;
                GroupCB.SelectedIndex = (int)(contextUser.GroupId - 1);
            }
            else
            {
                GroupCB.ItemsSource = null;
                GroupCB.IsEnabled = false;
            }
        }
        private void ChangePhotoBT_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog() { Filter = ".png, .jpeg, .jpg | *.png; *.jpeg; *.jpg" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var photo = File.ReadAllBytes(dialog.FileName);
                contextUser.PhotoPath = photo;
                DataContext = null;
                DataContext = contextUser;
            }
        }

        private void EditBT_Click(object sender, RoutedEventArgs e)
        {
            if (SurnameTB.Text ==  "" || NameTB.Text == "" || PatronymicTB.Text == "" || PhoneTB.Text == "" || EmailTB.Text == "" ||
               PasswordTB.Password == "" || RoleCB.SelectedItem == null)
            {
                MessageBox.Show("Вы не заполнили все поля!!!");
            }
            else
            {
                contextUser.Surname = SurnameTB.Text;
                contextUser.Name = NameTB.Text;
                contextUser.Patronymic = PatronymicTB.Text;
                contextUser.Phone = PhoneTB.Text;
                contextUser.Email = EmailTB.Text;
                contextUser.Password = PasswordTB.Password;

                var r = RoleCB.SelectedItem as Role;
                contextUser.RoleId = r.Id;
                if (r.Id == 1)
                {
                    var g = GroupCB.SelectedItem as Group;
                    contextUser.GroupId = g.Id;
                }
                else
                {
                    contextUser.GroupId = null;
                }

                App.DB.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены!");
            }
        }

        private void RegexValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
            if ((sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void RoleCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoleCB.SelectedIndex == 0)
            {
                GroupCB.IsEnabled = true;
                GroupCB.ItemsSource = App.DB.Group.ToList();
            }
            else
            {
                GroupCB.ItemsSource = null;
                GroupCB.SelectedItem = null;
                GroupCB.IsEnabled = false;
            }
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            App.MainWindow.MainFrame.Navigate(new AutorizationPage());
        }
    }
}
