using KlarfViewer.Model;
using KlarfViewer.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KlarfViewer.ViewModel
{
    public class WaferMapViewModel : BaseViewModel
    {
        private readonly WaferMapService _waferMapService;

        // Shared data model (by reference)
        private KlarfData _klarfData;
        private double _currentWidth;
        private double _currentHeight;

        public ObservableCollection<ShowDieViewModel> Dies { get; private set; }
        public Action<DieInfo> DieClicked { get; set; }

        public WaferMapViewModel()
        {
            Dies = new ObservableCollection<ShowDieViewModel>();
            _waferMapService = new WaferMapService();
        }

        // 1. Load data reference once
        public void LoadData(KlarfData klarfData)
        {
            _klarfData = klarfData;
            Render();
        }

        // 2. Update size and re-render using stored data
        public void UpdateMapSize(double newWidth, double newHeight)
        {
            _currentWidth = newWidth;
            _currentHeight = newHeight;
            Render();
        }

        // 3. Highlight a die based on index from MainViewModel
        public void HighlightDieAt(int xIndex, int yIndex)
        {
            if (Dies == null) return;
            foreach (var dieVM in Dies)
            {
                dieVM.IsSelected = (dieVM.XIndex == xIndex && dieVM.YIndex == yIndex);
            }
        }

        private void Render()
        {
            Dies.Clear();
            if (_klarfData == null || _currentWidth <= 0 || _currentHeight <= 0)
            {
                OnPropertyChanged(nameof(Dies));
                return;
            }

            var dieViewModels = _waferMapService.CalculateDieViewModels(_klarfData.Dies, _klarfData.Wafer, _currentWidth, _currentHeight, (dieInfo) => DieClicked?.Invoke(dieInfo));

            foreach (var dieVM in dieViewModels)
            {
                Dies.Add(dieVM);
            }

            OnPropertyChanged(nameof(Dies));
        }
    }
}