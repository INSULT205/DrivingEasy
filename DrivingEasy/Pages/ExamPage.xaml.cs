using DrivingEasy.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace DrivingEasy.Pages
{
    public partial class ExamPage : Page
    {
        User contextUser;
        private List<Questions> examQuestions;
        private Dictionary<int, AnswerQuestion> selectedAnswers = new Dictionary<int, AnswerQuestion>();
        private List<QuestionButtonControl> questionButtons;
        private DispatcherTimer examTimer;
        private TimeSpan remainingTime = TimeSpan.FromMinutes(20);
        private int incorrectAnswerCount = 0;

        public ExamPage(User user)
        {
            InitializeComponent();
            App.NavMenu.gridMenu.Visibility = Visibility.Collapsed;
            contextUser = user;
            questionButtons = new List<QuestionButtonControl>();
            LoadExamQuestions();
            InitializeQuestionButtons();
            StartExamTimer();

            if (examQuestions.Count > 0)
            {
                ShowQuestion(0);
            }
        }

        private void LoadExamQuestions()
        {
            var allQuestions = App.DB.Questions.ToList();
            var random = new Random();
            examQuestions = allQuestions.OrderBy(q => random.Next()).Take(20).ToList();
        }

        private void InitializeQuestionButtons()
        {
            for (int i = 1; i <= examQuestions.Count; i++)
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
            var question = examQuestions[questionIndex];
            var answers = App.DB.AnswerQuestion
                                .Where(aq => aq.QuestionId == question.Id)
                                .ToList();

            var questionDetail = new QuestionDetailControl(question, answers, null, questionIndex + 1);

            if (selectedAnswers.TryGetValue(questionIndex, out var selectedAnswer))
            {
                questionDetail.SetSelectedAnswer(selectedAnswer);
            }

            questionDetail.AnswerSelected += UpdateQuestionButtonColor;
            QuestionsFr.Content = questionDetail;
        }

        private void UpdateQuestionButtonColor(object sender, (bool isCorrect, int questionNumber, AnswerQuestion selectedAnswer) e)
        {
            selectedAnswers[e.questionNumber - 1] = e.selectedAnswer;

            var buttonControl = questionButtons[e.questionNumber - 1];
            buttonControl.QuestionButton.Background = e.isCorrect ? Brushes.Green : Brushes.Red;

            if (!e.isCorrect)
            {
                incorrectAnswerCount++;

                if (incorrectAnswerCount == 3)
                {
                    examTimer.Stop();
                    MessageBox.Show("Экзамен не сдан. Вы выбрали три неверных ответа. Просмотрите свои варианты ответов");
                    EndExam();
                    return;
                }

                AddExtraQuestions();
            }
        }

        private void AddExtraQuestions()
        {
            var allQuestions = App.DB.Questions.ToList();
            var random = new Random();
            var extraQuestions = allQuestions
                                 .Except(examQuestions)
                                 .OrderBy(q => random.Next())
                                 .Take(5)
                                 .ToList();

            int currentQuestionCount = examQuestions.Count;
            examQuestions.AddRange(extraQuestions);

            foreach (var question in extraQuestions)
            {
                currentQuestionCount++;
                var questionButton = new QuestionButtonControl { QuestionNumber = currentQuestionCount };
                questionButton.Margin = new Thickness(5);
                questionButton.QuestionButton.Click += QuestionButton_Click;
                QuestionsWp.Children.Add(questionButton);
                questionButtons.Add(questionButton);
            }
        }

        private void StartExamTimer()
        {
            examTimer = new DispatcherTimer();
            examTimer.Interval = TimeSpan.FromSeconds(1);
            examTimer.Tick += ExamTimer_Tick;
            examTimer.Start();
        }

        private void ExamTimer_Tick(object sender, EventArgs e)
        {
            bool allQuestionsAnswered = selectedAnswers.Count == examQuestions.Count;

            if (allQuestionsAnswered || remainingTime <= TimeSpan.Zero)
            {
                examTimer.Stop();
                EndExam();
            }
            else
            {
                remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));
                TimerText.Text = remainingTime.ToString(@"mm\:ss");
            }
        }

        private void EndExam()
        {
            examTimer.Stop();

            int correctAnswers = selectedAnswers.Count(sa => sa.Value.IsRight == true);
            int totalQuestions = examQuestions.Count;

            MessageBox.Show($"Экзамен завершен. Правильных ответов: {correctAnswers} из {totalQuestions}. Просмотрите свои варианты ответов");

            // Деактивировать кнопки для вопросов, на которые пользователь не ответил
            foreach (var buttonControl in questionButtons)
            {
                var questionIndex = buttonControl.QuestionNumber - 1;
                if (!selectedAnswers.ContainsKey(questionIndex))
                {
                    buttonControl.QuestionButton.IsEnabled = false;
                }
            }
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            examTimer.Stop();
            NavigationService.Navigate(new StudyPage(contextUser));
        }
    }
}
