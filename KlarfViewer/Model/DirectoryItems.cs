using System.Collections.ObjectModel;
using System.IO;

namespace KlarfViewer.Model
{
    public class DirectoryItems {
        public string Name { get; set; }
        public string FullPath { get; set; }
        
        
        public ObservableCollection<DirectoryItems> Children { get; set; }

        public DirectoryItems(string path)
        {
            FullPath = path;
            Name = Path.GetFileName(path);
            Children = new ObservableCollection<DirectoryItems>();
        }
    } 
}
