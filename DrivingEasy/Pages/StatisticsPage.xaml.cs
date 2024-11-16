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

        public StatisticsPage(User user)
        {
            InitializeComponent();
            contextUser = user;

            // Загрузка списка групп с возможностью выбора "Все"
            var groupList = new List<Group>(App.DB.Group.ToList());
            groupList.Insert(0, new Group() { Name = "Все" });
            GroupCB.ItemsSource = groupList;
            GroupCB.SelectedIndex = 0;

            LoadCharts(); // Начальная загрузка графиков
        }

        private void LoadCharts()
        {
            // Получение выбранной группы
            var selectedGroup = GroupCB.SelectedItem as Group;
            int? selectedGroupId = selectedGroup != null && selectedGroup.Name != "Все" ? selectedGroup.Id : (int?)null;

            // Данные для графика: прогресс по билетам
            var ticketData = App.DB.Tickets.Select(ticket => new
            {
                TicketName = ticket.Name,
                TotalQuestions = App.DB.QuestionsTicket.Count(qt => qt.TicketId == ticket.Id),
                CorrectAnswers = App.DB.UserAnswer
         .Where(ua => (ua.AnswerQuestion.IsRight ?? false) && // Обработка nullable
                      App.DB.QuestionsTicket.Any(qt => qt.TicketId == ticket.Id && qt.QuestionsId == ua.AnswerQuestion.QuestionId) &&
                      (selectedGroupId == null || App.DB.User
                          .Where(u => u.Id == ua.UserId)
                          .Select(u => u.GroupId)
                          .FirstOrDefault() == selectedGroupId)) // Фильтрация по группе
         .Count(),
                TotalErrors = App.DB.UserAnswer
         .Where(ua => (!(ua.AnswerQuestion.IsRight ?? true)) && // Обработка nullable
                      App.DB.QuestionsTicket.Any(qt => qt.TicketId == ticket.Id && qt.QuestionsId == ua.AnswerQuestion.QuestionId) &&
                      (selectedGroupId == null || App.DB.User
                          .Where(u => u.Id == ua.UserId)
                          .Select(u => u.GroupId)
                          .FirstOrDefault() == selectedGroupId))
         .Count()
            }).ToList();


            // Привязка данных для графика прогресса по билетам
            TicketProgressChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Всего вопросов",
                    Values = new ChartValues<int>(ticketData.Select(t => t.TotalQuestions))
                },
                new ColumnSeries
                {
                    Title = "Правильных ответов",
                    Values = new ChartValues<int>(ticketData.Select(t => t.CorrectAnswers))
                },
                new ColumnSeries
                {
                    Title = "Ошибок",
                    Values = new ChartValues<int>(ticketData.Select(t => t.TotalErrors))
                }
            };
            TicketProgressChart.AxisX.Add(new Axis
            {
                Labels = ticketData.Select(t => t.TicketName).ToArray()
            });
            TicketProgressChart.AxisY.Add(new Axis
            {
                Title = "Количество",
                LabelFormatter = value => value.ToString("N0")
            });

            var sessionData = App.DB.Schedule
                .Where(s => (selectedGroupId == null || s.GroupId == selectedGroupId) && s.EmployeeId == contextUser.Id) 
                .GroupBy(s => s.GroupId)
                .Select(g => new
                {
                    Name = App.DB.Group.Where(group => group.Id == g.Key).Select(group => group.Name).FirstOrDefault(), 
                    SessionsCount = g.Count()
                })
                .ToList();



            SessionsCountChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Количество занятий",
                    Values = new ChartValues<int>(sessionData.Select(d => d.SessionsCount))
                }
            };
            SessionsCountChart.AxisX.Add(new Axis
            {
                Labels = sessionData.Select(d => d.Name).ToArray()
            });
            SessionsCountChart.AxisY.Add(new Axis
            {
                Title = "Количество занятий",
                LabelFormatter = value => value.ToString("N0")
            });
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadCharts(); 
        }

        private void GroupCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadCharts(); 
        }
    }
}
