using KlarfViewer.Model;
using KlarfViewer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KlarfViewer.Service
{
    public class WaferMapService
    {
        public List<ShowDieViewModel> CalculateDieViewModels(IEnumerable<DieInfo> dieInfos, WaferInfo waferInfo, double canvasWidth, double canvasHeight, Action<DieInfo> dieClickAction)
        {
            if (dieInfos == null || !dieInfos.Any() || waferInfo == null)
            {
                return new List<ShowDieViewModel>(); // Return empty list
            }

            // Calculate Wafer range
            int minXIdx = dieInfos.Min(d => d.XIndex);
            int minYIdx = dieInfos.Min(d => d.YIndex);
            int maxXIdx = dieInfos.Max(d => d.XIndex);
            int maxYIdx = dieInfos.Max(d => d.YIndex);

            int numDiesX = maxXIdx - minXIdx + 1;
            int numDiesY = maxYIdx - minYIdx + 1;

            double displayDieWidth = (numDiesX > 0) ? canvasWidth / numDiesX : 0;
            double displayDieHeight = (numDiesY > 0) ? canvasHeight / numDiesY : 0;

            var dieViewModels = new List<ShowDieViewModel>();
            foreach (var dieInfo in dieInfos)
            {
                var dieVM = new ShowDieViewModel(dieInfo)
                {
                    Width = displayDieWidth,
                    Height = displayDieHeight,
                    X = (dieInfo.XIndex - minXIdx) * displayDieWidth,
                    Y = (maxYIdx - dieInfo.YIndex) * displayDieHeight,
                    DieClickedAction = dieClickAction
                };
                dieViewModels.Add(dieVM);
            }

            return dieViewModels;
        }
    }
}
