using System;
using System.Collections.Generic;
using System.IO;
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

namespace DrivingEasy.Components
{
    /// <summary>
    /// Логика взаимодействия для QuestionDetailControl.xaml
    /// </summary>
    public partial class QuestionDetailControl : UserControl
    {
        private User contextUser;
        private Questions currentQuestion;
        private List<AnswerQuestion> answerOptions;
        private int questionNumber;

        public QuestionDetailControl(Questions question, List<AnswerQuestion> answers, User user, int questionNumber)
        {
            InitializeComponent();
            contextUser = user;
            currentQuestion = question;
            answerOptions = answers;
            this.questionNumber = questionNumber;

            LoadQuestionImage(question.Photo);
            QuestionDescription.Text = question.Description;
            AnswersList.ItemsSource = answerOptions;
            this.DataContext = this;
        }

        private void LoadQuestionImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                var defaultImagePath = "/Resources/noImage.jpg";
                QuestionImage.Source = new BitmapImage(new Uri(defaultImagePath, UriKind.Relative));
                return;
            }

            using (var ms = new MemoryStream(imageData))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                QuestionImage.Source = bitmap;
            }
        }

        private void Answer_Checked(object sender, RoutedEventArgs e)
        {
            var selectedAnswer = (sender as RadioButton).DataContext as AnswerQuestion;

            SaveUserAnswer(selectedAnswer.Id);

            bool isCorrect = selectedAnswer.IsRight ?? false;

            var radioButton = sender as RadioButton;
            if (isCorrect)
            {
                radioButton.Foreground = Brushes.Green;
                radioButton.Background = Brushes.Green;
            }
            else
            {
                radioButton.Foreground = Brushes.Red;
                radioButton.Background = Brushes.Red;

                ShowCorrectAnswer();
            }

            UpdateQuestionButtonColor(isCorrect);
        }

        private void ShowCorrectAnswer()
        {
            foreach (var item in AnswersList.Items)
            {
                var container = AnswersList.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                if (container != null)
                {
                    var radioButton = container.FindName("AnswerRadioButton") as RadioButton;

                    if (radioButton != null && (bool)radioButton.Tag == true)
                    {
                        radioButton.Foreground = Brushes.Green;
                        radioButton.Background = Brushes.Green;
                    }
                }
            }
        }
        private TicketQuestionsPage GetParentPage()
        {
            DependencyObject parent = this;
            while (parent != null && !(parent is TicketQuestionsPage))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as TicketQuestionsPage;
        }
        private void UpdateQuestionButtonColor(bool isCorrect)
        {
            var parentPage = GetParentPage();
            var questionButton = parentPage?.QuestionsWp.Children
                               .OfType<QuestionButtonControl>()
                               .FirstOrDefault(b => b.QuestionNumber == questionNumber);

            if (questionButton != null)
            {
                questionButton.QuestionButton.Background = isCorrect ? Brushes.Green : Brushes.Red;
                questionButton.QuestionButton.Foreground = Brushes.White;
            }
        }
        private void SaveUserAnswer(int answerQuestionId)
        {
            var existingAnswer = App.DB.UserAnswer
                                       .FirstOrDefault(ua => ua.AnswerQuestionId == answerQuestionId && ua.UserId == contextUser.Id);

            if (existingAnswer == null)
            {
                var userAnswer = new UserAnswer
                {
                    AnswerQuestionId = answerQuestionId,
                    UserId = contextUser.Id
                };

                App.DB.UserAnswer.Add(userAnswer);
            }
            else
            {
                existingAnswer.AnswerQuestionId = answerQuestionId;
            }

            App.DB.SaveChanges();
        }
    }
}