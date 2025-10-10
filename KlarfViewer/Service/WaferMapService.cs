using KlarfViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KlarfViewer.Service
{
    public class WaferMapService
    {
        // Klarf 데이터를 받아 웨이퍼맵 레이아웃 정보를 계산하는 메서드
        public WaferMapLayout GenerateLayout(KlarfData klarfData)
        {
            var dieInfos = klarfData?.Dies;

            // 데이터가 없으면 기본 원형 웨이퍼 데이터를 생성
            if (dieInfos == null || !dieInfos.Any())
            {
                dieInfos = CreateDefaultDieInfos();
            }

            int minX = dieInfos.Min(d => d.XIndex);
            int minY = dieInfos.Min(d => d.YIndex);
            int maxX = dieInfos.Max(d => d.XIndex);
            int maxY = dieInfos.Max(d => d.YIndex);

            // Die의 상대적 위치를 (0,0) 부터 시작하도록 조정
            var normalizedDies = dieInfos.Select(d => new DieInfo
            {
                XIndex = d.XIndex - minX,
                YIndex = d.YIndex - minY,
                IsDefective = d.IsDefective
                // 필요한 다른 속성들도 복사
            }).ToList();

            return new WaferMapLayout
            {
                Dies = normalizedDies,
                Columns = maxX - minX + 1,
                Rows = maxY - minY + 1
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
