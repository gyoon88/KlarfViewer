using KlarfViewer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlarfViewer.ViewModel
{
    public class DefectListViewModel : BaseViewModel
    {
        public ObservableCollection<DefectInfo> DefectSpec { get; private set; }

        private DefectInfo _selectedDefect;
        public DefectInfo SelectedDefect
        {
            get => _selectedDefect;
            set
            {
                _selectedDefect = value;
                OnPropertyChanged(nameof(SelectedDefect));
            }
        }

        public DefectListViewModel(List<DefectInfo> defects = null)
        {
            DefectSpec = new ObservableCollection<DefectInfo>(defects ?? new List<DefectInfo>());
        }

        public void UpdateDefects(KlarfData klarfData)
        {
            DefectSpec = new ObservableCollection<DefectInfo>(klarfData.Defects);
            OnPropertyChanged(nameof(DefectSpec));
        }
    }
}
