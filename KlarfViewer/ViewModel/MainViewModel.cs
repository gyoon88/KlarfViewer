using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlarfViewer.ViewModel
{
    public class MainViewModel:BaseViewModel
    {
        
        public WaferMapViewModel? WaferMapVM { get; private set; }
        public DefectImageViewModel DefectImageVM { get; private set; }
        public FileListViewModel FileViewerVM { get; private set; }
        public DefectListViewModel DefectListVM { get; private set; }
        public MainViewModel() 
        {
            WaferMapVM = new WaferMapViewModel();
            DefectImageVM = new DefectImageViewModel();
            FileViewerVM = new FileListViewModel();
            DefectListVM = new DefectListViewModel();
        }
    }
}
