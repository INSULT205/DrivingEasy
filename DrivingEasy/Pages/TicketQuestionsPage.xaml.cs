using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DrivingEasy.Components;
using DrivingEasy.Pages;

namespace DrivingEasy
{
    public partial class TicketQuestionsPage : Page
    {
        private Tickets contextTickets;
        private User contextUser;
        private List<Questions> questionsList;
        private List<QuestionButtonControl> questionButtons;
        private Dictionary<int, AnswerQuestion> selectedAnswers = new Dictionary<int, AnswerQuestion>();

        public TicketQuestionsPage(User user, Tickets tickets)
        {
            InitializeComponent();
            contextUser = user;
            contextTickets = tickets;
            questionButtons = new List<QuestionButtonControl>();

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
            for (int i = 1; i <= questionsList.Count; i++)
            {
                var questionButton = new QuestionButtonControl { QuestionNumber = i };
                questionButton.Margin = new Thickness(5);
                questionButton.QuestionButton.Click += QuestionButton_Click;
                QuestionsWp.Children.Add(questionButton);
                questionButtons.Add(questionButton);
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

            var questionDetail = new QuestionDetailControl(question, answers, contextUser, questionIndex + 1);

            questionDetail.AnswerSelected += UpdateQuestionButtonColor;

            if (selectedAnswers.ContainsKey(questionIndex))
            {
                questionDetail.SetSelectedAnswer(selectedAnswers[questionIndex]);
            }

            QuestionsFr.Content = questionDetail;
        }

        private void UpdateQuestionButtonColor(object sender, (bool isCorrect, int questionNumber, AnswerQuestion selectedAnswer) e)
        {
            var buttonControl = questionButtons[e.questionNumber - 1];
            buttonControl.SelectedAnswer = e.selectedAnswer;
            buttonControl.QuestionButton.Background = e.isCorrect ? Brushes.Green : Brushes.Red;

            selectedAnswers[e.questionNumber - 1] = e.selectedAnswer;

            SaveAnswerToDatabase(e.selectedAnswer);

            if (selectedAnswers.Count == questionsList.Count)
            {
                ShowResult();
            }
        }

        private void SaveAnswerToDatabase(AnswerQuestion selectedAnswer)
        {
            var existingAnswer = App.DB.UserAnswer
                                      .FirstOrDefault(ua => ua.UserId == contextUser.Id && ua.AnswerQuestionId == selectedAnswer.Id);

            if (existingAnswer != null)
            {
                existingAnswer.AnswerQuestionId = selectedAnswer.Id;
                App.DB.SaveChanges();
            }
            else
            {
                App.DB.UserAnswer.Add(new UserAnswer
                {
                    AnswerQuestionId = selectedAnswer.Id,
                    UserId = contextUser.Id
                });
                App.DB.SaveChanges();
            }
        }

        private void ShowResult()
        {
            int correctAnswers = selectedAnswers
                                 .Count(sa => sa.Value.IsRight == true);  

            int totalQuestions = questionsList.Count;  

            MessageBox.Show($"Вы решили {correctAnswers} из {totalQuestions} вопросов правильно.");

            NavigationService.Navigate(new TicketsPage(contextUser));
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TicketsPage(contextUser));
        }
    }
}
