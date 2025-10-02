using System.Collections.ObjectModel;
using System.IO;

namespace KlarfViewer.Model
{
    public class DirectoryItems {
        public string Name { get; set; }
        public string FullPath { get; set; }
        // 자식 폴더들을 담을 컬렉션 (재귀 구조의 핵심)
        public ObservableCollection<DirectoryItems> Children { get; set; }

        public DirectoryItems(string path)
        {
            FullPath = path;
            Name = Path.GetFileName(path);
            Children = new ObservableCollection<DirectoryItems>();
        }
    } 
}
