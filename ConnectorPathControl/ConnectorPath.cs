using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ConnectorPathControl
{
    [TemplatePart(Name = "PART_Path", Type = typeof(Path))]
    public class ConnectorPath : Control
    {
        private Path path;

        #region Dependency Properties
        #region UIElement1
        public static readonly DependencyProperty UIElement1Property =
            DependencyProperty.Register("UIElement1", typeof(FrameworkElement), typeof(ConnectorPath),
                new PropertyMetadata(null));

        public FrameworkElement UIElement1
        {
            get { return (FrameworkElement)GetValue(UIElement1Property); }
            set { SetValue(UIElement1Property, value); }
        }
        #endregion

        #region UIElement2
        public static readonly DependencyProperty UIElement2Property =
            DependencyProperty.Register("UIElement2", typeof(FrameworkElement), typeof(ConnectorPath),
                new PropertyMetadata(null));

        public FrameworkElement UIElement2
        {
            get { return (FrameworkElement)GetValue(UIElement2Property); }
            set { SetValue(UIElement2Property, value); }
        }
        #endregion
        #endregion

        static ConnectorPath()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConnectorPath), new FrameworkPropertyMetadata(typeof(ConnectorPath)));
        }

        public ConnectorPath()
        {
            Loaded += OnLoaded;
        }

        public ConnectorPath(FrameworkElement element1, FrameworkElement element2)
        {
            UIElement1 = element1;
            UIElement2 = element2;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            PresentationSource ps = PresentationSource.FromVisual(sender as Visual);
            ps.ContentRendered += OnContentRendered;
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
            ((PresentationSource)sender).ContentRendered -= OnContentRendered;
            CreateConnectorPath();
        }

        private enum RelativePosition
        {
            Center, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left, TopLeft
        }

        private void CreateConnectorPath()
        {
            if(UIElement1 != null && UIElement2 != null)
            {
                int lowestZIndex = Math.Min(Panel.GetZIndex(UIElement1), Panel.GetZIndex(UIElement2));

                Panel.SetZIndex(this, lowestZIndex - 1);

                Rect rect1 = new Rect(UIElement1.TransformToAncestor(Window.GetWindow(UIElement1)).Transform(new Point(0, 0)),
                    new Vector(UIElement1.ActualWidth, UIElement1.ActualHeight));
                Point center1 = new Point(rect1.Left + rect1.Width / 2, rect1.Top + rect1.Height / 2);

                Rect rect2 = new Rect(UIElement2.TransformToAncestor(Window.GetWindow(UIElement2)).Transform(new Point(0, 0)),
                    new Vector(UIElement2.ActualWidth, UIElement2.ActualHeight));
                Point center2 = new Point(rect2.Left + rect2.Width / 2, rect2.Top + rect2.Height / 2);

                Vector v = new Vector(center2.X - center1.X, center2.Y - center1.Y);

                string name1 = UIElement1.Name;
                string name2 = UIElement2.Name;

                RelativePosition position = GetRelativePosition(v);

                PathFigure pathFigure = new PathFigure();

                switch(position)
                {
                    case RelativePosition.Top:
                        pathFigure.StartPoint = new Point( rect1.Left + rect1.Width/2, rect1.Bottom);
                        pathFigure.Segments = CreateTopSegments(rect1, rect2, v);
                        break;

                    case RelativePosition.TopRight:
                        if (Math.Abs(v.X) > Math.Abs(v.Y))
                        {
                            pathFigure.StartPoint = new Point( rect1.Left, rect1.Top + rect1.Height / 2);
                            pathFigure.Segments = CreateTopRightSegments(rect1, rect2, v, Orientation.Horizontal);
                        }
                        else
                        {
                            pathFigure.StartPoint = new Point( rect1.Left + rect1.Width / 2, rect1.Bottom);
                            pathFigure.Segments = CreateTopRightSegments(rect1, rect2, v, Orientation.Vertical);
                        }

                        break;

                    case RelativePosition.Right:
                        pathFigure.StartPoint = new Point(rect1.Left, rect1.Top + rect1.Height / 2);
                        pathFigure.Segments = CreateRightSegments(rect1, rect2, v);
                        break;

                    case RelativePosition.BottomRight:
                        if (Math.Abs(v.X) > Math.Abs(v.Y))
                        {
                            pathFigure.StartPoint = new Point(rect1.Left, rect1.Top + rect1.Height / 2);
                            pathFigure.Segments = CreateBottomRightSegments(rect1, rect2, v, Orientation.Horizontal);
                        }
                        else
                        {
                            pathFigure.StartPoint = new Point(rect1.Left + rect1.Width / 2, rect1.Top);
                            pathFigure.Segments = CreateBottomRightSegments(rect1, rect2, v, Orientation.Vertical);
                        }
                        
                        break;

                    case RelativePosition.Bottom:
                        pathFigure.StartPoint = new Point(rect1.Left + rect1.Width / 2, rect1.Top);
                        pathFigure.Segments = CreateBottomSegments(rect1, rect2, v);
                        break;

                    case RelativePosition.BottomLeft:
                        if (Math.Abs(v.X) > Math.Abs(v.Y))
                        {
                            pathFigure.StartPoint = new Point(rect1.Right, rect1.Top + rect1.Height / 2);
                            pathFigure.Segments = CreateBottomLeftSegments(rect1, rect2, v, Orientation.Horizontal);
                        }
                        else
                        {
                            pathFigure.StartPoint = new Point(rect1.Left + rect1.Width / 2, rect1.Top);
                            pathFigure.Segments = CreateBottomLeftSegments(rect1, rect2, v, Orientation.Vertical);
                        }

                        break;

                    case RelativePosition.Left:
                        pathFigure.StartPoint = new Point(rect1.Right, rect1.Top + rect1.Height / 2);
                        pathFigure.Segments = CreateLeftSegments(rect1, rect2, v);
                        break;

                    case RelativePosition.TopLeft:
                        if (Math.Abs(v.X) > Math.Abs(v.Y))
                        {
                            pathFigure.StartPoint = new Point(rect1.Right, rect1.Top + rect1.Height / 2);
                            pathFigure.Segments = CreateTopLeftSegments(rect1, rect2, v, Orientation.Horizontal);
                        }
                        else
                        {
                            pathFigure.StartPoint = new Point(rect1.Left + rect1.Width / 2, rect1.Bottom);
                            pathFigure.Segments = CreateTopLeftSegments(rect1, rect2, v, Orientation.Vertical);
                        }

                        break;
                }

                PathFigureCollection myFigureCollection = new PathFigureCollection { pathFigure };
                PathGeometry pathGeometry = new PathGeometry { Figures = myFigureCollection };

                path.Data = pathGeometry; 
            }
        }

        private RelativePosition GetRelativePosition(Vector v)
        {
            if (v.X == 0 && v.Y == 0)
                return RelativePosition.Center;

            if (v.X == 0)
                return v.Y > 0 ? RelativePosition.Top : RelativePosition.Bottom;

            if (v.Y == 0)
                return v.X > 0 ? RelativePosition.Left : RelativePosition.Right;

            if (v.X > 0)
                return v.Y > 0 ? RelativePosition.TopLeft : RelativePosition.BottomLeft;
            else
                return v.Y > 0 ? RelativePosition.TopRight : RelativePosition.BottomRight;
        }

        private PathSegmentCollection CreateTopSegments(Rect rect1, Rect rect2, Vector dist)
        {
            PathSegmentCollection pathSegments = new PathSegmentCollection();

            LineSegment ls = new LineSegment()
            {
                Point = new Point(rect2.Left + rect2.Width / 2, rect2.Top)
            };
            pathSegments.Add(ls);

            return pathSegments;
        }

        private PathSegmentCollection CreateTopRightSegments(Rect rect1, Rect rect2, Vector dist, Orientation orientation)
        {
            PathSegmentCollection pathSegments = new PathSegmentCollection();

            LineSegment ls1 = new LineSegment();
            LineSegment ls2 = new LineSegment();
            LineSegment ls3 = new LineSegment();

            switch(orientation)
            {
                case Orientation.Horizontal:
                    ls1.Point = new Point(rect1.Left + dist.X / 8, rect1.Top + rect1.Height / 2);
                    ls2.Point = new Point(rect2.Right - dist.X / 8, rect2.Top + rect2.Height / 2);
                    ls3.Point = new Point(rect2.Right, rect2.Top + rect2.Height / 2);
                    break;
                case Orientation.Vertical:
                    ls1.Point = new Point( rect1.Left + rect1.Width / 2, rect1.Bottom + dist.Y / 8 );
                    ls2.Point = new Point( rect2.Left + rect2.Width / 2, rect2.Top - dist.Y / 8);
                    ls3.Point = new Point( rect2.Left + rect2.Width / 2, rect2.Top);
                    break;
            }

            pathSegments.Add(ls1);
            pathSegments.Add(ls2);
            pathSegments.Add(ls3);

            return pathSegments;
        }

        private PathSegmentCollection CreateRightSegments(Rect rect1, Rect rect2, Vector dist)
        {
            PathSegmentCollection pathSegments = new PathSegmentCollection();

            LineSegment ls = new LineSegment()
            {
                Point = new Point(rect2.Right, rect2.Top + rect2.Height / 2)
            };
            pathSegments.Add(ls);

            return pathSegments;
        }

        private PathSegmentCollection CreateBottomRightSegments(Rect rect1, Rect rect2, Vector dist, Orientation orientation)
        {
            PathSegmentCollection pathSegments = new PathSegmentCollection();

            LineSegment ls1 = new LineSegment();
            LineSegment ls2 = new LineSegment();
            LineSegment ls3 = new LineSegment();

            switch (orientation)
            {
                case Orientation.Horizontal:
                    ls1.Point = new Point(rect1.Left + dist.X / 8, rect1.Top + rect1.Height / 2);
                    ls2.Point = new Point(rect2.Right - dist.X / 8, rect2.Top + rect2.Height / 2);
                    ls3.Point = new Point(rect2.Right, rect2.Top + rect2.Height / 2);
                    break;
                case Orientation.Vertical:
                    ls1.Point = new Point(rect1.Left + rect1.Width / 2, rect1.Top + dist.Y / 8);
                    ls2.Point = new Point(rect2.Left + rect2.Width / 2, rect2.Bottom - dist.Y / 8);
                    ls3.Point = new Point(rect2.Left + rect2.Width / 2, rect2.Bottom);
                    break;
            }

            pathSegments.Add(ls1);
            pathSegments.Add(ls2);
            pathSegments.Add(ls3);

            return pathSegments;
        }

        private PathSegmentCollection CreateBottomSegments(Rect rect1, Rect rect2, Vector dist)
        {
            PathSegmentCollection pathSegments = new PathSegmentCollection();

            LineSegment ls = new LineSegment()
            {
                Point = new Point(rect2.Left + rect2.Width / 2, rect2.Bottom)
            };
            pathSegments.Add(ls);

            return pathSegments;
        }

        private PathSegmentCollection CreateBottomLeftSegments(Rect rect1, Rect rect2, Vector dist, Orientation orientation)
        {
            PathSegmentCollection pathSegments = new PathSegmentCollection();

            LineSegment ls1 = new LineSegment();
            LineSegment ls2 = new LineSegment();
            LineSegment ls3 = new LineSegment();

            switch (orientation)
            {
                case Orientation.Horizontal:
                    ls1.Point = new Point(rect1.Right + dist.X / 8, rect1.Top + rect1.Height / 2);
                    ls2.Point = new Point(rect2.Left - dist.X / 8, rect2.Top + rect2.Height / 2);
                    ls3.Point = new Point(rect2.Left, rect2.Top + rect2.Height / 2);
                    break;
                case Orientation.Vertical:
                    ls1.Point = new Point(rect1.Left + rect1.Width / 2, rect1.Top + dist.Y / 8);
                    ls2.Point = new Point(rect2.Left + rect2.Width / 2, rect2.Bottom - dist.Y / 8);
                    ls3.Point = new Point(rect2.Left + rect2.Width / 2, rect2.Bottom);
                    break;
            }

            pathSegments.Add(ls1);
            pathSegments.Add(ls2);
            pathSegments.Add(ls3);

            return pathSegments;
        }

        private PathSegmentCollection CreateLeftSegments(Rect rect1, Rect rect2, Vector dist)
        {
            PathSegmentCollection pathSegments = new PathSegmentCollection();

            LineSegment ls1 = new LineSegment()
            {
                Point = new Point(rect2.Left, rect2.Top + rect2.Height / 2)
            };
            pathSegments.Add(ls1);

            return pathSegments;
        }

        private PathSegmentCollection CreateTopLeftSegments(Rect rect1, Rect rect2, Vector dist, Orientation orientation)
        {
            PathSegmentCollection pathSegments = new PathSegmentCollection();

            LineSegment ls1 = new LineSegment();
            LineSegment ls2 = new LineSegment();
            LineSegment ls3 = new LineSegment();

            switch (orientation)
            {
                case Orientation.Horizontal:
                    ls1.Point = new Point(rect1.Right + dist.X / 8, rect1.Top + rect1.Height / 2);
                    ls2.Point = new Point(rect2.Left - dist.X / 8, rect2.Top + rect2.Height / 2);
                    ls3.Point = new Point(rect2.Left, rect2.Top + rect2.Height / 2);
                    break;
                case Orientation.Vertical:
                    ls1.Point = new Point(rect1.Left + rect1.Width / 2, rect1.Bottom + dist.Y / 8);
                    ls2.Point = new Point(rect2.Left + rect2.Width / 2, rect2.Top - dist.Y / 8);
                    ls3.Point = new Point(rect2.Left + rect2.Width / 2, rect2.Top);
                    break;
            }

            pathSegments.Add(ls1);
            pathSegments.Add(ls2);
            pathSegments.Add(ls3);

            return pathSegments;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            path = GetTemplateChild("PART_Path") as Path;
        }
    }
}
