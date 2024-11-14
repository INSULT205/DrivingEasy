using System.Windows.Controls;

namespace DrivingEasy.Components
{
    public partial class QuestionButtonControl : UserControl
    {
        public AnswerQuestion SelectedAnswer { get; set; }
        public QuestionButtonControl()
        {
            InitializeComponent();
        }

        public int QuestionNumber
        {
            get => int.Parse(QuestionButton.Content.ToString());
            set => QuestionButton.Content = value.ToString();
        }
    }
}
