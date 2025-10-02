using KlarfViewer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlarfViewer.ViewModel
{
    public class DefectListViewModel:BaseViewModel
    {
        public ObservableCollection<DefectInfo>? DefectSpec { get; set; }
        public DefectListViewModel()
        {

        }
    }
}
