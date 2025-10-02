using KlarfViewer.Model;
using KlarfViewer.Service;
using System.ComponentModel;

namespace KlarfViewer.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly KlarfParsingService _klarfParser;
        private KlarfData _currentKlarfData;

        public WaferMapViewModel WaferMapVM { get; private set; }
        public DefectImageViewModel DefectImageVM { get; private set; }
        public FileListViewModel FileViewerVM { get; private set; }
        public DefectListViewModel DefectListVM { get; private set; }

        public MainViewModel()
        {
            _klarfParser = new KlarfParsingService();

            WaferMapVM = new WaferMapViewModel();
            DefectImageVM = new DefectImageViewModel();
            FileViewerVM = new FileListViewModel();
            DefectListVM = new DefectListViewModel();

            FileViewerVM.FileSelected += FileViewerVM_FileSelected;
            DefectListVM.PropertyChanged += DefectListVM_PropertyChanged;
        }

        private void FileViewerVM_FileSelected(string filePath)
        {
            _currentKlarfData = _klarfParser.Parse(filePath);
            WaferMapVM.UpdateWaferData(_currentKlarfData);
            DefectListVM.UpdateDefects(_currentKlarfData);
        }

        private void DefectListVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DefectListViewModel.SelectedDefect))
            {
                var selectedDefect = DefectListVM.SelectedDefect;
                if (selectedDefect != null && _currentKlarfData != null)
                {
                    WaferMapVM.SelectedDefect = selectedDefect;

                    string tiffFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(_currentKlarfData.FilePath), _currentKlarfData.Wafer.TiffFilename);
                    DefectImageVM.UpdateImage(tiffFilePath, selectedDefect.ImageId);
                }
            }
        }
    }
}
