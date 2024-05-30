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

namespace NetworkService.Views
{
    /// <summary>
    /// Interaction logic for NetworkEntitiesView.xaml
    /// </summary>
    public partial class NetworkEntitiesView : UserControl
    {
        public NetworkEntitiesView()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Check if the click was outside the DataGrid
            if (!dataGrid.IsMouseOver)
            {
                if (dataGrid.SelectedItem != null)
                {
                    dataGrid.SelectedItem = null;
                }
            }

            //Bandage fix to remove focus from the id textbox...
            if (textBox.IsFocused)
            {
                comboBox.Focus();
            }
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AutoFillButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveToProfileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FilterButton1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveToProfileButton1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (dataGrid.SelectedItem != null)
            //{
            //    dataGrid.SelectedItem = null;
            //}
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
