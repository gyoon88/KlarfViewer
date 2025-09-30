using KlarfViewer.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace KlarfViewer.ViewModel
{
    public class FileListViewModel
    {
        // View의 TreeView와 바인딩될 최상위 아이템 리스트
        public ObservableCollection<FileSystemObjectViewModel> Items { get; set; }

        // "폴더 열기" 버튼과 바인딩될 Command
        public RelayCommand OpenFolderCommand { get; }

        public FileListViewModel()
        {
            Items = new ObservableCollection<FileSystemObjectViewModel>();
            OpenFolderCommand = new RelayCommand(ExecuteOpenFolder); // RelayCommand는 직접 구현 필요
        }

        private void ExecuteOpenFolder(object obj)
        {
            // 1. 폴더 선택 대화상자 띄우기
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedPath = dialog.SelectedPath;

                // 2. 기존 목록 지우기
                Items.Clear();

                // 3. 선택된 폴더를 기준으로 트리 데이터 구조 생성
                var rootNode = new FileSystemObjectViewModel(selectedPath);
                LoadSubDirectoriesAndFiles(rootNode);
                Items.Add(rootNode);
            }
        }

        // 재귀적으로 하위 폴더와 파일을 로드하는 함수
        private void LoadSubDirectoriesAndFiles(FileSystemObjectViewModel parentNode)
        {
            try
            {
                // 1. 하위 폴더들 로드
                foreach (var dirPath in Directory.GetDirectories(parentNode.FullPath))
                {
                    var subDirNode = new FileSystemObjectViewModel(dirPath);
                    // 자기 자신을 다시 호출하여 더 깊은 하위 폴더들도 로드 (재귀)
                    LoadSubDirectoriesAndFiles(subDirNode);
                    parentNode.Children.Add(subDirNode);
                }

                // 2. 해당 폴더의 파일들 로드
                foreach (var filePath in Directory.GetFiles(parentNode.FullPath))
                {
                    // 파일 노드 추가 (파일은 자식이 없으므로 재귀 호출 안 함)
                    var fileNode = new FileSystemObjectViewModel(filePath);
                    parentNode.Children.Add(fileNode);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // 접근 권한이 없는 폴더는 무시
            }
        }
    }
}
