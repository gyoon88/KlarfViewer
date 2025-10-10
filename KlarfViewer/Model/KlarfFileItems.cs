using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlarfViewer.Model
{
    public class KlarfFileItem
    {
        // Connect for checkbox in on ui
        public bool IsSelected { get; set; }

        public string Name { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string FullPath { get; set; }
    }

}
