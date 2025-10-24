using System;
using System.ComponentModel;
using KlarfViewer.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KlarfViewer.View
{
    /// <summary>
    /// Analysis.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HistogramWindow : Window
    {
        private HistogramViewModel? viewModel;
        private const double AxisMargin = 10; // Margin for axis labels
        public HistogramWindow()
        {
            InitializeComponent();
            Loaded += HistogramWindow_Loaded;

        }

        private void HistogramWindow_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = DataContext as HistogramViewModel;
            if (viewModel == null) return;

            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            DrawUI();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HistogramViewModel.MaxHistogramValue))
            {
                DrawUI();
            }
        }

        private void HistogramCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawUI();
        }

        private void DrawUI()
        {
            HistogramCanvas.Children.Clear();
            XAxisLabelsPanel.Children.Clear();
            YAxisLabelsPanel.Children.Clear();

            if (viewModel == null || viewModel.MaxHistogramValue == 0) return;

            DrawAxes();
            DrawHistogram();
            DrawXAxisLabels();
            DrawYAxisLabels();
        }

        private void DrawAxes()
        {
            double canvasWidth = HistogramCanvas.ActualWidth;
            double canvasHeight = HistogramCanvas.ActualHeight;

            // Y-Axis Line
            var yAxis = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = canvasHeight,
                Stroke = Brushes.WhiteSmoke,
                StrokeThickness = 1
            };
            HistogramCanvas.Children.Add(yAxis);

            // X-Axis Line
            var xAxis = new Line
            {
                X1 = 0,
                Y1 = canvasHeight,
                X2 = canvasWidth,
                Y2 = canvasHeight,
                Stroke = Brushes.WhiteSmoke,
                StrokeThickness = 1
            };
            HistogramCanvas.Children.Add(xAxis);
        }

        private void DrawHistogram()
        {
            if (viewModel == null) return;

            if (viewModel.IsColorImage)
            {
                DrawChannelHistogram(viewModel.R_HistogramData, Color.FromArgb(128, 255, 0, 0)); // Red
                DrawChannelHistogram(viewModel.G_HistogramData, Color.FromArgb(128, 0, 255, 0)); // Green
                DrawChannelHistogram(viewModel.B_HistogramData, Color.FromArgb(128, 0, 0, 255)); // Blue
            }
            else
            {
                DrawChannelHistogram(viewModel.GrayscaleHistogramData, Colors.WhiteSmoke);
            }
        }

        private void DrawChannelHistogram(int[]? data, Color color)
        {
            if (data == null || data.Length == 0 || viewModel == null) return;

            int max = viewModel.MaxHistogramValue;
            if (max == 0) return;

            double canvasWidth = HistogramCanvas.ActualWidth;
            double canvasHeight = HistogramCanvas.ActualHeight;
            double barWidth = canvasWidth / data.Length;

            var brush = new SolidColorBrush(color);
            brush.Freeze();

            for (int i = 0; i < data.Length; i++)
            {
                double barHeight = (double)data[i] / max * canvasHeight;
                if (barHeight <= 0) continue;

                var bar = new Rectangle
                {
                    Width = barWidth,
                    Height = barHeight,
                    Fill = brush
                };

                Canvas.SetLeft(bar, i * barWidth);
                Canvas.SetBottom(bar, 0);
                HistogramCanvas.Children.Add(bar);
            }
        }

        private void DrawXAxisLabels()
        {
            double canvasWidth = HistogramCanvas.ActualWidth;
            const int labelCount = 5; // 0, 64, 128, 192, 255

            for (int i = 0; i < labelCount; i++)
            {
                int value = (int)Math.Round((255.0 / (labelCount - 1)) * i);
                var label = new TextBlock
                {
                    Text = value.ToString(),
                    Foreground = Brushes.WhiteSmoke,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                // Create a container to allow centering the label
                var container = new Border { Width = canvasWidth / (labelCount - 1), Child = label };
                XAxisLabelsPanel.Children.Add(container);
            }
        }

        private void DrawYAxisLabels()
        {
            if (viewModel == null) return;

            double canvasHeight = YAxisLabelsPanel.ActualHeight;
            const int labelCount = 4; // Number of labels to show
            int max = viewModel.MaxHistogramValue;

            YAxisLabelsPanel.RowDefinitions.Clear();

            for (int i = 0; i < labelCount; i++)
            {
                YAxisLabelsPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                double valueFraction = (double)i / (labelCount - 1);
                int value = (int)(max * (1 - valueFraction)); // From top to bottom

                var label = new TextBlock
                {
                    Text = FormatYAxisLabel(value),
                    Foreground = Brushes.WhiteSmoke,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Grid.SetRow(label, i);
                YAxisLabelsPanel.Children.Add(label);
            }
        }

        private string FormatYAxisLabel(int value)
        {
            if (value >= 1000000) return $"{(double)value / 1000000:0.#}M";
            if (value >= 1000) return $"{(double)value / 1000:0.#}K";
            return value.ToString();
        }
    }
}
