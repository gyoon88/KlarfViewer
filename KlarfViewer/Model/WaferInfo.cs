using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlarfViewer.Model
{
    public class WaferInfo
    {
        // from WaferID
        public string WaferID { get; set; }
        public string DeviceID { get; set; }

        public SampleCenter SampleCenterLocation { get; set; }
        public int TotalDies { get; set; }
        // from LotID
        public string LotID { get; set; }

        // from Slot
        public string Slot { get; set; }

        // from DiePitch 
        public DieSize DiePitch { get; set; }

        // from FileTimestamp
        public DateTime FileTimestamp { get; set; }

        // from TiffFilename
        public string TiffFilename { get; set; }
    }

    // DiePitch와 같이 두 개의 double 값을 갖는 데이터를 위한 작은 구조체
    public struct DieSize
    {
        public double Width { get; set; }
        public double Height { get; set; }
    }
    public struct SampleCenter
    {
        public double XLoc { get; set; }
        public double YLoc { get; set; }
    }
}
