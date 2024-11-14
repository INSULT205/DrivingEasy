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
using System.Windows.Shapes;

namespace DrivingEasy.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddGroupWindow.xaml
    /// </summary>
    public partial class AddGroupWindow : Window
    {
        public AddGroupWindow()
        {
            InitializeComponent();
        }

        private void AddGroupBT_Click(object sender, RoutedEventArgs e)
        {
            if (GroupTB.Text == "")
            {
                MessageBox.Show("Заполните поле!");
            }
            else
            {
                Group group = new Group();
                group.Name = GroupTB.Text;
                App.DB.Group.Add(group);
                App.DB.SaveChanges();
                MessageBox.Show("Вы успешно добавили новую группу!");
                this.DialogResult = true;
            }
        }

        private void BackBT_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
    }
}
