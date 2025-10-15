using System;

namespace KlarfViewer.Model
{
    // Defect의 17개 정보
    public class DefectInfo
    {
        public int Id { get; set; }            // DEFECTID


        public int XIndex { get; set; }        // XINDEX (Die의 X 인덱스)
        public int YIndex { get; set; }        // YINDEX (Die의 Y 인덱스)
        public double XSize { get; set; }      // XSIZE , dieCursor size
        public double YSize { get; set; }      // YSIZE

        public double XRel { get; set; }       // XREL (Die 내 상대 X좌표)
        public double YRel { get; set; }       // YREL (Die 내 상대 Y좌표)
        public double DefectArea { get; set; }    // DEFECTAREA um^2

        public int DSize { get; set; }         // DSIZE diameter
        //public int Test {  get; set; }         // TEST step 
        //public int ClassNumber { get; set; }   // CLASSNUMBER Type of Defect 
        //public int ClusterNumber { get; set; } // Clusternumber Clustered number of wafer


        public int ImageCount { get; set; }    // IMAGECOUNT
        public int ImageList { get; set; }
        public int ImageId { get; set; }       // IMAGELIST의 첫 번째 값 (대표 이미지 ID)

        public int DefectIdInDie { get; set; }
        public int TotalDefectsInDie { get; set; }


        //public int RoughBinNumber {  get; set; } // ROUGH BIN NUMBER
        //public int FineBinNumber { get; set; }
        //public int ReviewSample { get; set; }
    }
}
