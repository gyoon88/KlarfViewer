using KlarfViewer.Model;
using KlarfViewer.Service;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace KlarfViewer.ViewModel
{
    public class WaferMapViewModel : BaseViewModel
    {
        private readonly WaferMapService _waferMapService;
        private const double VIRTUAL_CANVAS_SIZE = 1000.0;

        private WaferInfo waferInfomation;
        public WaferInfo WaferInfomation
        {
            get => waferInfomation;
            private set => SetProperty(ref waferInfomation, value);
        }

        private ObservableCollection<DieViewModel> dies;
        public ObservableCollection<DieViewModel> Dies
        {
            get => dies;
            private set => SetProperty(ref dies, value);
        }

        private double waferMapWidth;
        public double WaferMapWidth
        {
            get => waferMapWidth;
            private set => SetProperty(ref waferMapWidth, value);
        }

        private double waferMapHeight;
        public double WaferMapHeight
        {
            get => waferMapHeight;
            private set => SetProperty(ref waferMapHeight, value);
        }

        public ICommand SelectDieCommand { get; }
        public event Action<DieViewModel> DieSelected;

        private DefectInfo selectedDefect;
        public DefectInfo SelectedDefect
        {
            get => selectedDefect;
            set
            {
                selectedDefect = value;
                OnPropertyChanged(nameof(SelectedDefect));
                UpdateDieSelection();
            }
        }

        public WaferMapViewModel()
        {
            Dies = new ObservableCollection<DieViewModel>();
            _waferMapService = new WaferMapService();
            SelectDieCommand = new RelayCommand<DieViewModel>(ExecuteSelectDie, CanExecuteSelectDie);
            UpdateWaferData(null); // 기본 맵 생성
        }

        private bool CanExecuteSelectDie(DieViewModel die)
        {
            return die != null && die.IsDefective;
        }

        private void ExecuteSelectDie(DieViewModel die)
        {
            DieSelected?.Invoke(die);
        }

        public void UpdateWaferData(KlarfData klarfData)
        {
            var renderData = _waferMapService.CalculateWaferMapRender(klarfData, VIRTUAL_CANVAS_SIZE);

            WaferInfomation = renderData.WaferInfo;

            var newDies = new ObservableCollection<DieViewModel>();
            if (renderData.DieRenders != null)
            {
                foreach (var dieRenderInfo in renderData.DieRenders)
                {
                    var dieVM = new DieViewModel(dieRenderInfo.OriginalDie)
                    {
                        Width = dieRenderInfo.Width,
                        Height = dieRenderInfo.Height,
                        X = dieRenderInfo.X,
                        Y = dieRenderInfo.Y
                    };
                    newDies.Add(dieVM);
                }
            }
            Dies = newDies;
            
            WaferMapWidth = renderData.WaferMapWidth;
            WaferMapHeight = renderData.WaferMapHeight;

            UpdateDieSelection();
        }

        private void UpdateDieSelection()
        {
            if (Dies == null) return;
            foreach (var dieVM in Dies)
            {
                bool isSelected = SelectedDefect != null &&
                                  dieVM.XIndex == SelectedDefect.XIndex &&
                                  dieVM.YIndex == SelectedDefect.YIndex;
                dieVM.IsSelected = isSelected;
            }
        }
    }
}