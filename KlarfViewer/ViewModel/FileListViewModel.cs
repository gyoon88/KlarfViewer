using KlarfViewer.Service;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using static System.Net.WebRequestMethods;


namespace KlarfViewer.ViewModel
{
    public class FileListViewModel : BaseViewModel
    {
        private readonly FileSystemService fileSystemService;
        public event Action<string> FileSelected;

        private FileSystemObjectViewModel selectedDirectory;
        public FileSystemObjectViewModel SelectedDirectory
        {
            get => selectedDirectory;
            set
            {
                if (SetProperty(ref selectedDirectory, value) && value != null) // if value change with not null
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
        public ICommand SelectedItemChangedCommand { get; }
        public ICommand RefreshCommand { get; }

        public FileListViewModel()
        {
            fileSystemService = new FileSystemService();
            Directories = new ObservableCollection<FileSystemObjectViewModel>();
            Files = new ObservableCollection<FileSystemObjectViewModel>();
            OpenFolderCommand = new RelayCommand(ExecuteOpenFolder);
            RefreshCommand = new RelayCommand(ExecuteRefresh);

            SelectedItemChangedCommand = new RelayCommand<object>(ExecuteSelectedItemChanged);
        }

        private void ExecuteSelectedItemChanged(object selectedItem)
        {
            if (selectedItem is FileSystemObjectViewModel fso && fso.IsDirectory)
            {
                SelectedDirectory = fso;
            }
        }

        //private void ExecuteOpenFolder()
        //{
        //    var dialog = new VistaFolderBrowserDialog
        //    {
        //        Description = "Select a folder.",
        //        UseDescriptionForTitle = true
        //    };

        //    if (dialog.ShowDialog() == true)
        //    {
        //        string selectedPath = dialog.SelectedPath;
        //        Directories.Clear();
        //        Files.Clear();
        //        var rootNode = new FileSystemObjectViewModel(selectedPath, isDirectory: true);
        //        fileSystemService.LoadSubDirectories(rootNode);
        //        Directories.Add(rootNode);
        //        SelectedDirectory = rootNode;
        //    }
        //}

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
                if (dialog.FileNames.Length == 0)
                {
                    return;
                }
                string selectedPath = System.IO.Path.GetDirectoryName(dialog.FileNames[0]);

                Directories.Clear();
                Files.Clear();

                // 디렉토리 구조를 로드
                var rootNode = new FileSystemObjectViewModel(selectedPath, isDirectory: true);
                fileSystemService.LoadSubDirectories(rootNode); // 하위 폴더 로드
                Directories.Add(rootNode);

                SelectedDirectory = rootNode;


            }
        }
        private void ExecuteRefresh()
        {

            Directories.Clear();
            Files.Clear();           

        }

        private void LoadFiles(FileSystemObjectViewModel directoryNode)
        {
            Files.Clear();
            foreach (var fileNode in fileSystemService.GetFiles(directoryNode.FullPath))
            {
                Files.Add(fileNode);
            }
        }
    }
}