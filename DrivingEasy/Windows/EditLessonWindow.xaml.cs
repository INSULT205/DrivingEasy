using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace DrivingEasy.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditLessonWindow.xaml
    /// </summary>
    public partial class EditLessonWindow : Window
    {
        Schedule contextLesson;
        User contextUser;
        public EditLessonWindow(User user, Schedule lesson)
        {
            InitializeComponent();
            contextUser = user;
            contextLesson = lesson;
            DataContext = contextLesson;
            Refresh();
           
        }

        public void Refresh()
        {
            if(contextUser.RoleId == 2)
            {
                GroupCB.IsHitTestVisible = false;
                StudentCB.IsHitTestVisible = false;
                InstructorCB.IsHitTestVisible = false;
                InstructorCB.Visibility = Visibility.Collapsed;
                TypeClassesCB.IsHitTestVisible = false;
            }


            var instructor = new List<User>(App.DB.User.Where(x => x.RoleId == 2).ToList());
            InstructorCB.ItemsSource = instructor;
            InstructorCB.SelectedItem = instructor.FirstOrDefault(x => x.Id == contextLesson.EmployeeId);

            var typeClasses = new List<TypeClass>(App.DB.TypeClass.ToList());
            TypeClassesCB.ItemsSource = typeClasses;
            TypeClassesCB.SelectedIndex = (int)(contextLesson.TypeClassId - 1);

            var statuses = new List<Status>(App.DB.Status.ToList());
            StatusCB.ItemsSource = statuses;
            StatusCB.SelectedIndex = (int)(contextLesson.StatusId - 1);

            if (TypeClassesCB.SelectedIndex == 0)
            {
                GroupCB.IsEnabled = true;
                GroupCB.ItemsSource = App.DB.Group.ToList();
                GroupCB.SelectedIndex = (int)(contextLesson.TypeClassId - 1);

                StudentCB.ItemsSource = null;
                StudentCB.SelectedItem = null;
                StudentCB.IsEnabled = false;
            }
            else
            {
                StudentCB.IsEnabled = true;
                var student = new List<User>(App.DB.User.Where(x => x.RoleId == 1).ToList());
                StudentCB.ItemsSource = student;
                StudentCB.SelectedItem = student.FirstOrDefault(x => x.Id == contextLesson.StudentId);

                GroupCB.ItemsSource = null;
                GroupCB.SelectedItem = null;
                GroupCB.IsEnabled = false;
            }

            DateDP.SelectedDate = contextLesson.ScheduleTime.Value.Date;
            TimeDP.Text = contextLesson.ScheduleTime.Value.TimeOfDay.ToString();
        }
        private void BackBT_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
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

        private void EditBT_Click(object sender, RoutedEventArgs e)
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
                if ((TypeClassesCB.SelectedItem as TypeClass).Name == "Практическое" && StudentCB.SelectedItem == null)
                    error += "Выберите студента\n";
                if (GroupCB.SelectedItem == null && (TypeClassesCB.SelectedItem as TypeClass).Name == "Теоретическое")
                    error += "Выберите группу\n";
                if (String.IsNullOrWhiteSpace(error) == false)
                {
                    MessageBox.Show(error);
                    return;
                }
                contextLesson.ScheduleTime = DateDP.SelectedDate.Value + DateTime.Parse(TimeDP.Text).TimeOfDay;
                var t = TypeClassesCB.SelectedItem as TypeClass;
                contextLesson.TypeClassId = t.Id;
                var u = InstructorCB.SelectedItem as User;
                contextLesson.EmployeeId = u.Id;
                if (TypeClassesCB.SelectedIndex == 0)
                {
                    var g = GroupCB.SelectedItem as Group;
                    contextLesson.GroupId = g.Id;
                    contextLesson.StudentId = null;
                }
                else
                {
                    var s = StudentCB.SelectedItem as User;
                    contextLesson.StudentId = s.Id;
                    contextLesson.GroupId = null;
                }
                var p = StatusCB.SelectedItem as Status;
                contextLesson.StatusId = p.Id;
                App.DB.SaveChanges();
                MessageBox.Show("Вы успешно отредактировали занятие");
                this.DialogResult = true;
                Close();
            }
            catch
            {
                MessageBox.Show("Пожалуйста, удостовертесь в правильности всех данных. Возникла какая-то ошибка!");
            }
        }

    }
}
