using KlarfViewer.Model;
using KlarfViewer.Service;
using System.ComponentModel;
using KlarfViewer.Command;
using System.Windows.Input;
using KlarfViewer.View;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        public ImageProcessingViewModel ImageProcessingVM { get; private set; }
        public ExportCsvCommand CsvCommand { get; private set; }
        public HistogramViewModel HistogramVM { get; private set; }

        public Dictionary<int, BitmapSource> ModifiedImages { get; } = new Dictionary<int, BitmapSource>();

        private HistogramWindow _histogramWindow;
        private ImageProcessingWindow _imageProcessingWindow;

        public ICommand ShowHistogramCommand { get; private set; }
        public ICommand ShowImageProcessingWindowCommand { get; private set; }

        public void AddModifiedImage(int defectId, BitmapSource image)
        {
            ModifiedImages[defectId] = image;
        }

        public MainViewModel()
        {
            klarfParser = new KlarfParsingService();

            // Initialize child ViewModels
            WaferMapVM = new WaferMapViewModel();
            DefectImageVM = new DefectImageViewModel(this);
            FileListVM = new FileListViewModel(this);
            DefectListVM = new DefectListViewModel();
            ImageProcessingVM = new ImageProcessingViewModel(DefectImageVM, this);
            HistogramVM = new HistogramViewModel(this);

            _histogramWindow = new HistogramWindow() { DataContext = HistogramVM };
            _imageProcessingWindow = new ImageProcessingWindow() { DataContext = ImageProcessingVM };

            // Command
            CsvCommand = new ExportCsvCommand(this);
            ShowImageProcessingWindowCommand = new RelayCommand(() =>
            {
                if (_imageProcessingWindow == null || !_imageProcessingWindow.IsVisible)
                {
                    _imageProcessingWindow = new ImageProcessingWindow() { DataContext = ImageProcessingVM };
                    _imageProcessingWindow.Show();
                }
                else
                {
                    _imageProcessingWindow.Activate();
                }
            }, () => DefectImageVM.DefectImage != null);

            ShowHistogramCommand = new RelayCommand(() => 
            {
                if (_histogramWindow == null || !_histogramWindow.IsVisible)
                {
                    _histogramWindow = new HistogramWindow() { DataContext = HistogramVM };
                    _histogramWindow.Show();
                }
                else
                {
                    _histogramWindow.Activate();
                }
            }, () => DefectImageVM.DefectImage != null);

            // Subscribe to events from child VMs to handle synchronization
            FileListVM.FileSelected += OnFileSelected;
            DefectListVM.PropertyChanged += OnDefectSelectionChanged; // DefectViewer 
            WaferMapVM.DieClicked += OnDieClicked;

        }
        private async void OnFileSelected(string filePath)
        {
            FileListVM.IsParsing = true;
            FileListVM.ParsingProgress = 0;

            var progress = new Progress<double>(percentage =>
            {
                FileListVM.ParsingProgress = percentage;
            });

            try
            {
                currentKlarfData = await klarfParser.ParseAsync(filePath, progress);
                WaferMapVM.LoadData(currentKlarfData);
                DefectListVM.LoadData(currentKlarfData);
            }
            catch (Exception ex)
            {
                // Handle exceptions from parsing
                Console.WriteLine($"Error parsing file: {ex.Message}");
                // Optionally, show an error message to the user
            }
            finally
            {
                FileListVM.IsParsing = false;
            }
        }

        // Defect List => WaferMapVM/DefectImageVM
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

        // DefectListVM
        private void OnDieClicked(DieInfo clickedDie)
        {
            if (clickedDie == null) return;

            // Tell DefectList to select the corresponding defect
            DefectListVM.SelectDefectAt(clickedDie.XIndex, clickedDie.YIndex);
        }
    }
}
