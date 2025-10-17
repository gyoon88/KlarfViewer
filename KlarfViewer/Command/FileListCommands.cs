using KlarfViewer.ViewModel;
using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.Intrinsics.X86;
using System.Windows.Input;

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

        public ICommand OpenFolderCommand { get; }
        public ICommand SelectedItemChangedCommand { get; }
        public ICommand RefreshCommand { get; }

        public FileListCommands(FileListViewModel viewModel)
        {
            vm = viewModel;
            OpenFolderCommand = new RelayCommand(ExecuteOpenFolder);
            RefreshCommand = new RelayCommand(ExecuteRefresh);
            SelectedItemChangedCommand = new RelayCommand<object>(ExecuteSelectedItemChanged);
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
            vm.Directories.Clear();
            vm.Files.Clear();
            vm.SelectedDirectory = null;
        }
    }
}
