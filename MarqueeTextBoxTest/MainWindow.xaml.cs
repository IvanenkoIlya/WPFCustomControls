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

namespace MarqueeTextBoxTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnClick_Play(object sender, RoutedEventArgs e)
        {
            TestMarquee.PlayAnimation();
        }

        private void OnClick_Stop(object sender, RoutedEventArgs e)
        {
            TestMarquee.StopAnimation();
        }

        private void OnClick_PauseResume(object sender, RoutedEventArgs e)
        {
            if(TestMarquee.storyboard.GetIsPaused())
            {
                PauseResumeButton.Content = "Pause";
                TestMarquee.ResumeAnimation();
            }
            else
            {
                PauseResumeButton.Content = "Resume";
                TestMarquee.PauseAnimation();
            }
        }
    }
}
