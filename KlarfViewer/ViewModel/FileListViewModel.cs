using KlarfViewer.Service;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using KlarfViewer.Command;

namespace KlarfViewer.ViewModel
{
    public class FileListViewModel : BaseViewModel
    {
        public readonly  FileSystemService fileSystemService;
        public event Action<string>? FileSelected;

        private FileSystemObjectViewModel? selectedDirectory;
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

        
        
        private FileSystemObjectViewModel? selectedFile;
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

        
        
        private bool isParsing;
        public bool IsParsing
        {
            get => isParsing;
            set => SetProperty(ref isParsing, value);
        }

        private double parsingProgress;
        public double ParsingProgress
        {
            get => parsingProgress;
            set => SetProperty(ref parsingProgress, value);
        }

        private FileListCommands? commands;
        public FileListCommands Commands
        {
            get => commands;
            set => SetProperty(ref commands, value);
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
            Commands = new FileListCommands(this);
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
