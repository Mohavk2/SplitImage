using SplitImage.Services.Slicers.Interfaces;
using SplitImage.Services.Slicers.Structures;
using SplitImage.ViewModels.SlicerProviders;
using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;

namespace SplitImage.Views.SlicerProviders
{
    /// <summary>
    /// Interaction logic for GridSlicerProviderUC.xaml
    /// </summary>
    public partial class GridSlicerProviderUC : UserControl, ISlicerProvider
    {
        public GridSlicerProviderUC()
        {
            InitializeComponent();
        }

        public event SliceSettingsStatusChangedHandler? SettingStatusChanged
        {
            add
            {
                if (this.DataContext is ISlicerProvider vm)
                {
                    vm.SettingStatusChanged += value;
                }
            }

            remove
            {
                if (this.DataContext is ISlicerProvider vm)
                {
                    vm.SettingStatusChanged -= value;
                }
            }
        }

        public SettingStatus GetSettingStatus()
        {
            if (this.DataContext is ISlicerProvider vm)
            {
                return vm.GetSettingStatus();
            }
            //TODO: replace 'return null slicer' with an exception and logging
            return new SettingStatus { slicer = null, isSettingCompleted = false };
        }

        private void Rows_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out var value))
            {
                e.Handled = true;
            }
        }

        private void Columns_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out var value))
            {
                e.Handled = true;
            }
        }
    }
}
