using KlarfViewer.Model;
using KlarfViewer.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KlarfViewer.ViewModel
{
    public class WaferMapViewModel : BaseViewModel
    {
        private readonly WaferMapService waferMapService;

        // Shared data model (by reference)
        private KlarfData klarfInfomation;
        private double currentWidth;
        private double currentHeight;

        public ObservableCollection<ShowDieViewModel> Dies { get; private set; }
        public Action<DieInfo> DieClicked { get; set; } // 선택된 다이

        public WaferMapViewModel()
        {
            Dies = new ObservableCollection<ShowDieViewModel>();
            waferMapService = new WaferMapService();
        }

        // Load data reference from MainViewModel that give Model data
        public void LoadData(KlarfData klarfData)
        {
            klarfInfomation = klarfData;
            Render();
        }

        // Update size and re-render using stored data
        public void UpdateMapSize(double newWidth, double newHeight)
        {
            currentWidth = newWidth;
            currentHeight = newHeight;
            Render();
        }

        // Highlight a die based on index from MainViewModel
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
            if (klarfInfomation == null || currentWidth <= 0 || currentHeight <= 0)
            {
                OnPropertyChanged(nameof(Dies));
                return;
            }

            var dieViewModels = waferMapService.CalculateDieViewModels(klarfInfomation.Dies, klarfInfomation.Wafer, currentWidth, currentHeight, (dieInfo) => DieClicked?.Invoke(dieInfo));

            foreach (var dieVM in dieViewModels)
            {
                Dies.Add(dieVM);
            }

            OnPropertyChanged(nameof(Dies));
        }
    }
}