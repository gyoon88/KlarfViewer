using KlarfViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KlarfViewer.Service
{
    public class WaferMapService
    {
        public WaferMapRender CalculateWaferMapRender(IEnumerable<DieInfo> dieInfos, WaferInfo waferInfo, double canvasWidth, double canvasHeight)
        {
            if (dieInfos == null || !dieInfos.Any())
            {
                waferInfo = new WaferInfo { DiePitch = new DieSize { Width = 20, Height = 20 } };
                dieInfos = CreateDefaultDieInfos();
            }

            int minXIdx = dieInfos.Min(d => d.XIndex);
            int minYIdx = dieInfos.Min(d => d.YIndex);
            int maxXIdx = dieInfos.Max(d => d.XIndex);
            int maxYIdx = dieInfos.Max(d => d.YIndex);

            double diePitchWidth = (waferInfo?.DiePitch.Width > 0) ? waferInfo.DiePitch.Width : 1.0;
            double diePitchHeight = (waferInfo?.DiePitch.Height > 0) ? waferInfo.DiePitch.Height : 1.0;

            int numDiesX = maxXIdx - minXIdx + 1;
            int numDiesY = maxYIdx - minYIdx + 1;

            double totalWaferWidth = numDiesX * diePitchWidth;
            double totalWaferHeight = numDiesY * diePitchHeight;

            double scaleX = totalWaferWidth > 0 ? canvasWidth / totalWaferWidth : 0;
            double scaleY = totalWaferHeight > 0 ? canvasHeight / totalWaferHeight : 0;

            double displayDieWidth = diePitchWidth * scaleX;
            double displayDieHeight = diePitchHeight * scaleY;

            var dieRenders = new List<DieRenderInfo>();
            foreach (var dieInfo in dieInfos)
            {
                dieRenders.Add(new DieRenderInfo
                {
                    OriginalDie = dieInfo,
                    Width = displayDieWidth,
                    Height = displayDieHeight,
                    X = (dieInfo.XIndex - minXIdx) * displayDieWidth,
                    Y = (maxYIdx - dieInfo.YIndex) * displayDieHeight
                });
            }

            return new WaferMapRender
            {
                DieRenders = dieRenders,
                WaferMapWidth = canvasWidth,
                WaferMapHeight = canvasHeight,
                WaferInfo = waferInfo
            };
        }

        private List<DieInfo> CreateDefaultDieInfos()
        {
            var defaultDies = new List<DieInfo>();
            for (int y = -15; y <= 15; y++)
            {
                for (int x = -15; x <= 15; x++)
                {
                    if (x * x + y * y < 15 * 15) // 원형 필터
                    {
                        defaultDies.Add(new DieInfo { XIndex = x, YIndex = y });
                    }
                }
            }
            return defaultDies;
        }
    }
}
