using System;
using System.Linq;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;

namespace DrivingEasy.Pages
{
    public partial class StudentStatisticsPage : Page
    {
        private User contextUser;

        public StudentStatisticsPage(User user)
        {
            InitializeComponent();
            contextUser = user;
            LoadCharts();
        }

        private void LoadCharts()
        {
            var ticketData = App.DB.Tickets.Select(ticket => new
            {
                TicketName = ticket.Name,
                TotalQuestions = App.DB.QuestionsTicket.Count(qt => qt.TicketId == ticket.Id),

                TotalCorrectAnswers = App.DB.UserAnswer
                    .Where(ua => ua.UserId == contextUser.Id &&
                                 App.DB.QuestionsTicket.Any(qt => qt.TicketId == ticket.Id && qt.QuestionsId == ua.AnswerQuestion.QuestionId))
                    .Count(ua => (ua.AnswerQuestion.IsRight ?? false)),

                TotalErrors = App.DB.UserAnswer
                    .Where(ua => ua.UserId == contextUser.Id &&
                                 App.DB.QuestionsTicket.Any(qt => qt.TicketId == ticket.Id && qt.QuestionsId == ua.AnswerQuestion.QuestionId))
                    .Count(ua => !(ua.AnswerQuestion.IsRight ?? true))
            }).ToList();

            var ticketDataWithAverages = ticketData.Select(data => new
            {
                data.TicketName,
                data.TotalQuestions,
                AverageCorrectAnswers = data.TotalCorrectAnswers,
                AverageErrors = data.TotalErrors
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
                    Values = new ChartValues<int>(ticketDataWithAverages.Select(t => t.AverageCorrectAnswers))
                },
                new ColumnSeries
                {
                    Title = "Ошибки",
                    Values = new ChartValues<int>(ticketDataWithAverages.Select(t => t.AverageErrors))
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
    }
}
