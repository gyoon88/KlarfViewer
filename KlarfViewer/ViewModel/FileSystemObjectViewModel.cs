using System.Collections.ObjectModel;
using System.IO;

namespace KlarfViewer.ViewModel
{
    public class FileSystemObjectViewModel : BaseViewModel
    {
        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string fullPath;
        public string FullPath
        {
            get => fullPath;
            set => SetProperty(ref fullPath, value);
        }

        private ObservableCollection<FileSystemObjectViewModel> children;
        public ObservableCollection<FileSystemObjectViewModel> Children
        {
            get => children;
            set => SetProperty(ref children, value);
        }
        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        public bool IsDirectory { get; }

        public DateTime LastModified { get; set; }
        public FileSystemObjectViewModel(string path, bool isDirectory)
        {
            FullPath = path;
            Name = Path.GetFileName(path);
            if (string.IsNullOrEmpty(Name))
                Name = FullPath;

            IsDirectory = isDirectory;
            Children = new ObservableCollection<FileSystemObjectViewModel>();
            if (!IsDirectory)
            {
                LastModified = File.GetLastWriteTime(path);
            }
        }
    }
}