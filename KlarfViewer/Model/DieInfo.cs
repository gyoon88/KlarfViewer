using System.Collections.Generic;
using System.Windows.Media.Effects;

namespace KlarfViewer.Model
{
    /// <summary>
    /// The Class that Include Infomation of one of die on wafer
    /// </summary>
    public class DieInfo
    {
        /// <summary>
        /// Die's index of X axis in the wafer grid
        /// </summary>
        public int XIndex { get; set; }

        /// <summary>
        /// Die's index of Y axis in the wafer grid
        /// </summary>
        public int YIndex { get; set; }

        /// <summary>
        /// There is Defect or not in die
        /// Use in Wafer Map Viewer to marking defect die points
        /// </summary>
        public bool IsDefective { get; set; }

        /// <summary>
        /// Check user selected this die?
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Die Instance Constructor
        /// </summary>
        public DieInfo()
        {
            IsDefective = false;
            IsSelected = false;
            
        }
    }
}
