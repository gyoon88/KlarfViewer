using System;
using System.Collections.Generic;
using System.Windows.Media.Effects;


namespace KlarfViewer.Model
{   

// Klarf 파일 하나를 대표하는 클래스
    public class KlarfData
    {
        // 파일 헤더 정보
        public string LotID { get; set; }
        public string WaferID { get; set; }
        public string TiffFilename { get; set; }
        public DateTime FileTimestamp { get; set; }

        // Die(칩) 크기 정보 (가로, 세로)
        public DieSize DiePitch { get; set; }

        // 이 파일에 포함된 모든 Die(칩)의 리스트
        // SampleTestPlan을 파싱해서 채웁니다.
        public List<DieData> Dies { get; set; }

        // 이 파일에 포함된 모든 Defect(결함)의 리스트
        // DefectList를 파싱해서 채웁니다.
        public List<DefectInfo> Defects { get; set; }

        public KlarfData()
        {
            Dies = new List<DieData>();
            Defects = new List<DefectInfo>();
        }
    }

    // DiePitch와 같이 두 개의 double 값을 갖는 데이터를 위한 작은 구조체
    public struct DieSize
    {
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
