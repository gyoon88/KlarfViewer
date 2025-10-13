using System.Collections.Generic;

namespace KlarfViewer.Model
{
    public class DieRenderInfo
    {
        public DieInfo OriginalDie { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class WaferMapRender
    {
        public List<DieRenderInfo> DieRenders { get; set; }
        public double WaferMapWidth { get; set; }
        public double WaferMapHeight { get; set; }
        public WaferInfo WaferInfo { get; set; }
    }
}
