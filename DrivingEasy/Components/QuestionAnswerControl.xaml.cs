using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DrivingEasy.Components
{
    public partial class QuestionAnswerControl : UserControl
    {
        public QuestionAnswerControl()
        {
            InitializeComponent();
        }
        public void SetAnswer(AnswerQuestion answer)
        {
            var radioButton = new RadioButton
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0, 10, 0),
                Tag = answer
            };
            radioButton.Checked += Answer_Checked;

            var textBlock = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                Text = answer.Answers.Decription,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 5, 0)
            };

            AnswerStack.Children.Add(radioButton);
            AnswerStack.Children.Add(textBlock);
        }

        private void Answer_Checked(object sender, RoutedEventArgs e)
        {
            var selectedAnswer = (sender as RadioButton)?.Tag as AnswerQuestion;
            bool isCorrect = selectedAnswer?.IsRight ?? false;

            AnswerStack.Background = isCorrect ? Brushes.Green : Brushes.Red;
            foreach (var child in AnswerStack.Children)
            {
                if (child is TextBlock textBlock)
                {
                    textBlock.Foreground = Brushes.White;
                }
            }
        }
    }
}
