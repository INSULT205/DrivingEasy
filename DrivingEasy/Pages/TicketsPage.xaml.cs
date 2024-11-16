using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DrivingEasy.Pages;

namespace DrivingEasy.Pages
{
    public partial class TicketsPage : Page
    {
        User contextUser;
        int typeStudy;

        public TicketsPage(User user, int typeStudy)
        {
            InitializeComponent();
            contextUser = user;
            this.typeStudy = typeStudy;
            App.NavMenu.gridMenu.Visibility = Visibility.Visible;
            TicketsLV.ItemsSource = GetTicketsWithResult();
        }

        public IEnumerable<Tickets> GetTicketsWithResult()
        {
            var tickets = App.DB.Tickets.ToList();

            foreach (var ticket in tickets)
            {
                var questionIds = App.DB.QuestionsTicket
                    .Where(qt => qt.TicketId == ticket.Id)
                    .Select(qt => qt.QuestionsId)
                    .ToList();

                var userAnswers = App.DB.UserAnswer
                    .Where(ua => ua.UserId == contextUser.Id && questionIds.Contains(ua.AnswerQuestion.QuestionId))
                    .ToList();

                if (userAnswers.Count == 0)
                {
                    ticket.BackgroundColor = Brushes.Transparent;
                }
                else
                {
                    bool allCorrect = userAnswers.All(ua => ua.AnswerQuestion.IsRight == true);
                    bool hasIncorrect = userAnswers.Any(ua => ua.AnswerQuestion.IsRight == false);

                    if (allCorrect)
                    {
                        ticket.BackgroundColor = Brushes.Green;
                    }
                    else if (hasIncorrect)
                    {
                        ticket.BackgroundColor = Brushes.Red;
                    }
                    else
                    {
                        ticket.BackgroundColor = Brushes.Transparent;
                    }
                }
            }

            if (typeStudy == 2)
            {
                return tickets.Where(ticket => ticket.BackgroundColor == Brushes.Red);
            }
            else
            {
                return tickets;
            }
        }

        private void TicketsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTicket = TicketsLV.SelectedItem as Tickets;
            if (selectedTicket != null)
            {
                NavigationService.Navigate(new TicketQuestionsPage(contextUser, selectedTicket, typeStudy));
            }
        }
    }
}
