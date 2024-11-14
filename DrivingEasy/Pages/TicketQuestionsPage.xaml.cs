using DrivingEasy.Components;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

namespace DrivingEasy
{
    /// <summary>
    /// Логика взаимодействия для TicketQuestionsPage.xaml
    /// </summary>
    public partial class TicketQuestionsPage : Page
    {
        Tickets contextTickets;
        User contextUser;
        private List<Questions> questionsList;
        public TicketQuestionsPage(User user, Tickets tickets)
        {
            InitializeComponent();
            contextUser = user;
            contextTickets = tickets;

            LoadQuestions();
            InitializeQuestionButtons();

            if (questionsList.Count > 0)
            {
                ShowQuestion(0);
            }
        }
        private void LoadQuestions()
        {
            var questionIds = App.DB.QuestionsTicket
                                    .Where(qt => qt.TicketId == contextTickets.Id)
                                    .Select(qt => qt.QuestionsId)
                                    .ToList();

            questionsList = App.DB.Questions
                                  .Where(q => questionIds.Contains(q.Id))
                                  .ToList();
        }
        private void InitializeQuestionButtons()
        {
            for (int i = 1; i <= 20; i++)
            {
                var questionButton = new QuestionButtonControl { QuestionNumber = i };
                questionButton.QuestionButton.Click += QuestionButton_Click;
                QuestionsWp.Children.Add(questionButton);
            }
        }

        private void QuestionButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int questionNumber = int.Parse(button.Content.ToString());
            ShowQuestion(questionNumber - 1);
        }

        private void ShowQuestion(int questionIndex)
        {
            var question = questionsList[questionIndex];

            var answers = App.DB.AnswerQuestion
                                .Where(aq => aq.QuestionId == question.Id)
                                .ToList();

            QuestionsFr.Content = new QuestionDetailControl(question, answers, contextUser, questionIndex + 1);
        }
    }
}