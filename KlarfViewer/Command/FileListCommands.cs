using KlarfViewer.Service;
using KlarfViewer.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace KlarfViewer.Command
{
    /// <summary>
    /// Store Commands that be using FileListViewer
    /// OpenFolderCommand   
    /// SelectedItemChangedCommand 
    /// public ICommand RefreshCommand 
    /// </summary>
    /// 
    public class FileListCommands
    {
        private readonly FileListViewModel vm;
        private readonly MainViewModel mainVM;

        public ICommand OpenFolderCommand { get; }
        public ICommand SelectedItemChangedCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SaveAllModifiedImagesCommand { get; }

        public FileListCommands(FileListViewModel viewModel, MainViewModel mainViewModel)
        {
            vm = viewModel;
            mainVM = mainViewModel;
            OpenFolderCommand = new RelayCommand(ExecuteOpenFolder);
            RefreshCommand = new RelayCommand(ExecuteRefresh);
            SelectedItemChangedCommand = new RelayCommand<object>(ExecuteSelectedItemChanged);
            SaveAllModifiedImagesCommand = new RelayCommand(ExecuteSaveAllModifiedImages, CanExecuteSaveAllModifiedImages);
        }

        private bool CanExecuteSaveAllModifiedImages()
        {
            return mainVM.ModifiedImages.Count > 0;
        }

        private void ExecuteSaveAllModifiedImages()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save Modified Images",
                Filter = "TIF Files (*.tif)|*.tif",
                FileName = "modified_images.tif"
            };

            if (dialog.ShowDialog() == true)
            {
                var imagesToExport = new List<BitmapSource>();
                var currentKlarfData = mainVM.GetCurrentKlarfData();

                if (currentKlarfData == null || currentKlarfData.Defects == null)
                {
                    return;
                }

                string tiffFilePath = Path.Combine(Path.GetDirectoryName(currentKlarfData.FilePath), currentKlarfData.Wafer.TiffFilename);
                if (!File.Exists(tiffFilePath))
                {
                    // Handle file not found error
                    return;
                }

                var decoder = new TiffBitmapDecoder(
                    new Uri(tiffFilePath, UriKind.Absolute),
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad
                );

                foreach (var defect in currentKlarfData.Defects.OrderBy(d => d.Id))
                {
                    if (mainVM.ModifiedImages.TryGetValue(defect.Id, out BitmapSource modifiedImage))
                    {
                        imagesToExport.Add(modifiedImage);
                    }
                    else
                    {
                        int frameIndex = defect.Id - 1;
                        if (frameIndex >= 0 && frameIndex < decoder.Frames.Count)
                        {
                            imagesToExport.Add(decoder.Frames[frameIndex]);
                        }
                    }
                }

                mainVM.CsvCommand.ExportImages(dialog.FileName, imagesToExport);
            }
        }

        private void ExecuteSelectedItemChanged(object selectedItem)
        {
            if (selectedItem is FileSystemObjectViewModel fso && fso.IsDirectory)
            {
                vm.SelectedDirectory = fso;
            }
        }

        private void ExecuteOpenFolder()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Klarf file(s)",
                Filter = "Inspection Files (*.klarf, *.001)|*.klarf;*.001|All files (*.*)|*.*",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                if (dialog.FileNames.Length == 0) return;
                
                string selectedPath = Path.GetDirectoryName(dialog.FileNames[0]);

                vm.Directories.Clear();
                vm.Files.Clear();

                var rootNode = new FileSystemObjectViewModel(selectedPath, isDirectory: true);
                vm.fileSystemService.LoadSubDirectories(rootNode);
                vm.Directories.Add(rootNode);
                vm.SelectedDirectory = rootNode;
            }
        }

        private void ExecuteRefresh()
        {
            if (vm.SelectedDirectory != null)
            {
                string currentPath = vm.SelectedDirectory.FullPath;
                vm.Directories.Clear();
                vm.Files.Clear();

                var rootNode = new FileSystemObjectViewModel(currentPath, isDirectory: true);
                vm.fileSystemService.LoadSubDirectories(rootNode);
                vm.Directories.Add(rootNode);
                vm.SelectedDirectory = rootNode;
            }
        }
    }
}
