using KlarfViewer.Model;
using KlarfViewer.Service;
using System.ComponentModel;

namespace KlarfViewer.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly KlarfParsingService klarfParser;
        private KlarfData currentKlarfData;
        private WaferInfo waferInfo;

        public WaferMapViewModel WaferMapVM { get; private set; }
        public DefectImageViewModel DefectImageVM { get; private set; }
        public FileListViewModel FileListVM { get; private set; }
        public DefectListViewModel DefectListVM { get; private set; }

        public WaferInfo WaferInfomation 
        { 
            get => waferInfo;
            set => SetProperty(ref waferInfo, value);
        }

        public MainViewModel()
        {
            klarfParser = new KlarfParsingService();

            WaferMapVM = new WaferMapViewModel();
            DefectImageVM = new DefectImageViewModel();
            FileListVM = new FileListViewModel();
            waferInfo = new WaferInfo();
            DefectListVM = new DefectListViewModel(waferInfo);

            FileListVM.FileSelected += FileViewerVM_FileSelected;
            DefectListVM.PropertyChanged += DefectListVM_PropertyChanged;
            WaferMapVM.DieSelected += WaferMapVM_DieSelected;
        }

        private void FileViewerVM_FileSelected(string filePath)
        {
            currentKlarfData = klarfParser.Parse(filePath);
            WaferMapVM.UpdateWaferData(currentKlarfData);
            DefectListVM.UpdateData(currentKlarfData);
        }

        private void WaferMapVM_DieSelected(DieViewModel die)
        {
            if (die == null) return;

            // 클릭된 다이와 동일한 인덱스를 가진 첫 번째 결함을 찾음
            var correspondingDefect = DefectListVM.DefectSpec.FirstOrDefault(d => d.XIndex == die.XIndex && d.YIndex == die.YIndex);
            if (correspondingDefect != null)
            {
                DefectListVM.SelectedDefect = correspondingDefect;
            }
        }

        private void DefectListVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DefectListViewModel.SelectedDefect))
            {
                var selectedDefect = DefectListVM.SelectedDefect;
                if (selectedDefect != null && currentKlarfData != null)
                {
                    WaferMapVM.SelectedDefect = selectedDefect;

                    string tiffFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(currentKlarfData.FilePath), currentKlarfData.Wafer.TiffFilename);
                    DefectImageVM.UpdateImage(tiffFilePath, selectedDefect.ImageId);
                }
            }
        }
    }
}
