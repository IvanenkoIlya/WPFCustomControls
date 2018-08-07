using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MarqueeTextBox
{
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentPresenter))]
    public class MarqueeLabel : Label
    {
        #region Dependancy Properties
        #region LoopMode
        public enum Loop
        {
            FadeToBeginning,
            Continuous
        }

        public static readonly DependencyProperty LoopModeProperty =
            DependencyProperty.Register("LoopMode", typeof(Loop), typeof(MarqueeLabel),
                new FrameworkPropertyMetadata(Loop.FadeToBeginning));

        public Loop LoopMode
        {
            get { return (Loop) GetValue(LoopModeProperty); }
            set { SetValue(LoopModeProperty, value); }
        }
        #endregion

        #region ScrollSpeed
        public static readonly DependencyProperty ScrollSpeedProperty =
            DependencyProperty.Register("ScrollSpeed", typeof(double), typeof(MarqueeLabel),
                new PropertyMetadata(1.0));

        public double ScrollSpeed
        {
            get { return (double)GetValue(ScrollSpeedProperty); }
            set { SetValue(ScrollSpeedProperty, value); }
        }
        #endregion

        #region StartPauseTime
        public static readonly DependencyProperty StartPauseTimeProperty =
            DependencyProperty.Register("StartPauseTime", typeof(TimeSpan), typeof(MarqueeLabel),
                new PropertyMetadata(TimeSpan.FromMilliseconds(500)));

        public TimeSpan StartPauseTime
        {
            get { return (TimeSpan)GetValue(StartPauseTimeProperty); }
            set { SetValue(StartPauseTimeProperty, value); }
        }
        #endregion

        #region FadeTime
        public static readonly DependencyProperty FadeTimeProperty =
            DependencyProperty.Register("FadeTime", typeof(TimeSpan), typeof(MarqueeLabel),
                new PropertyMetadata(TimeSpan.FromMilliseconds(500)));

        public TimeSpan FadeTime
        {
            get { return (TimeSpan)GetValue(FadeTimeProperty); }
            set { SetValue(FadeTimeProperty, value); }
        }
        #endregion

        #region EndPauseTime
        public static readonly DependencyProperty EndPauseTimeProperty =
            DependencyProperty.Register("EndPauseTime", typeof(TimeSpan), typeof(MarqueeLabel),
                new PropertyMetadata(TimeSpan.FromMilliseconds(500)));

        public TimeSpan EndPauseTime
        {
            get { return (TimeSpan)GetValue(EndPauseTimeProperty); }
            set { SetValue(EndPauseTimeProperty, value); }
        }
        #endregion
        #endregion

        public Storyboard storyboard;
        public ContentPresenter contentPresenter;
        private TextBlock textBlock;

        private double textWidth;

        static MarqueeLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarqueeLabel), new FrameworkPropertyMetadata(typeof(MarqueeLabel)));
        }

        public MarqueeLabel()
        {
            double h = Height;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
            {
                textBlock = VisualTreeHelper.GetChild(contentPresenter, 0) as TextBlock;

                textWidth = textBlock.ActualWidth;
            }

            if(textWidth > ActualWidth)
            {
                storyboard = CreateMarqueeStoryboard();
            }
        }

        private Storyboard CreateMarqueeStoryboard()
        {
            ThicknessAnimationUsingKeyFrames movementAnimation = new ThicknessAnimationUsingKeyFrames();
            DoubleAnimationUsingKeyFrames opacityAnimation = new DoubleAnimationUsingKeyFrames();
            double time = StartPauseTime.TotalMilliseconds;

            movementAnimation.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0),
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));

            time += (textWidth - ActualWidth) / (0.05 * ScrollSpeed);

            movementAnimation.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0) { Left = -textWidth - Padding.Left * 2 - Padding.Right + ActualWidth },
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));

            time += EndPauseTime.TotalMilliseconds;

            movementAnimation.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0) { Left = -textWidth - Padding.Left * 2 - Padding.Right + ActualWidth },
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));
            opacityAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(1.0,
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));

            time += FadeTime.TotalMilliseconds;

            opacityAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.0,
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));
            movementAnimation.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0) { Left = -textWidth - Padding.Left * 2 - Padding.Right + ActualWidth },
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));

            time += 1;

            movementAnimation.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0),
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));

            time += FadeTime.TotalMilliseconds;

            opacityAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(1.0,
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));

            Storyboard storyboard = new Storyboard
            {
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTarget(movementAnimation, textBlock);
            Storyboard.SetTarget(opacityAnimation, textBlock);
            Storyboard.SetTargetProperty(movementAnimation, new PropertyPath(MarginProperty));
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));

            storyboard.Children.Add(movementAnimation);
            storyboard.Children.Add(opacityAnimation);

            return storyboard;
        }

        public void PlayAnimation()
        {
            storyboard.Begin();
        }

        public void StopAnimation()
        {
            storyboard.Stop();
        }

        public void PauseAnimation()
        {
            storyboard.Pause();
        }

        public void ResumeAnimation()
        {
            storyboard.Resume();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            contentPresenter = GetTemplateChild("PART_Content") as ContentPresenter;
        }
    }
}
