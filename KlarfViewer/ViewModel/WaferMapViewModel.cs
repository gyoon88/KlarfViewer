using KlarfViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlarfViewer.ViewModel
{
    public class WaferMapViewModel : BaseViewModel
    {
        public WaferInfo WaferInfomation { get; private set; }
        public List<KlarfViewer.Model.DieInfo> Dies { get; private set; }

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

        public WaferMapViewModel(WaferInfo waferInfo = null, List<KlarfViewer.Model.DieInfo> dies = null)
        {
            WaferInfomation = waferInfo ?? new WaferInfo();
            Dies = dies ?? new List<KlarfViewer.Model.DieInfo>();
        }

        public void UpdateWaferData(KlarfData klarfData)
        {
            WaferInfomation = klarfData.Wafer;
            Dies = klarfData.Dies;
            OnPropertyChanged(nameof(WaferInfomation));
            OnPropertyChanged(nameof(Dies));
        }
    }
}
