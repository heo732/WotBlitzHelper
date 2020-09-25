using System.Windows;

namespace MasteryMarks
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var a = new MasteryMarksLoader("t3mp0").GetMasteryMarks();
        }
    }
}