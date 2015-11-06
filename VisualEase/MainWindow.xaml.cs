using f14.VisualEase.Core;
using f14.VisualEase.Core.Easing;
using f14.VisualEase.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VisualEase
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double _startValue = 0.0;
        private double _duration = 0.0;
        private double _endValue = 0.0;
        private Polyline _curve;
        private Dictionary<double, double> _curveRealData;
        private MethodInfo _easeFunc;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;

            FunctionTypeBox.ItemsSource = Enum.GetValues(typeof(EaseFuncType)).Cast<EaseFuncType>();
            FunctionModeBox.ItemsSource = Enum.GetValues(typeof(EasingMode)).Cast<EasingMode>();

            FunctionTypeBox.SelectedItem = EaseFuncType.Linear;
            FunctionModeBox.SelectedItem = EasingMode.EaseOut;
        }

        private void SetSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_curve != null)
                FunctionCurve.Children.Remove(_curve);

            try
            {
                _duration = Double.Parse(TimeBox.Text);
                _endValue = Double.Parse(ValueBox.Text);

                EaseFuncType easeType = (EaseFuncType)FunctionTypeBox.SelectedItem;
                EasingMode easeMode = (EasingMode)FunctionModeBox.SelectedItem;

                _easeFunc = EaseFuncMath.GetFunction(easeType, easeMode);

                _curve = new Polyline();
                _curve.Points = new PointCollection();
                _curve.Stroke = Brushes.Blue;
                _curve.StrokeThickness = 2.0;

                double stepValue = _duration * 0.01;
                _curveRealData = new Dictionary<double, double>();

                for (double i = 0; i <= _duration; MathHelper.Clamp(i += stepValue, 0, _duration))
                {
                    object[] parameters = new object[4] { i, _startValue, _endValue, _duration };
                    double resultValue = (double)_easeFunc.Invoke(null, parameters);
                    _curveRealData.Add(i, resultValue);
                }

                double canvasHeight = FunctionCurve.ActualHeight;
                double canvasWidth = FunctionCurve.ActualWidth;
                double resultMax = _curveRealData.Values.Max();

                foreach (var kv in _curveRealData)
                {
                    double passedTimePrc = kv.Key / _duration;
                    double passedValuePrc = kv.Value / resultMax;

                    Point curvePoint = new Point(canvasWidth * passedTimePrc, canvasHeight * passedValuePrc);
                    _curve.Points.Add(curvePoint);
                }

                FunctionCurve.Children.Insert(FunctionCurve.Children.Count - 1, _curve);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Curve_MouseMove(object sender, MouseEventArgs e)
        {
            if (_curve == null)
                return;

            Point position = e.GetPosition(FunctionCurve);       
            
            Canvas.SetLeft(CurveVerticalLine, position.X);
            Canvas.SetTop(CurveHorizintalLine, position.Y);

            Canvas.SetTop(CurveInfoBorder, position.Y);
            Canvas.SetLeft(CurveInfoBorder, position.X);

            double t = _duration * (position.X / _curve.ActualWidth);
            double ft = (double)_easeFunc.Invoke(null, new object[4] { t, _startValue, _endValue, _duration });

            CurveCurrentValue.Text = string.Format("t: {0}", t.ToString("0.0000"));
            CurveCurrentTime.Text = string.Format("f(t): {0}", ft.ToString("0.0000"));   
        }

        private void FunctionCurve_MouseEnter(object sender, MouseEventArgs e)
        {
            CurveInfoBorder.Visibility = System.Windows.Visibility.Visible;
        }

        private void FunctionCurve_MouseLeave(object sender, MouseEventArgs e)
        {
            CurveInfoBorder.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
