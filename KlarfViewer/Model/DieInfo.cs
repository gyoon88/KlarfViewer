using System.Collections.Generic;
using System.Windows.Media.Effects;

namespace KlarfViewer.Model
{
    public class DieInfo
    {

        public int XIndex { get; set; }

        public int YIndex { get; set; }

        public bool IsDefective { get; set; }

        public bool IsSelected { get; set; }
        
        public int DefectCount { get; set; }
        public DieInfo()
        {
            
            IsDefective = false;
            IsSelected = false;
            DefectCount = 0;
        }
    }
}
