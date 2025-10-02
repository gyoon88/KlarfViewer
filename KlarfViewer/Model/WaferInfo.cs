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

        // from LotID
        public string LotID { get; set; }

        // from Slot
        public int Slot { get; set; }

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
}
