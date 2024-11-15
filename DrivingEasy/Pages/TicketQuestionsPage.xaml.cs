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
        private int typeStudy;

        public TicketQuestionsPage(User user, Tickets tickets, int typeStudy)
        {
            InitializeComponent();
            contextUser = user;
            contextTickets = tickets;
            this.typeStudy = typeStudy;
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

            if (typeStudy == 2)
            {
                var incorrectQuestions = App.DB.UserAnswer
                   .Where(ua => ua.UserId == contextUser.Id && ua.AnswerQuestion.IsRight == false && questionIds.Contains(ua.AnswerQuestion.QuestionId))
                   .Select(ua => ua.AnswerQuestion.QuestionId)
                   .Distinct()
                   .ToList();

                questionsList = questionsList.Where(q => incorrectQuestions.Contains(q.Id)).ToList();

                if (questionsList.Count == 0)
                {
                    MessageBox.Show("Вы исправили все ошибки в данном билете.");
                    NavigationService.Navigate(new TicketsPage(contextUser, 2));
                }
            }
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

            var questionId = questionsList[e.questionNumber - 1].Id;
            var correctAnswer = App.DB.AnswerQuestion
                                      .Where(aq => aq.QuestionId == questionId && aq.IsRight == true)
                                      .SingleOrDefault(); 

            if (correctAnswer != null && correctAnswer.Id == e.selectedAnswer.Id)
            {
                buttonControl.QuestionButton.Background = Brushes.Green;
            }
            else
            {
                buttonControl.QuestionButton.Background = Brushes.Red;
            }

            selectedAnswers[e.questionNumber - 1] = e.selectedAnswer;

            SaveAnswerToDatabase(e.selectedAnswer);

            if (selectedAnswers.Count == questionsList.Count)
            {
                if (typeStudy == 2 && selectedAnswers.Values.All(ans => ans.IsRight == true))
                {
                    MessageBox.Show("Вы исправили все ошибки в данном билете.");
                    NavigationService.Navigate(new TicketsPage(contextUser, 2));
                }
                else
                {
                    ShowProgress();
                }
            }
        }

        private void SaveAnswerToDatabase(AnswerQuestion selectedAnswer)
        {
            var existingAnswer = App.DB.UserAnswer
                                      .FirstOrDefault(ua => ua.UserId == contextUser.Id && ua.AnswerQuestion.QuestionId == selectedAnswer.QuestionId);

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

        private void ShowProgress()
        {
            int correctAnswers = selectedAnswers.Count(sa => sa.Value.IsRight == true);
            int totalQuestions = questionsList.Count;

            if (correctAnswers < totalQuestions)
            {
                MessageBox.Show($"Вы исправили {correctAnswers} из {totalQuestions} вопросов.");
                NavigationService.Navigate(new TicketsPage(contextUser, typeStudy));
            }
            else
            {
                ShowResult();
            }
        }

        private void ShowResult()
        {
            int correctAnswers = selectedAnswers.Count(sa => sa.Value.IsRight == true);
            int totalQuestions = questionsList.Count;

            MessageBox.Show($"Вы решили {correctAnswers} из {totalQuestions} вопросов правильно.");
            NavigationService.Navigate(new TicketsPage(contextUser, typeStudy));
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TicketsPage(contextUser, typeStudy));
        }
    }
}
