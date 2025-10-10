using KlarfViewer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using System.Windows.Input;

namespace KlarfViewer.ViewModel
{
    public class WaferMapViewModel : BaseViewModel
    {
        public WaferInfo WaferInfomation { get; private set; }
        public ObservableCollection<DieViewModel> Dies { get; private set; }
        public double WaferMapWidth { get; private set; }
        public double WaferMapHeight { get; private set; }

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
            var dieInfos = klarfData?.Dies;
            WaferInfomation = klarfData?.Wafer;

            if (dieInfos == null || !dieInfos.Any())
            {
                // 기본 데이터 생성
                WaferInfomation = new WaferInfo { DiePitch = new DieSize { Width = 20, Height = 20 } };
                dieInfos = new List<DieInfo>();
                for (int y = -15; y <= 15; y++)
                {
                    for (int x = -15; x <= 15; x++)
                    {
                        if (x * x + y * y < 15 * 15) // 원형 필터
                        {
                            ((List<DieInfo>)dieInfos).Add(new DieInfo { XIndex = x, YIndex = y });
                        }
                    }
                }
            }

            LoadDies(dieInfos);
            UpdateDieSelection();
        }

        private void LoadDies(IEnumerable<DieInfo> dieInfos)
        {
            Dies.Clear();
            if (dieInfos == null || !dieInfos.Any()) return;

            int minXIdx = dieInfos.Min(d => d.XIndex);
            int minYIdx = dieInfos.Min(d => d.YIndex);
            int maxXIdx = dieInfos.Max(d => d.XIndex);
            int maxYIdx = dieInfos.Max(d => d.YIndex);

            double diePitchWidth = (WaferInfomation?.DiePitch.Width > 0) ? WaferInfomation.DiePitch.Width : 1.0;
            double diePitchHeight = (WaferInfomation?.DiePitch.Height > 0) ? WaferInfomation.DiePitch.Height : 1.0;

            int numDiesX = maxXIdx - minXIdx + 1;
            int numDiesY = maxYIdx - minYIdx + 1;

            double totalWaferWidth = numDiesX * diePitchWidth;
            double totalWaferHeight = numDiesY * diePitchHeight;

            // 가상 캔버스 크기를 1000x1000으로 설정하고, 여기에 맞게 축척 계산
            double virtualCanvasSize = 1000.0;
            double scaleX = virtualCanvasSize / totalWaferWidth;
            double scaleY = virtualCanvasSize / totalWaferHeight;
            double scale = Math.Min(scaleX, scaleY); // 가로세로 비율 유지를 위해 더 작은 축척 사용

            double displayDieWidth = diePitchWidth * scale;
            double displayDieHeight = diePitchHeight * scale;

            foreach (var dieInfo in dieInfos)
            {
                var dieVM = new DieViewModel(dieInfo)
                {
                    Width = displayDieWidth,
                    Height = displayDieHeight,
                    X = (dieInfo.XIndex - minXIdx) * displayDieWidth,
                    Y = (maxYIdx - dieInfo.YIndex) * displayDieHeight
                };
                Dies.Add(dieVM);
            }

            WaferMapWidth = numDiesX * displayDieWidth;
            WaferMapHeight = numDiesY * displayDieHeight;

            OnPropertyChanged(nameof(Dies));
            OnPropertyChanged(nameof(WaferMapWidth));
            OnPropertyChanged(nameof(WaferMapHeight));
        }

        private void UpdateDieSelection()
        {
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