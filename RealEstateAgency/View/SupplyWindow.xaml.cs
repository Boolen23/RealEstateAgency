using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RealEstateAgency.View
{
    /// <summary>
    /// Логика взаимодействия для SupplyWindow.xaml
    /// </summary>
    public partial class SupplyWindow : Window
    {
        public SupplyWindow()
        {
            InitializeComponent();
        }
        private void grid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == "Image")
                e.Cancel = true;
            grid.CellStyle = TryFindResource("CellCenter") as Style;
            grid.ColumnHeaderStyle = TryFindResource("HeaderCenter") as Style;
            grid.RowHeaderStyle = TryFindResource("TransparentRowHeaderStyle") as Style;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Photo.Visibility = Visibility.Hidden;
        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Photo.Visibility = Visibility.Visible;

        }

    }
}
