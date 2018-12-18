using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RealEstateAgency
{
    /// <summary>
    /// Логика взаимодействия для RangeSliderBar.xaml
    /// </summary>
    public partial class RangeSliderBar : UserControl, INotifyPropertyChanged
    {
        public RangeSliderBar()
        {
            InitializeComponent();
            this.Loaded += RangeSlider_Loaded;
        }
        void RangeSlider_Loaded(object sender, RoutedEventArgs e)
        {
            LowerSlider.ValueChanged += LowerSlider_ValueChanged;
            UpperSlider.ValueChanged += UpperSlider_ValueChanged;
            UpperSlider.Value = Math.Max(UpperSlider.Value, LowerSlider.Value);
            LowerSlider.Value = Math.Min(UpperSlider.Value, LowerSlider.Value);
            UpperValue = Maximum;
            OnPropertyChange("TextS");
        }

        private void LowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpperSlider.Value = Math.Max(UpperSlider.Value, LowerSlider.Value);
            OnPropertyChange("TextS");
        }

        private void UpperSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LowerSlider.Value = Math.Min(UpperSlider.Value, LowerSlider.Value);
            OnPropertyChange("TextS");
        }
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(RangeSliderBar), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });

        public double LowerValue
        {
            get { return (double)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value);  }
        }

        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(double), typeof(RangeSliderBar), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });
        public double UpperValue
        {
            get { return (double)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); }
        }

        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(double), typeof(RangeSliderBar), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(RangeSliderBar), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        public string TextS
        {
            get
            {
                if (UpperValue == Maximum)
                {
                    return "от " + ((int)LowerValue).ToString();
                }
                if ((int)LowerValue == (int)UpperValue) return ((int)LowerValue).ToString();
                return ((int)LowerValue).ToString() + " - " + ((int)UpperValue).ToString();
            }
        }

    }
}
