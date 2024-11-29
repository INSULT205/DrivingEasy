using DrivingEasy.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Логика взаимодействия для LessonPage.xaml
    /// </summary>
    public partial class LessonPage : Page
    {
        User contextUser;
        public LessonPage(User user)
        {
            InitializeComponent();
            contextUser = user;

            var gridView = LessonLV.View as GridView;
            
            if (contextUser.RoleId == 1)
            {
                GroupCB.Visibility = Visibility.Collapsed;
                DeleteBT.Visibility = Visibility.Collapsed;
                EditBT.Visibility = Visibility.Collapsed;
                AddBT.Visibility = Visibility.Collapsed;
                if (gridView != null)
                {
                    var columnToRemove = gridView.Columns.FirstOrDefault(c => c.Header.ToString() == "Студент");
                    var columnToRemove1 = gridView.Columns.FirstOrDefault(c => c.Header.ToString() == "Группа");
                    if (columnToRemove != null)
                    {
                        gridView.Columns.Remove(columnToRemove);
                        gridView.Columns.Remove(columnToRemove1);
                    }
                }
            }
            else if (contextUser.RoleId == 2)
            {
                InstructorCB.Visibility = Visibility.Collapsed;
                DeleteBT.Visibility = Visibility.Collapsed;
                AddBT.Visibility = Visibility.Collapsed;
                if (gridView != null)
                {
                    var columnToRemove = gridView.Columns.FirstOrDefault(c => c.Header.ToString() == "Инструктор");
                    if (columnToRemove != null)
                    {
                        gridView.Columns.Remove(columnToRemove);
                    }
                }
            }

            var instructor = new List<User>(App.DB.User.Where(x => x.RoleId == 2).ToList());
            InstructorCB.ItemsSource = instructor;
            instructor.Insert(0, new User() { Name = "Все" });
            InstructorCB.SelectedIndex = 0;

            var group = new List<Group>(App.DB.Group.ToList());
            GroupCB.ItemsSource = group;
            group.Insert(0, new Group() { Name = "Все" });
            GroupCB.SelectedIndex = 0;

            var typeClasses = new List<TypeClass>(App.DB.TypeClass.ToList());
            TypeClassCB.ItemsSource = typeClasses;
            typeClasses.Insert(0, new TypeClass() { Name = "Все" });
            TypeClassCB.SelectedIndex = 0;

            var statuses = new List<Status>(App.DB.Status.ToList());
            StatusCB.ItemsSource = statuses;
            statuses.Insert(0, new Status() { Name = "Все" });
            StatusCB.SelectedIndex = 0;

            Refresh();
        }

        public void Refresh()
        {
            var filterLesson = App.DB.Schedule.ToList();

            if (DateDP != null && DateDP.SelectedDate != null)
                filterLesson = filterLesson.Where(x => x.ScheduleTime.Value.Date == DateDP.SelectedDate.Value.Date).ToList();

            var instructor = InstructorCB.SelectedItem as User;
            if (InstructorCB.SelectedIndex > 0 && instructor != null)
                filterLesson = filterLesson.Where(x => x.EmployeeId == instructor.Id).ToList();

            var status = StatusCB.SelectedItem as Status;
            if (StatusCB.SelectedIndex > 0 && status != null)
                filterLesson = filterLesson.Where(x => x.StatusId == status.Id).ToList();

            var typeClass = TypeClassCB.SelectedItem as TypeClass;
            if (TypeClassCB.SelectedIndex > 0 && typeClass != null)
                filterLesson = filterLesson.Where(x => x.TypeClassId == typeClass.Id).ToList();

            var group = GroupCB.SelectedItem as Group;
            if (GroupCB.SelectedIndex > 0 && group != null)
                filterLesson = filterLesson.Where(x => x.GroupId == group.Id).ToList();

            filterLesson = filterLesson.OrderBy(f => f.ScheduleTime).ToList();
            filterLesson = filterLesson.Where((f => f.ScheduleTime.Value.Date >= DateTime.Now.Date && f.ScheduleTime.Value.Date <= (DateTime.Now.Date.AddDays(31)))).ToList();

            if (contextUser.RoleId == 1)
            {
                filterLesson = filterLesson.Where(x => x.StudentId == contextUser.Id || x.GroupId == contextUser.GroupId).ToList();
            }
            else if (contextUser.RoleId == 2)
            {
                filterLesson = filterLesson.Where(x => x.EmployeeId == contextUser.Id).ToList();
            }
            LessonLV.ItemsSource = filterLesson;
        }


        private void DeleteBT_Click(object sender, RoutedEventArgs e)
        {
            var schedule = LessonLV.SelectedItem as Schedule;
            if (schedule == null)
                MessageBox.Show("Вы не выбрали занятие для удаления!");
            else
            {
                App.DB.Schedule.Remove(schedule);
                App.DB.SaveChanges();
                Refresh();
            }
        }

        private void EditBT_Click(object sender, RoutedEventArgs e)
        {
            var lesson = LessonLV.SelectedItem as Schedule;
            if (lesson == null)
                MessageBox.Show("Вы не выбрали занятие для редактирования");
            else
            {
                new EditLessonWindow(contextUser,lesson).ShowDialog();
                Refresh();
            }
        }
        private void AddBT_Click(object sender, RoutedEventArgs e)
        {
            AddLessonWindow addLessonWindow = new AddLessonWindow();
            bool? result = addLessonWindow.ShowDialog();

            if (result == true) 
            {
                Refresh();
            }
        }


        private void InstructorCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void GroupCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }
        private void TypeClassCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void StatusCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void ClearFilterBT_Click(object sender, RoutedEventArgs e)
        {
            InstructorCB.SelectedItem = null;
            DateDP.Text = "";
            GroupCB.SelectedItem = null;
            TypeClassCB.SelectedItem = null;
            StatusCB.SelectedItem = null;
        }

        private void DateDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}
