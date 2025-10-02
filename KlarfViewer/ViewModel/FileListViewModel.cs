
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace KlarfViewer.ViewModel
{
    public class FileListViewModel : BaseViewModel
    {
        public event Action<string> FileSelected;

        private FileSystemObjectViewModel _selectedItem;
        public FileSystemObjectViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                if (value != null && value.IsFile && Path.GetExtension(value.FullPath).Equals(".klarf", StringComparison.OrdinalIgnoreCase))
                {
                    FileSelected?.Invoke(value.FullPath);
                }
            }
        }

        public ObservableCollection<FileSystemObjectViewModel> Items { get; set; }
        public ICommand OpenFolderCommand { get; }

        public FileListViewModel()
        {
            Items = new ObservableCollection<FileSystemObjectViewModel>();
            OpenFolderCommand = new RelayCommand(ExecuteOpenFolder);
        }

        private void ExecuteOpenFolder(object obj)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedPath = dialog.SelectedPath;
                Items.Clear();
                var rootNode = new FileSystemObjectViewModel(selectedPath);
                LoadSubDirectoriesAndFiles(rootNode);
                Items.Add(rootNode);
            }
        }

        private void LoadSubDirectoriesAndFiles(FileSystemObjectViewModel parentNode)
        {
            try
            {
                foreach (var dirPath in Directory.GetDirectories(parentNode.FullPath))
                {
                    var subDirNode = new FileSystemObjectViewModel(dirPath);
                    LoadSubDirectoriesAndFiles(subDirNode);
                    if (subDirNode.Children.Count > 0) 
                    {
                        parentNode.Children.Add(subDirNode);
                    }
                }

                foreach (var filePath in Directory.GetFiles(parentNode.FullPath, "*.klarf"))
                {
                    var fileNode = new FileSystemObjectViewModel(filePath);
                    parentNode.Children.Add(fileNode);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore folders with no access rights
            }
        }
    }
}
