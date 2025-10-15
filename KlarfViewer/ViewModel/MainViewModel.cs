using KlarfViewer.Model;
using KlarfViewer.Service;
using System.ComponentModel;
using System.Linq;

namespace KlarfViewer.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly KlarfParsingService klarfParser;
        private KlarfData currentKlarfData; // The single source of truth

        public WaferMapViewModel WaferMapVM { get; private set; }
        public DefectImageViewModel DefectImageVM { get; private set; }
        public FileListViewModel FileListVM { get; private set; }
        public DefectListViewModel DefectListVM { get; private set; }

        public MainViewModel()
        {
            klarfParser = new KlarfParsingService();

            // Initialize child ViewModels
            WaferMapVM = new WaferMapViewModel();
            DefectImageVM = new DefectImageViewModel();
            FileListVM = new FileListViewModel();
            DefectListVM = new DefectListViewModel();

            // Subscribe to events from child VMs to handle synchronization
            FileListVM.FileSelected += OnFileSelected;
            DefectListVM.PropertyChanged += OnDefectSelectionChanged;
            WaferMapVM.DieClicked += OnDieClicked;
        }

        private void OnFileSelected(string filePath)
        {
            currentKlarfData = klarfParser.Parse(filePath);
            
            // Give each child VM a reference to the shared data model
            WaferMapVM.LoadData(currentKlarfData);
            DefectListVM.LoadData(currentKlarfData);
        }

        private void OnDefectSelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(DefectListViewModel.SelectedDefect)) return;

            var selectedDefect = DefectListVM.SelectedDefect;
            if (selectedDefect == null) return;

            // Tell WaferMap to highlight the corresponding die
            WaferMapVM.HighlightDieAt(selectedDefect.XIndex, selectedDefect.YIndex);

            // Tell DefectImage to update the image
            if (currentKlarfData != null)
            {
                string tiffFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(currentKlarfData.FilePath), currentKlarfData.Wafer.TiffFilename);
                DefectImageVM.UpdateImage(tiffFilePath, selectedDefect.Id);
            }
        }

        private void OnDieClicked(DieInfo clickedDie)
        {
            if (clickedDie == null) return;

            // Tell DefectList to select the corresponding defect
            DefectListVM.SelectDefectAt(clickedDie.XIndex, clickedDie.YIndex);
        }
    }
}
