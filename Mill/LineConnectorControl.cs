using System.Windows;
using System.Windows.Controls;

namespace Mill
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Mill"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Mill;assembly=Mill"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:LineConnectorControl/>
    ///
    /// </summary>


    /// <summary>
    /// Custom Line control to draw a line between two other controls
    /// </summary>
    public class LineConnectorControl : Control
    {
        static LineConnectorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineConnectorControl), new FrameworkPropertyMetadata(typeof(LineConnectorControl)));
        }

        #region Target Properties for Visual Line

        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        // Using a DependencyProperty as the backing store for X1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(LineConnectorControl), new PropertyMetadata(0d));



        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        // Using a DependencyProperty as the backing store for X2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(LineConnectorControl), new PropertyMetadata(0d));



        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        // Using a DependencyProperty as the backing store for Y1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(LineConnectorControl), new PropertyMetadata(0d));


        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        // Using a DependencyProperty as the backing store for Y2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(LineConnectorControl), new PropertyMetadata(0d));

        #endregion

        #region Source Elements needed to compute Visual Line

        // Positions are computed relative to this element
        public FrameworkElement PositionRoot
        {
            get { return (FrameworkElement)GetValue(PositionRootProperty); }
            set { SetValue(PositionRootProperty, value); }
        }

        // This is the starting point of the line
        public FrameworkElement ConnectedControl1
        {
            get { return (FrameworkElement)GetValue(ConnectedControl1Property); }
            set { SetValue(ConnectedControl1Property, value); }
        }

        // This is the ending point of the line
        public FrameworkElement ConnectedControl2
        {
            get { return (FrameworkElement)GetValue(ConnectedControl2Property); }
            set { SetValue(ConnectedControl2Property, value); }
        }

        // Using a DependencyProperty as the backing store for PositionRoot.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionRootProperty =
            DependencyProperty.Register("PositionRoot", typeof(FrameworkElement), typeof(LineConnectorControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(ElementChanged)));

        // Using a DependencyProperty as the backing store for ConnectedControl1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectedControl1Property =
            DependencyProperty.Register("ConnectedControl1", typeof(FrameworkElement), typeof(LineConnectorControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(ElementChanged)));

        // Using a DependencyProperty as the backing store for ConnectedControl2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectedControl2Property =
            DependencyProperty.Register("ConnectedControl2", typeof(FrameworkElement), typeof(LineConnectorControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(ElementChanged)));

        #endregion

        #region Update logic to compute line coordinates based on Source Elements

        private static void ElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (LineConnectorControl)d;
            self.UpdatePositions();
            var fr = (FrameworkElement)e.NewValue;
            fr.SizeChanged += self.ElementSizeChanged;
        }

        private void ElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePositions();
        }

        private void UpdatePositions()
        {
            if (PositionRoot != null && ConnectedControl1 != null && ConnectedControl2 != null)
            {
                Rect rect1 = GetRootedRect(ConnectedControl1);
                Rect rect2 = GetRootedRect(ConnectedControl2);

                X1 = rect1.Location.X + rect1.Width / 2;
                Y1 = rect1.Location.Y + rect1.Height / 2;
                X2 = rect2.Location.X + rect2.Width / 2;
                Y2 = rect2.Location.Y + rect2.Height / 2;
            }
        }

        private Rect GetRootedRect(FrameworkElement childControl)
        {
            var rootRelativePosition = childControl.TransformToAncestor(PositionRoot).Transform(new Point(0, 0));
            return new Rect(rootRelativePosition, new Size(childControl.ActualWidth, childControl.ActualHeight));
        }

        #endregion
    }
}

