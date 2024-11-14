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
    /// Логика взаимодействия для AddLessonWindow.xaml
    /// </summary>
    public partial class AddLessonWindow : Window
    {
        public AddLessonWindow()
        {
            InitializeComponent();

            var instructor = new List<User>(App.DB.User.Where(x => x.RoleId == 2).ToList());
            InstructorCB.ItemsSource = instructor;

            var typeClasses = new List<TypeClass>(App.DB.TypeClass.ToList());
            TypeClassesCB.ItemsSource = typeClasses;
        }

        private void BackBT_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void AddBT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string error = "";
                if (InstructorCB.SelectedItem == null)
                    error += "Выберите инструктора\n";
                if (TypeClassesCB.SelectedItem == null)
                    error += "Выберите вид занятия \n";
                if (DateDP.SelectedDate == null)
                    error += "Выберите дату\n";
                if (String.IsNullOrWhiteSpace(TimeDP.Text))
                    error += "Выберите время оказания услуги\n";
                if (StudentCB.SelectedItem == null && GroupCB.Items == null)
                    error += "Выберите студента\n";
                if (GroupCB.SelectedItem == null && StudentCB.Items == null)
                    error += "Выберите группу\n";
                if (String.IsNullOrWhiteSpace(error) == false)
                {
                    MessageBox.Show(error);
                    return;
                }
                Schedule schedule = new Schedule();
                schedule.ScheduleTime = DateDP.SelectedDate.Value + DateTime.Parse(TimeDP.Text).TimeOfDay;
                var t = TypeClassesCB.SelectedItem as TypeClass;
                schedule.TypeClassId = t.Id;
                var u = InstructorCB.SelectedItem as User;
                schedule.EmployeeId = u.Id;
                if (TypeClassesCB.SelectedIndex == 0)
                {
                    var g = GroupCB.SelectedItem as Group;
                    schedule.GroupId = g.Id;
                    schedule.StudentId = null;
                }
                else
                {
                    var s = StudentCB.SelectedItem as User;
                    schedule.StudentId = s.Id;
                    schedule.GroupId = null;
                }
                schedule.StatusId = 1;
                App.DB.Schedule.Add(schedule);
                App.DB.SaveChanges();
                MessageBox.Show("Вы успешно добавили новое занятие");
                this.DialogResult = true;
                Close();
            }
            catch
            {
                MessageBox.Show("Пожалуйста, удостовертесь в правильности всех данных. Возникла какая-то ошибка!");
            }
        }

        private void TypeClassesCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeClassesCB.SelectedIndex == 0)
            {
                GroupCB.IsEnabled = true;
                GroupCB.ItemsSource = App.DB.Group.ToList();

                StudentCB.ItemsSource = null;
                StudentCB.SelectedItem = null;
                StudentCB.IsEnabled = false;
            }
            else
            {
                StudentCB.IsEnabled = true;
                var student = new List<User>(App.DB.User.Where(x => x.RoleId == 1).ToList());
                StudentCB.ItemsSource = student;

                GroupCB.ItemsSource = null;
                GroupCB.SelectedItem = null;
                GroupCB.IsEnabled = false;
            }
        }
    }
}
