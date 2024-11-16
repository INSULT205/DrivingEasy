using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DrivingEasy.Pages;
using System.Windows.Media;

namespace DrivingEasy.Pages
{
    /// <summary>
    /// Логика взаимодействия для StudyPage.xaml
    /// </summary>
    public partial class StudyPage : Page
    {
        private User contextUser;

        public StudyPage(User user)
        {
            InitializeComponent();
            contextUser = user;
            App.NavMenu.gridMenu.Visibility = Visibility.Visible;
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            var userAnswers = App.DB.UserAnswer.Where(ua => ua.UserId == contextUser.Id).ToList();

            int totalQuestions = App.DB.Questions.Count();

            int correctAnswers = userAnswers.Count(ua => ua.AnswerQuestion.IsRight == true);

            QuestionCountTB.Text = $"{correctAnswers} из {totalQuestions}";

            var allTickets = App.DB.Tickets.ToList();
            int solvedTickets = allTickets.Count(ticket => {
                var questionIds = App.DB.QuestionsTicket
                                       .Where(qt => qt.TicketId == ticket.Id)
                                       .Select(qt => qt.QuestionsId)
                                       .ToList();
                var ticketAnswers = userAnswers.Where(ua => questionIds.Contains(ua.AnswerQuestion.QuestionId)).ToList();
                return ticketAnswers.Count > 0 && ticketAnswers.All(ua => ua.AnswerQuestion.IsRight == true);
            });
            TicketsCountTB.Text = $"{solvedTickets} из {allTickets.Count}";

            int incorrectAnswers = userAnswers.Count(ua => ua.AnswerQuestion.IsRight == false);
            MistakeTB.Text = incorrectAnswers.ToString();
        }



        private void TicketsBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TicketsPage(contextUser, 1));
        }

        private void MistakeBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TicketsPage(contextUser, 2));
        }

        private void MarathonBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MarathonPage(contextUser));
        }

        private void ExamBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ExamPage(contextUser));
        }
    }
}
