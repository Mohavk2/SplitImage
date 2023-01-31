using SkillBase.ViewModels.Common;
using SplitImage.Helpers;
using SplitImage.Services;
using SplitImage.Services.Slicers.Interfaces;
using SplitImage.Services.Slicers.Structures;
using SplitImage.ViewModels.Common;
using SplitImage.Views.SlicerProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using DrawingImage = System.Drawing.Image;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Drawing;
using System.Drawing.Imaging;

namespace SplitImage.ViewModels
{
    internal class MainWindowVM : BaseViewModel, IDisposable
    {

        BackgroundWorker worker;

        public MainWindowVM()
        {
            SlicerNames = new() { "Grid" };
            currentSlicerName = "Grid";

            var slicerSettingView = new GridSlicerProviderUC();
            slicerSettingView.SettingStatusChanged += HandleSlicerSettingsChanged;
            HandleSlicerSettingsChanged(slicerSettingView.GetSettingStatus());
            currentSlicerProviderView = slicerSettingView;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += SliceImages;
            worker.ProgressChanged += SlicingProgressChanged;
            worker.RunWorkerCompleted += SlicingCompleted;
        }

        string sourcePath = string.Empty;
        public string SourcePath
        {
            get => sourcePath;
            set
            {
                sourcePath = value;
                RaisePropertyChanged(nameof(SourcePath));
            }
        }

        public UICommand ChooseSourceDirectory => new UICommand((parameter) =>
        {
            var path = SelectDirectoryDialog.ShowDialog("Select source directory");
            if (path != string.Empty)
            {
                SourcePath = path;
            }
        });

        string destinationPath = string.Empty;
        public string DestinationPath
        {
            get => destinationPath;
            set
            {
                destinationPath = value;
                RaisePropertyChanged(nameof(DestinationPath));
            }
        }

        public UICommand ChooseDestinationDirectory => new UICommand((parameter) =>
        {
            var path = DestinationPath = SelectDirectoryDialog.ShowDialog("Select destination directory");
            if (path != string.Empty)
            {
                DestinationPath = path;
            }
        });

        public List<string> SlicerNames { get; }

        string currentSlicerName;
        public string CurrentSlicerName
        {
            get => currentSlicerName;
            set
            {
                currentSlicerName = value;

                switch (value)
                {
                    case "Grid":
                        CurrentSlicerProviderView = new GridSlicerProviderUC();
                        break;
                }

                RaisePropertyChanged(nameof(CurrentSlicerName));
            }
        }

        ISlicerProvider currentSlicerProviderView;
        public UserControl CurrentSlicerProviderView
        {
            get => (currentSlicerProviderView as UserControl) ?? new UserControl();
            set
            {
                if (value is ISlicerProvider val)
                {
                    currentSlicerProviderView.SettingStatusChanged -= HandleSlicerSettingsChanged;
                    currentSlicerProviderView = val;
                    currentSlicerProviderView.SettingStatusChanged += HandleSlicerSettingsChanged;
                    HandleSlicerSettingsChanged(currentSlicerProviderView.GetSettingStatus());

                    RaisePropertyChanged(nameof(CurrentSlicerProviderView));
                }
            }
        }

        ISlicer? currentSlicer;

        void HandleSlicerSettingsChanged(SettingStatus status)
        {
            currentSlicer = status.isSettingCompleted ? status.slicer : null;
        }

        public UICommand Slice => new((paremeter) =>
        {
            SlicingStatus = "Processing...";
            worker.RunWorkerAsync();

        }, (parameter) =>
        {
            return currentSlicer != null
                && SourcePath != string.Empty
                && DestinationPath != string.Empty;
        });


        private void SliceImages(object? sender, DoWorkEventArgs e)
        {
            if (currentSlicer != null)
            {
                var fileFormats = currentSlicer.SupportedImageFormats;
                var images = DirectoryHelper.GetFilesFrom(SourcePath, fileFormats, true);
                float percents = 0;
                foreach (var image in images)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        percents += 100f / images.Length;
                        string? destinationDirectory = Path.GetDirectoryName(image)?.Replace(SourcePath, DestinationPath);
                        if (destinationDirectory != null)
                        {
                            Directory.CreateDirectory(destinationDirectory);

                            Bitmap[,] pieces = currentSlicer.Slice(new(image));

                            for (int i = 0; i < pieces.GetLength(0); i++)
                            {
                                for (int j = 0; j < pieces.GetLength(1); j++)
                                {
                                    string pieceName = "Piece_" + i + '_' + j + "_" + Path.GetFileName(image);
                                    pieces[i, j].Save(Path.Combine(destinationDirectory, pieceName), ImageFormat.Png);
                                }
                            }
                        }
                        worker.ReportProgress((int)percents);
                    }
                }
            }
            else
            {
                e.Cancel = true;
            }
        }


        int slicingProgress = 0;
        public int SlicingProgress
        {
            get => slicingProgress;
            set
            {
                slicingProgress = value;
                RaisePropertyChanged(nameof(SlicingProgress));
            }
        }

        void SlicingProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            SlicingProgress = (int)e.ProgressPercentage;
        }

        string slicingStatus = "";
        public string SlicingStatus
        {
            get => slicingStatus;
            set
            {
                slicingStatus = value;
                RaisePropertyChanged(nameof(SlicingStatus));
            }
        }

        public UICommand CancelSlicing => new((perameter) =>
        {
            worker.CancelAsync();

        }, (p) => worker.IsBusy);

        private void SlicingCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            SlicingStatus = e.Cancelled ? "Cancelled" : "Complete";
            SlicingProgress = 0;
        }

        public void Dispose()
        {
            currentSlicerProviderView.SettingStatusChanged -= HandleSlicerSettingsChanged;
            worker.DoWork -= SliceImages;
            worker.ProgressChanged -= SlicingProgressChanged;
            worker.RunWorkerCompleted -= SlicingCompleted;
        }
    }
}
