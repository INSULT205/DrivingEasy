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

namespace DrivingEasy.Components
{
    /// <summary>
    /// Логика взаимодействия для QuestionButtonControl.xaml
    /// </summary>
    public partial class QuestionButtonControl : UserControl
    {
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
