using System.Collections.ObjectModel;
using System.IO;

namespace KlarfViewer.ViewModel
{
    public class FileSystemObjectViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public ObservableCollection<FileSystemObjectViewModel> Children { get; set; }
        public bool IsDirectory { get; }

        public bool IsFile => !IsDirectory;

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public FileSystemObjectViewModel(string path, bool isDirectory)
        {
            FullPath = path;
            Name = Path.GetFileName(path);
            if (string.IsNullOrEmpty(Name))
                Name = FullPath;

            IsDirectory = isDirectory;
            Children = new ObservableCollection<FileSystemObjectViewModel>();
        }

        public FileSystemObjectViewModel(string path)
        {
            FullPath = path;
            Name = Path.GetFileName(path);
            if (string.IsNullOrEmpty(Name))
                Name = FullPath;

            IsDirectory = Directory.Exists(path);
            Children = new ObservableCollection<FileSystemObjectViewModel>();
        }
    }
}