using Ookii.Dialogs.Wpf;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace KlarfViewer.ViewModel
{
    public class FileListViewModel : BaseViewModel
    {
        public event Action<string> FileSelected;

        private FileSystemObjectViewModel selectedDirectory;
        public FileSystemObjectViewModel SelectedDirectory
        {
            get => selectedDirectory;
            set
            {
                if (SetProperty(ref selectedDirectory, value) && value != null)
                {
                    LoadFiles(value);
                }
            }
        }

        private FileSystemObjectViewModel selectedFile;
        public FileSystemObjectViewModel SelectedFile
        {
            get => selectedFile;
            set
            {
                if (SetProperty(ref selectedFile, value) && value != null && !value.IsDirectory)
                {
                    var extension = Path.GetExtension(value.FullPath);
                    if (extension.Equals(".klarf", StringComparison.OrdinalIgnoreCase) ||
                        extension.Equals(".001", StringComparison.OrdinalIgnoreCase))
                    {
                        FileSelected?.Invoke(value.FullPath);
                    }
                }
            }
        }

        public ObservableCollection<FileSystemObjectViewModel> Directories { get; }
        public ObservableCollection<FileSystemObjectViewModel> Files { get; }

        public ICommand OpenFolderCommand { get; }

        public FileListViewModel()
        {
            Directories = new ObservableCollection<FileSystemObjectViewModel>();
            Files = new ObservableCollection<FileSystemObjectViewModel>();
            OpenFolderCommand = new RelayCommand(ExecuteOpenFolder);
        }

        private void ExecuteOpenFolder()
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Select a folder.",
                UseDescriptionForTitle = true
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = dialog.SelectedPath;
                Directories.Clear();
                Files.Clear();
                var rootNode = new FileSystemObjectViewModel(selectedPath, isDirectory: true);
                LoadSubDirectories(rootNode);
                Directories.Add(rootNode);
                SelectedDirectory = rootNode;
            }
        }

        private void LoadSubDirectories(FileSystemObjectViewModel parentNode)
        {
            try
            {
                foreach (var dirPath in Directory.GetDirectories(parentNode.FullPath))
                {
                    var subDirNode = new FileSystemObjectViewModel(dirPath, isDirectory: true);
                    LoadSubDirectories(subDirNode); 
                    parentNode.Children.Add(subDirNode);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore folders without access permissions
            }
        }

        private void LoadFiles(FileSystemObjectViewModel directoryNode)
        {
            Files.Clear();
            try
            {
                foreach (var filePath in Directory.GetFiles(directoryNode.FullPath))
                {
                    var fileNode = new FileSystemObjectViewModel(filePath, isDirectory: false);
                    Files.Add(fileNode);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore folders without access permissions
            }
        }
    }
}