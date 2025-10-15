using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KlarfViewer.Behaviors
{
    // Interact with View
    public class ImageInteractionBehavior : Behavior<Grid>
    {
        private Image image;
        private Canvas canvas;
        private Line measurementLine;
        private Point? panStartPoint;
        private Point imageOrigin;
        private Point? measurementStartPoint;

        public static readonly DependencyProperty IsInMeasurementModeProperty =
            DependencyProperty.Register(nameof(IsInMeasurementMode), typeof(bool), typeof(ImageInteractionBehavior), new PropertyMetadata(false));

        public bool IsInMeasurementMode
        {
            get { return (bool)GetValue(IsInMeasurementModeProperty); }
            set { SetValue(IsInMeasurementModeProperty, value); }
        }

        public static readonly DependencyProperty DistanceProperty =
            DependencyProperty.Register(nameof(Distance), typeof(double), typeof(ImageInteractionBehavior), new PropertyMetadata(0.0));

        public double Distance
        {
            get { return (double)GetValue(DistanceProperty); }
            set { SetValue(DistanceProperty, value); }
        }

        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register(nameof(ZoomLevel), typeof(double), typeof(ImageInteractionBehavior), 
                new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnZoomLevelChanged));

        public double ZoomLevel
        {
            get { return (double)GetValue(ZoomLevelProperty); }
            set { SetValue(ZoomLevelProperty, value); }
        }

        private static void OnZoomLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (ImageInteractionBehavior)d;
            behavior.UpdateZoom();
        }

        private void UpdateZoom()
        {
            if (image == null) return;
            var transformGroup = image.RenderTransform as TransformGroup;
            if (transformGroup == null) return;
            var scaleTransform = (ScaleTransform)transformGroup.Children[0];
            var translateTransform = (TranslateTransform)transformGroup.Children[1];

            double newScale = ZoomLevel / 100.0;
            scaleTransform.ScaleX = newScale;
            scaleTransform.ScaleY = newScale;

            ConstrainTranslation(translateTransform.X, translateTransform.Y);
        }

        private void ConstrainTranslation(double newX, double newY)
        {
            if (image == null || image.Source == null || AssociatedObject.ActualWidth == 0) return;

            var transformGroup = image.RenderTransform as TransformGroup;
            var scaleTransform = (ScaleTransform)transformGroup.Children[0];
            var translateTransform = (TranslateTransform)transformGroup.Children[1];

            double scale = scaleTransform.ScaleX;
            double viewerWidth = AssociatedObject.ActualWidth;
            double viewerHeight = AssociatedObject.ActualHeight;

            double scaledImageWidth = image.ActualWidth * scale;
            double scaledImageHeight = image.ActualHeight * scale;

            double boundX = Math.Max(0, (scaledImageWidth - viewerWidth) / 2);
            double boundY = Math.Max(0, (scaledImageHeight - viewerHeight) / 2);

            translateTransform.X = Math.Max(-boundX, Math.Min(newX, boundX));
            translateTransform.Y = Math.Max(-boundY, Math.Min(newY, boundY));
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            image = AssociatedObject.Children[0] as Image;
            canvas = AssociatedObject.Children[1] as Canvas;
            if (image == null || canvas == null)
            {
                // Fallback if the structure is not as expected
                image = AssociatedObject.FindName("PART_Image") as Image;
                canvas = AssociatedObject.FindName("PART_Canvas") as Canvas;
                if (image == null || canvas == null) return;
            }

            image.RenderTransformOrigin = new Point(0.5, 0.5);
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform());
            transformGroup.Children.Add(new TranslateTransform());
            image.RenderTransform = transformGroup;

            AssociatedObject.MouseWheel += OnMouseWheel;
            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseRightButtonDown += OnMouseRightButtonDown;
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Reset transform
            if (image != null)
            {
                var transformGroup = image.RenderTransform as TransformGroup;
                var st = transformGroup.Children[0] as ScaleTransform;
                var tt = transformGroup.Children[1] as TranslateTransform;
                ZoomLevel = 100.0;
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }

        // 이미지 확대 축소 
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (image == null) return;

            var transformGroup = image.RenderTransform as TransformGroup;
            var scaleTransform = (ScaleTransform)transformGroup.Children[0];
            var translateTransform = (TranslateTransform)transformGroup.Children[1];

            double zoomFactor = e.Delta > 0 ? 1.1 : 1 / 1.1;
            Point mousePos = e.GetPosition(AssociatedObject);

            double currentScale = scaleTransform.ScaleX;
            double newScale = currentScale * zoomFactor;
            ZoomLevel = newScale * 100.0;

            double newX = mousePos.X - (mousePos.X - translateTransform.X) * zoomFactor;
            double newY = mousePos.Y - (mousePos.Y - translateTransform.Y) * zoomFactor;
            // cusor 업데이트
            ConstrainTranslation(newX, newY);
        }

        // 이미지 Shifting
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (image == null) return;
            AssociatedObject.CaptureMouse();

            if (IsInMeasurementMode || Keyboard.Modifiers == ModifierKeys.Shift)
            {
                measurementStartPoint = e.GetPosition(canvas);
                if (measurementLine == null)
                {
                    measurementLine = new Line
                    {
                        Stroke = Brushes.Red,
                        StrokeThickness = 2
                    };
                    canvas.Children.Add(measurementLine);
                }
                measurementLine.X1 = measurementStartPoint.Value.X;
                measurementLine.Y1 = measurementStartPoint.Value.Y;
                measurementLine.X2 = measurementStartPoint.Value.X;
                measurementLine.Y2 = measurementStartPoint.Value.Y;
                measurementLine.Visibility = Visibility.Visible;
            }
            else
            {
                panStartPoint = e.GetPosition(AssociatedObject);
                var translateTransform = (TranslateTransform)((TransformGroup)image.RenderTransform).Children[1];
                imageOrigin = new Point(translateTransform.X, translateTransform.Y);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            // 측정 모드이거나 shift 를 누른 상태일때 
            if (measurementStartPoint.HasValue && (IsInMeasurementMode || Keyboard.Modifiers == ModifierKeys.Shift))
            {
                var currentPos = e.GetPosition(canvas);
                measurementLine.X2 = currentPos.X;
                measurementLine.Y2 = currentPos.Y;

                var transformGroup = image.RenderTransform as TransformGroup;
                var scaleTransform = (ScaleTransform)transformGroup.Children[0];

                Distance = Math.Round(Math.Sqrt(Math.Pow(measurementLine.X2 - measurementLine.X1, 2) + Math.Pow(measurementLine.Y2 - measurementLine.Y1, 2)) / scaleTransform.ScaleX, 2);
            }
            else if (panStartPoint.HasValue)
            {
                var currentPos = e.GetPosition(AssociatedObject);
                var delta = new Vector(currentPos.X - panStartPoint.Value.X, currentPos.Y - panStartPoint.Value.Y);
                var translateTransform = (TranslateTransform)((TransformGroup)image.RenderTransform).Children[1];
                double newX = imageOrigin.X + delta.X;
                double newY = imageOrigin.Y + delta.Y;
                ConstrainTranslation(newX, newY);
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.ReleaseMouseCapture();
            panStartPoint = null;

            if (measurementStartPoint.HasValue)
            {
                // To keep the line and distance, we don't reset them here.
                // Resetting will be handled by the view model if needed.
                measurementStartPoint = null;
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.Loaded -= AssociatedObject_Loaded;
                AssociatedObject.MouseWheel -= OnMouseWheel;
                AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
                AssociatedObject.MouseMove -= OnMouseMove;
                AssociatedObject.MouseRightButtonDown -= OnMouseRightButtonDown;
            }
            base.OnDetaching();
        }
    }
}
