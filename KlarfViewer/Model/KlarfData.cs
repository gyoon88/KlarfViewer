using System;
using System.Collections.Generic;
using System.Windows.Media.Effects;


namespace KlarfViewer.Model
{
    public class KlarfData
    {
        // 웨이퍼 정보
        public WaferInfo Wafer { get; set; }

        // 모든 칩(Die) 목록
        public List<DieInfo> Dies { get; set; }

        // 모든 불량(Defect) 목록
        public List<DefectInfo> Defects { get; set; }

        public KlarfData()
        {
            Wafer = new WaferInfo();
            Dies = new List<DieInfo>();
            Defects = new List<DefectInfo>();
        }
    }

}
