using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MarqueeTextBox
{
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_Content_2", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_Border", Type = typeof(Border))]
    public class MarqueeLabel : Label
    {
        #region Dependency Properties
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
            set { SetValue(LoopModeProperty, value); RecreateStoryboard(); }
        }
        #endregion

        #region ScrollSpeed
        public static readonly DependencyProperty ScrollSpeedProperty =
            DependencyProperty.Register("ScrollSpeed", typeof(double), typeof(MarqueeLabel),
                new PropertyMetadata(1.0));

        public double ScrollSpeed
        {
            get { return (double)GetValue(ScrollSpeedProperty); }
            set { SetValue(ScrollSpeedProperty, value); RecreateStoryboard(); }
        }
        #endregion

        #region StartPauseTime
        public static readonly DependencyProperty StartPauseTimeProperty =
            DependencyProperty.Register("StartPauseTime", typeof(TimeSpan), typeof(MarqueeLabel),
                new PropertyMetadata(TimeSpan.FromMilliseconds(500)));

        public TimeSpan StartPauseTime
        {
            get { return (TimeSpan)GetValue(StartPauseTimeProperty); }
            set { SetValue(StartPauseTimeProperty, value); RecreateStoryboard(); }
        }
        #endregion

        #region FadeTime
        public static readonly DependencyProperty FadeTimeProperty =
            DependencyProperty.Register("FadeTime", typeof(TimeSpan), typeof(MarqueeLabel),
                new PropertyMetadata(TimeSpan.FromMilliseconds(500)));

        public TimeSpan FadeTime
        {
            get { return (TimeSpan)GetValue(FadeTimeProperty); }
            set { SetValue(FadeTimeProperty, value); RecreateStoryboard(); }
        }
        #endregion

        #region EndPauseTime
        public static readonly DependencyProperty EndPauseTimeProperty =
            DependencyProperty.Register("EndPauseTime", typeof(TimeSpan), typeof(MarqueeLabel),
                new PropertyMetadata(TimeSpan.FromMilliseconds(500)));

        public TimeSpan EndPauseTime
        {
            get { return (TimeSpan)GetValue(EndPauseTimeProperty); }
            set { SetValue(EndPauseTimeProperty, value); RecreateStoryboard(); }
        }
        #endregion
        #endregion
        public Storyboard storyboard;
        private Border border;
        public ContentPresenter contentPresenter;
        public ContentPresenter contentPresenter2;
        private TextBlock textBlock;
        private TextBlock textBlock2;

        private double spacing = 30;
        private bool dontChange = false;

        private double textWidth;

        static MarqueeLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarqueeLabel), new FrameworkPropertyMetadata(typeof(MarqueeLabel)));
        }

        public MarqueeLabel()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount(contentPresenter) > 0 && VisualTreeHelper.GetChildrenCount(contentPresenter2) > 0)
            {
                textBlock = VisualTreeHelper.GetChild(contentPresenter, 0) as TextBlock;
                textBlock2 = VisualTreeHelper.GetChild(contentPresenter2, 0) as TextBlock;

                textBlock2.Visibility = Visibility.Hidden;
                textWidth = textBlock.ActualWidth;
                textBlock.SizeChanged += TextBlockSizeChanged;

                //textBlock.Background = new SolidColorBrush(Color.FromRgb(0,0,0));
                //textBlock2.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));

                //var temp1 = textBlock.ActualWidth;
                //var temp2 = textBlock2.ActualWidth;

                textBlock2.Margin = new Thickness(0) { Left = textWidth + spacing };
            }

            CreateStoryboard();
        }

        private void TextBlockSizeChanged(object sender, EventArgs e)
        {
            textWidth = textBlock.ActualWidth;
            if (!dontChange)
                RecreateStoryboard();
            else
                dontChange = false;
        }

        private void CreateStoryboard()
        {
            if (textWidth > ActualWidth)
            {
                switch (LoopMode)
                {
                    case Loop.Continuous:
                        storyboard = CreateContinuousMarqueeStoryboard();
                        break;
                    case Loop.FadeToBeginning:
                        storyboard = CreateFadeMarqueeStoryboard();
                        break;
                }
            }
            else
            {
                storyboard = null;
            }
        }
        
        private Storyboard CreateContinuousMarqueeStoryboard()
        {
            textBlock2.Visibility = Visibility.Visible;

            dontChange = true;

            textBlock.Width = textBlock.ActualWidth + spacing;
            textBlock2.Width = textBlock2.ActualWidth + spacing;

            ThicknessAnimationUsingKeyFrames movementAnimation1 = new ThicknessAnimationUsingKeyFrames();
            ThicknessAnimationUsingKeyFrames movementAnimation2 = new ThicknessAnimationUsingKeyFrames();
            double time = 0;

            movementAnimation1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0),
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));
            movementAnimation2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0) { Left = textWidth + spacing },
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));

            time += (textWidth + spacing) / (0.05 * ScrollSpeed);

            movementAnimation1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0) { Left = -textWidth - Padding.Left - Padding.Right - spacing},
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));
            movementAnimation2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0),
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));
            movementAnimation1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0) { Left = textWidth + spacing},
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time + 1))));

            time += (textWidth + spacing) / (0.05 * ScrollSpeed);

            movementAnimation2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0) { Left = -textWidth - Padding.Left  - Padding.Right - spacing},
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));
            movementAnimation1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0),
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));
            movementAnimation2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0) { Left = textWidth + spacing },
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time + 1))));

            Storyboard storyboard = new Storyboard()
            {
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTarget(movementAnimation1, textBlock);
            Storyboard.SetTarget(movementAnimation2, textBlock2); 
            Storyboard.SetTargetProperty(movementAnimation1, new PropertyPath(MarginProperty));
            Storyboard.SetTargetProperty(movementAnimation2, new PropertyPath(MarginProperty));

            storyboard.Children.Add(movementAnimation1);
            storyboard.Children.Add(movementAnimation2);

            return storyboard;
        }

        private Storyboard CreateFadeMarqueeStoryboard()
        {
            ThicknessAnimationUsingKeyFrames movementAnimation = new ThicknessAnimationUsingKeyFrames();
            DoubleAnimationUsingKeyFrames opacityAnimation = new DoubleAnimationUsingKeyFrames();
            double time = StartPauseTime.TotalMilliseconds;

            movementAnimation.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0),
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
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

        private void RecreateStoryboard()
        {
            if (storyboard != null)
                storyboard.Stop();

            //textBlock2.Visibility = Visibility.Hidden;
            //textBlock.Margin = new Thickness(0);
            //textBlock2.Margin = new Thickness(0) { Left = textWidth + spacing };
            //storyboard = null;
            CreateStoryboard();

            if (storyboard != null)
                storyboard.Begin();
        }

        public void PlayAnimation()
        {
            if(storyboard != null)
                storyboard.Begin();
        }

        public void StopAnimation()
        {
            if (storyboard != null)
                storyboard.Stop();
        }

        public void PauseAnimation()
        {
            if (storyboard != null)
                storyboard.Pause();
        }

        public void ResumeAnimation()
        {
            if (storyboard != null)
                storyboard.Resume();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            contentPresenter = GetTemplateChild("PART_Content") as ContentPresenter;
            contentPresenter2 = GetTemplateChild("PART_Content_2") as ContentPresenter;
            border = GetTemplateChild("PART_Border") as Border;
        }
    }
}
