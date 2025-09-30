using System.Collections.ObjectModel;
using System.IO;
namespace KlarfViewer.ViewModel
{
    public class FileSystemObjectViewModel
    {
        // 화면에 표시될 이름 (예: "MyFolder" 또는 "image.tif")
        public string Name { get; set; }

        // 해당 파일 또는 폴더의 전체 경로
        public string FullPath { get; set; }

        // 이 항목이 가진 자식 항목들의 리스트 (하위 폴더 및 파일)
        // 폴더인 경우에만 여기에 아이템이 추가됩니다.
        public ObservableCollection<FileSystemObjectViewModel> Children { get; set; }

        public FileSystemObjectViewModel(string path)
        {
            FullPath = path;
            Name = Path.GetFileName(path);
            // 파일의 경우 이름이 없으면 전체 경로를 이름으로 사용 (예: C:\)
            if (string.IsNullOrEmpty(Name))
                Name = FullPath;

            Children = new ObservableCollection<FileSystemObjectViewModel>();
        }
    }
}
