using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlarfViewer.Model
{
    // 불량(Defect) 하나에 대한 모든 정보를 담는 클래스
    public class DefectInfo
    {
        // DefectRecordSpec에 정의된 주요 정보들
        public int Id { get; set; }            // DEFECTID
        public double XRel { get; set; }       // XREL (Die 내 상대 X좌표)
        public double YRel { get; set; }       // YREL (Die 내 상대 Y좌표)
        public int XIndex { get; set; }        // XINDEX (Die의 X 인덱스)
        public int YIndex { get; set; }        // YINDEX (Die의 Y 인덱스)
        public double XSize { get; set; }      // XSIZE
        public double YSize { get; set; }      // YSIZE
        public int ClassNumber { get; set; }   // CLASSNUMBER
        public int ImageCount { get; set; }    // IMAGECOUNT
        public int ImageId { get; set; }       // IMAGELIST의 첫 번째 값 (대표 이미지 ID)

        // 필요에 따라 DefectRecordSpec의 모든 17개 항목을 속성으로 추가할 수 있습니다.
    }
}
