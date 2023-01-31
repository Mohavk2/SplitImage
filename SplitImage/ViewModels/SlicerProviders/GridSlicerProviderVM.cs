using SkillBase.ViewModels.Common;
using SplitImage.Services.Slicers;
using SplitImage.Services.Slicers.Interfaces;
using SplitImage.Services.Slicers.Structures;
using SplitImage.ViewModels.Common;
using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SplitImage.ViewModels.SlicerProviders
{
    internal class GridSlicerProviderVM : BaseViewModel, ISlicerProvider
    {
        public event SliceSettingsStatusChangedHandler? SettingStatusChanged;

        GridSlicer slicer = new GridSlicer();

        public GridSlicerProviderVM()
        {
            VisualExample = new Canvas();
            VisualExample.Width = 150;
            VisualExample.Height = 100;
            var backgroundImgURI = new Uri("pack://application:,,,/placeholder.png");
            var backgroundImg = new ImageBrush(new BitmapImage(backgroundImgURI));
            VisualExample.Background = backgroundImg;
        }

        public int Rows
        {
            get => slicer.Rows;
            set
            {
                slicer.Rows = value < GridSlicer.MinRows ? GridSlicer.MinRows 
                            : value > GridSlicer.MaxColumns ? GridSlicer.MaxColumns : value;

                RaisePropertyChanged(nameof(Rows));
                RedrawVisualExample();
                RaiseSettingsStatusChanged();
            }
        }

        public UICommand RowUp => new((parameter) =>
        {
            Rows += 1;
        }, (p) => Rows < GridSlicer.MaxRows);

        public UICommand RowDown => new((parameter) =>
        {
            Rows -= 1;
        }, (p) => Rows > GridSlicer.MinRows);

        public int Columns
        {
            get => slicer.Columns;
            set
            {
                slicer.Columns = value < GridSlicer.MinColumns ? GridSlicer.MinColumns 
                               : value > GridSlicer.MaxColumns ? GridSlicer.MaxColumns : value;

                RaisePropertyChanged(nameof(Columns));
                RedrawVisualExample();
                RaiseSettingsStatusChanged();
            }
        }

        public UICommand ColumnUp => new((parameter) =>
        {
            Columns += 1;
        }, (p) => Columns < GridSlicer.MaxColumns);

        public UICommand ColumnDown => new((parameter) =>
        {
            Columns -= 1;
        }, (p) => Columns > GridSlicer.MinColumns);

        void RaiseSettingsStatusChanged()
        {
            SettingStatus status = new()
            { 
                slicer = slicer, 
                isSettingCompleted = true 
            };
            SettingStatusChanged?.Invoke(status);
        }

        public SettingStatus GetSettingStatus()
        {
            return new()
            {
                slicer = slicer,
                isSettingCompleted = true
            };
        }

        void RedrawVisualExample()
        {
            VisualExample.Children.Clear();

            int rowLines = Rows - 1;
            double rowGap = (VisualExample.Height - rowLines) / Rows;
            double rowOffset = 0;
            for (int i = 0; i < rowLines; i++)
            {
                Line line = CreateStyledLine();
                line.X1 = 0;
                line.X2 = VisualExample.Width;
                line.Y1 = line.Y2 = rowOffset += rowGap + line.StrokeThickness;

                VisualExample.Children.Add(line);
            }

            int columnLines = Columns - 1;
            double columnGap = (VisualExample.Width - columnLines) / Columns;
            double columnOffset = 0;
            for (int i = 0; i < columnLines; i++)
            {
                Line line = CreateStyledLine();
                line.Y1 = 0;
                line.Y2 = VisualExample.Height;
                line.X1 = line.X2 = columnOffset += columnGap + line.StrokeThickness;

                VisualExample.Children.Add(line);
            }
        }

        Line CreateStyledLine()
        {
            Line line = new();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 1;
            line.SnapsToDevicePixels = true;
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            return line;
        }

        public Canvas VisualExample { get; set; }
    }
}
