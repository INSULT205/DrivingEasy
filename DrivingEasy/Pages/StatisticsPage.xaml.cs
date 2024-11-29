using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;

namespace DrivingEasy.Pages
{
    public partial class StatisticsPage : Page
    {
        private User contextUser;
        public static List<User> students;
        public static List<Group> groups;

        public StatisticsPage(User user)
        {
            InitializeComponent();
            contextUser = user;
            AddGroup();
            AddStudent();
            LoadCharts();
        }

        private void LoadCharts()
        {
            var selectedGroup = GroupCB.SelectedItem as Group;
            int? selectedGroupId = selectedGroup != null && selectedGroup.Name != "Все" ? selectedGroup.Id : (int?)null;

            var selectedStudent = StudentCB.SelectedItem as User;
            int? selectedStudentId = selectedStudent != null && selectedStudent.Name != "Все" ? selectedStudent.Id : (int?)null;

            try
            {
                var ticketData = App.DB.Tickets.Select(ticket => new
                {
                    TicketName = ticket.Name,
                    TotalQuestions = App.DB.QuestionsTicket.Count(qt => qt.TicketId == ticket.Id),

                    TotalCorrectAnswers = App.DB.User
                        .Where(u => (selectedGroupId == null || u.GroupId == selectedGroupId) &&
                                    (selectedStudentId == null || u.Id == selectedStudentId))
                        .Sum(u =>
                            (int?)App.DB.UserAnswer
                                .Where(ua => ua.UserId == u.Id &&
                                             App.DB.QuestionsTicket.Any(qt => qt.TicketId == ticket.Id && qt.QuestionsId == ua.AnswerQuestion.QuestionId))
                                .Count(ua => (ua.AnswerQuestion.IsRight ?? false)) ?? 0),

                    TotalErrors = App.DB.User
                        .Where(u => (selectedGroupId == null || u.GroupId == selectedGroupId) &&
                                    (selectedStudentId == null || u.Id == selectedStudentId))
                        .Sum(u =>
                            (int?)App.DB.UserAnswer
                                .Where(ua => ua.UserId == u.Id &&
                                             App.DB.QuestionsTicket.Any(qt => qt.TicketId == ticket.Id && qt.QuestionsId == ua.AnswerQuestion.QuestionId))
                                .Count(ua => !(ua.AnswerQuestion.IsRight ?? true)) ?? 0),

                    TotalRespondingStudents = App.DB.User
                        .Where(u => (selectedGroupId == null || u.GroupId == selectedGroupId) &&
                                    (selectedStudentId == null || u.Id == selectedStudentId))
                        .Count(u =>
                            App.DB.UserAnswer
                                .Any(ua => ua.UserId == u.Id &&
                                           App.DB.QuestionsTicket.Any(qt => qt.TicketId == ticket.Id && qt.QuestionsId == ua.AnswerQuestion.QuestionId)))
                }).ToList();
                ticketData = ticketData.Select(data => new
                {
                    data.TicketName,
                    data.TotalQuestions,
                    TotalCorrectAnswers = data.TotalCorrectAnswers == 0 ? 0 : data.TotalCorrectAnswers,
                    TotalErrors = data.TotalErrors == 0 ? 0 : data.TotalErrors,
                    TotalRespondingStudents = data.TotalRespondingStudents == 0 ? 0 : data.TotalRespondingStudents
                }).ToList();

                var ticketDataWithAverages = ticketData.Select(data => new
                {
                    data.TicketName,
                    data.TotalQuestions,
                    AverageCorrectAnswers = data.TotalRespondingStudents > 0
                        ? (double)data.TotalCorrectAnswers / data.TotalRespondingStudents
                        : 0,
                    AverageErrors = data.TotalRespondingStudents > 0
                        ? (double)data.TotalErrors / data.TotalRespondingStudents
                        : 0
                }).ToList();

                TicketProgressChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Всего вопросов",
                    Values = new ChartValues<int>(ticketDataWithAverages.Select(t => t.TotalQuestions))
                },
                new ColumnSeries
                {
                    Title = "Правильные ответы",
                    Values = new ChartValues<double>(ticketDataWithAverages.Select(t => t.AverageCorrectAnswers))
                },
                new ColumnSeries
                {
                    Title = "Ошибки",
                    Values = new ChartValues<double>(ticketDataWithAverages.Select(t => t.AverageErrors))
                }
            };

                TicketProgressChart.AxisX.Clear();
                TicketProgressChart.AxisY.Clear();
                TicketProgressChart.AxisX.Add(new Axis
                {
                    Title = "Билеты",
                    Labels = ticketDataWithAverages.Select(t => t.TicketName).ToArray()
                });
                TicketProgressChart.AxisY.Add(new Axis
                {
                    Title = "Количество",
                    LabelFormatter = value => value.ToString("N0")
                });
            }
            catch (Exception)
            {

            }

            var sessionData = App.DB.Schedule
     .Where(s =>
         (selectedGroupId == null || s.GroupId == selectedGroupId) &&
         (selectedStudentId == null || s.StudentId == selectedStudentId) &&
         (s.StudentId == null || App.DB.User.Any(u => u.Id == s.StudentId && u.Role.Name == "Студент")))
     .GroupBy(s =>
         selectedStudentId == null && selectedGroupId == null
         ? s.GroupId
         : s.StudentId ?? s.GroupId)
     .Select(g => new
     {
         FullNameWithGroup = selectedStudentId == null && selectedGroupId == null
             ? App.DB.Group
                 .Where(group => group.Id == g.Key)
                 .Select(group => group.Name)
                 .FirstOrDefault()
             : g.Key.HasValue
                 ? App.DB.User
                     .Where(student => student.Id == g.Key && student.Role.Name == "Студент")
                     .Select(student => student.Surname + " " + student.Name + " " + student.Patronymic +
                                        " (" + App.DB.Group.Where(group => group.Id == student.GroupId)
                                                           .Select(group => group.Name)
                                                           .FirstOrDefault() + ")")
                     .FirstOrDefault()
                             : App.DB.Group
                                 .Where(group => group.Id == g.Key)
                                 .Select(group => group.Name)
                                 .FirstOrDefault(),
         SessionsCount = g.Count()
     })
     .Where(d => d.FullNameWithGroup != null)
     .ToList();

            if (selectedStudentId != null && selectedGroupId != null)
            {
                var studentSessions = App.DB.Schedule
                    .Where(s => s.StudentId == selectedStudentId)
                    .Count();

                if (!sessionData.Any(d => d.FullNameWithGroup.Contains($"{selectedStudent.Surname} {selectedStudent.Name} {selectedStudent.Patronymic}")))
                {
                    sessionData.Add(new
                    {
                        FullNameWithGroup = $"{selectedStudent.Surname} {selectedStudent.Name} {selectedStudent.Patronymic} ({selectedGroup.Name})",
                        SessionsCount = studentSessions
                    });
                }
            }
            else if (selectedStudentId != null && selectedGroupId == null)
            {
                var studentGroupId = App.DB.User.Where(u => u.Id == selectedStudentId).Select(u => u.GroupId).FirstOrDefault();
                var studentSessions = App.DB.Schedule
                    .Where(s => s.StudentId == selectedStudentId)
                    .Count();

                if (!sessionData.Any(d => d.FullNameWithGroup.Contains($"{selectedStudent.Surname} {selectedStudent.Name} {selectedStudent.Patronymic}")))
                {
                    sessionData.Add(new
                    {
                        FullNameWithGroup = $"{selectedStudent.Surname} {selectedStudent.Name} {selectedStudent.Patronymic}",
                        SessionsCount = studentSessions
                    });
                }

                if (studentGroupId != null)
                {
                    var groupSessions = App.DB.Schedule
                        .Where(s => s.GroupId == studentGroupId && s.StudentId == null)
                        .Count();

                    var groupName = App.DB.Group.Where(g => g.Id == studentGroupId).Select(g => g.Name).FirstOrDefault();

                    if (!sessionData.Any(d => d.FullNameWithGroup.Contains($"Группа: {groupName}")))
                    {
                        sessionData.Add(new
                        {
                            FullNameWithGroup = $"Группа: {groupName}",
                            SessionsCount = groupSessions
                        });
                    }
                }
            }
            else if (selectedGroupId != null)
            {
                var groupSessions = App.DB.Schedule
                    .Where(s => s.GroupId == selectedGroupId && s.StudentId == null)
                    .Count();

                sessionData.Add(new
                {
                    FullNameWithGroup = $"Группа: {selectedGroup.Name}",
                    SessionsCount = groupSessions
                });

                var studentsInGroup = App.DB.User.Where(u => u.GroupId == selectedGroupId).ToList();

                foreach (var student in studentsInGroup)
                {
                    var studentSessions = App.DB.Schedule
                        .Where(s => s.StudentId == student.Id)
                        .Count();

                    if (!sessionData.Any(d => d.FullNameWithGroup.Contains($"{student.Surname} {student.Name} {student.Patronymic} ({selectedGroup.Name})")))
                    {
                        sessionData.Add(new
                        {
                            FullNameWithGroup = $"{student.Surname} {student.Name} {student.Patronymic} ({selectedGroup.Name})",
                            SessionsCount = studentSessions
                        });
                    }
                }
            }

            SessionsCountChart.Series = new SeriesCollection
{
    new ColumnSeries
    {
        Title = "Количество занятий",
        Values = new ChartValues<int>(sessionData.Select(d => (int)d.SessionsCount))
    }
};

            SessionsCountChart.AxisX.Clear();
            SessionsCountChart.AxisY.Clear();
            SessionsCountChart.AxisX.Add(new Axis
            {
                Title = "Студенты и группы",
                Labels = sessionData.Select(d => (string)d.FullNameWithGroup).ToArray()
            });
            SessionsCountChart.AxisY.Add(new Axis
            {
                Title = "Количество занятий",
                LabelFormatter = value => value.ToString("N0")
            });

        }
        private void GroupCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupCB.SelectedIndex > 0)
            {
                var students = new List<User>(App.DB.User.Where(x => x.Role.Name == "Студент").ToList());
                var groupStudent = GroupCB.SelectedItem as Group;
                StudentCB.ItemsSource = students.Where(x => x.GroupId == groupStudent.Id).ToList();
            }
            else
            {
                AddStudent();
            }
            LoadCharts();
        }

        private void StudentCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadCharts();
        }

        private void AddGroup()
        {
            var groups = new List<Group>(App.DB.Group.ToList());
            groups.Insert(0, new Group() { Name = "Все" });
            GroupCB.ItemsSource = groups;
            GroupCB.SelectedIndex = 0;
        }
        private void AddStudent()
        {
            var students = new List<User>(App.DB.User.Where(x => x.Role.Name == "Студент").ToList());
            students.Insert(0, new User() { Name = "Все" });
            StudentCB.ItemsSource = students;
            StudentCB.SelectedIndex = 0;
        }
    }
}
