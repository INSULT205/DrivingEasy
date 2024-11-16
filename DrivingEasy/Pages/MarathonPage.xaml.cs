using DrivingEasy.Components;
using System;
using System.Collections.Generic;
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

namespace DrivingEasy.Pages
{
    /// <summary>
    /// Логика взаимодействия для MarathonPage.xaml
    /// </summary>
    public partial class MarathonPage : Page
    {
        private User contextUser;
        private List<Questions> allQuestionsList;
        private List<QuestionButtonControl> questionButtons;
        private Dictionary<int, AnswerQuestion> selectedAnswers = new Dictionary<int, AnswerQuestion>();
        private int currentQuestionIndex = 0;
        public MarathonPage(User user)
        {
            InitializeComponent();
            contextUser = user;
            questionButtons = new List<QuestionButtonControl>();
            App.NavMenu.gridMenu.Visibility = Visibility.Collapsed;
            LoadAllQuestions();
            InitializeQuestionButtons();

            if (allQuestionsList.Count > 0)
            {
                ShowQuestion(currentQuestionIndex);
            }
        }
        private void LoadAllQuestions()
        {
            allQuestionsList = App.DB.Questions.ToList();
        }

        private void InitializeQuestionButtons()
        {
            for (int i = 1; i <= allQuestionsList.Count; i++)
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
            var question = allQuestionsList[questionIndex];

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

            if (selectedAnswers.Count == allQuestionsList.Count)
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

            int totalQuestions = allQuestionsList.Count;

            MessageBox.Show($"Марафон завершён! Вы ответили правильно на {correctAnswers} из {totalQuestions} вопросов.");
            NavigationService.Navigate(new StudyPage(contextUser)); 
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StudyPage(contextUser)); 
        }
    }
}
