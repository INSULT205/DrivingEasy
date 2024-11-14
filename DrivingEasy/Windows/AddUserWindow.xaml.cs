using DrivingEasy.Pages;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
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

namespace DrivingEasy.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        List<Group> groups { get; set; }
        List<Role> roles { get; set; }
        User contextUser;
        public AddUserWindow()
        {
            InitializeComponent();

            roles = new List<Role>(App.DB.Role.ToList());
            RoleCB.ItemsSource = roles;

            User user = new User();
            contextUser = user;
            this.DataContext = this;
        }

        private void AddBT_Click(object sender, RoutedEventArgs e)
        {
            if (SurnameTB.Text == null || NameTB.Text == null || PatronymicTB.Text == null || PhoneTB.Text == null || EmailTB.Text == null ||
                PasswordTB.Text == null || RoleCB.SelectedItem == null)
            {
                MessageBox.Show("Вы не заполнили все поля!!!");
            }
            else
            {
                Refresh();
                App.DB.User.Add(contextUser);
                App.DB.SaveChanges();
                MessageBox.Show("Вы успешно добавили нового пользователя!");
                this.DialogResult = true;
                Close();
            }
        }

        private void AddPhotoBT_Click(object sender, RoutedEventArgs e)
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
        private void RegexValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
            if ((sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void Refresh()
        {
            contextUser.Surname = SurnameTB.Text;
            contextUser.Name = NameTB.Text;
            contextUser.Patronymic = PatronymicTB.Text;
            contextUser.Phone = PhoneTB.Text;
            contextUser.Email = EmailTB.Text;
            contextUser.Password = PasswordTB.Text;
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
            contextUser.IsBlocked = false;
        }
        private void BackBT_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
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

        private void AddGroupBT_Click(object sender, RoutedEventArgs e)
        {
            AddGroupWindow addGroupWindow = new AddGroupWindow();
            addGroupWindow.ShowDialog();
        }
    }
}
