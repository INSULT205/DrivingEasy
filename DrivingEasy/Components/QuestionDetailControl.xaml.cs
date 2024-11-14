using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DrivingEasy.Components
{
    public partial class QuestionDetailControl : UserControl
    {
        private User contextUser;
        private Questions currentQuestion;
        private List<AnswerQuestion> answerOptions;
        private int questionNumber;
        private bool answerSelected = false;

        public event EventHandler<(bool isCorrect, int questionNumber, AnswerQuestion selectedAnswer)> AnswerSelected;

        public QuestionDetailControl(Questions question, List<AnswerQuestion> answers, User user, int questionNumber)
        {
            InitializeComponent();
            contextUser = user;
            currentQuestion = question;
            answerOptions = answers;
            this.questionNumber = questionNumber;

            LoadQuestionImage(question.Photo);
            QuestionDescription.Text = question.Description;
            LoadAnswers();
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

        private void LoadAnswers()
        {
            foreach (var answer in answerOptions)
            {
                var answerControl = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(5)
                };

                var radioButton = new RadioButton
                {
                    Tag = answer,
                    VerticalAlignment = VerticalAlignment.Center
                };
                radioButton.Checked += Answer_Checked;

                var answerText = new TextBlock
                {
                    Text = answer.Answers.Decription,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(5),
                    Foreground = Brushes.Black
                };

                answerText.MouseLeftButtonUp += (s, e) =>
                {
                    if (!answerSelected)
                    {
                        radioButton.RaiseEvent(new RoutedEventArgs(RadioButton.CheckedEvent));
                    }
                };

                answerControl.Children.Add(radioButton);
                answerControl.Children.Add(answerText);
                AnswerStack.Children.Add(answerControl);
            }
        }

        public void SetSelectedAnswer(AnswerQuestion selectedAnswer)
        {
            foreach (StackPanel answerControl in AnswerStack.Children)
            {
                var radioButton = answerControl.Children[0] as RadioButton;
                var answerText = answerControl.Children[1] as TextBlock;

                if (radioButton?.Tag is AnswerQuestion answer)
                {
                    if (answer == selectedAnswer)
                    {
                        radioButton.IsChecked = true;
                        answerControl.Background = answer.IsRight.GetValueOrDefault() ? Brushes.Green : Brushes.Red;
                        answerText.Foreground = Brushes.White;
                    }
                }
            }
            answerSelected = true;
        }

        private void Answer_Checked(object sender, RoutedEventArgs e)
        {
            if (answerSelected) return;

            var selectedAnswer = (sender as RadioButton)?.Tag as AnswerQuestion;

            bool isCorrect = selectedAnswer?.IsRight ?? false;

            foreach (StackPanel answerControl in AnswerStack.Children)
            {
                var radioButton = answerControl.Children[0] as RadioButton;
                var answerText = answerControl.Children[1] as TextBlock;

                if (radioButton?.Tag is AnswerQuestion answer)
                {
                    if (answer.IsRight == true)
                    {
                        answerControl.Background = Brushes.Green;
                        answerText.Foreground = Brushes.White;
                    }
                    else if (selectedAnswer == answer)
                    {
                        answerControl.Background = Brushes.Red;
                        answerText.Foreground = Brushes.White;
                    }
                    else
                    {
                        answerControl.Background = Brushes.Transparent;
                        answerText.Foreground = Brushes.Black;
                    }

                    radioButton.IsEnabled = false;
                }
            }

            answerSelected = true;
            AnswerSelected?.Invoke(this, (isCorrect, questionNumber, selectedAnswer));
        }
    }
}
