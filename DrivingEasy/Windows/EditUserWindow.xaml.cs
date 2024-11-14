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
    /// Логика взаимодействия для EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {
        List<Role> roles { get; set; }
        List<Group> groups { get; set; }
        User contextUser;
        public EditUserWindow(User user)
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
            PasswordTB.Text = contextUser.Password;

            roles = App.DB.Role.ToList();
            RoleCB.ItemsSource = roles;
            RoleCB.SelectedIndex = (int)(contextUser.RoleId - 1);
            if (contextUser.RoleId == 1)
            {
                groups = App.DB.Group.ToList();
                GroupCB.ItemsSource = groups;
                GroupCB.SelectedIndex = (int)(contextUser.GroupId - 1);
            }
            else
            {
                GroupCB.ItemsSource = null;
            }
            IsBlockedCB.IsChecked = contextUser.IsBlocked;
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

        private void SaveBT_Click(object sender, RoutedEventArgs e)
        {
            if (SurnameTB.Text == null || NameTB.Text == null || PatronymicTB.Text == null || PhoneTB.Text == null || EmailTB.Text == null ||
                PasswordTB.Text == null || RoleCB.SelectedItem == null)
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
                contextUser.IsBlocked = IsBlockedCB.IsChecked;

                App.DB.SaveChanges();
                MessageBox.Show("Изменения пользователя прошли успешно!");
                this.DialogResult = true;
                Close();
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
    }
}
