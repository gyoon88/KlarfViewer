using KlarfViewer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KlarfViewer.ViewModel
{
    public class DefectListViewModel : BaseViewModel
    {
        public ObservableCollection<DefectInfo> DefectSpec { get; private set; }

        private WaferInfo wafer;
        public WaferInfo Wafer
        {
            get => wafer;
            set => SetProperty(ref wafer, value);
        }


        private DefectInfo selectedDefect;
        public DefectInfo SelectedDefect
        {
            get => selectedDefect;
            set
            {
                if (selectedDefect != value)
                {
                    selectedDefect = value;
                    OnPropertyChanged(nameof(SelectedDefect));
                    UpdateDefectIndices();
                }
            }
        }

        private int currentDefectIndex;
              
        public int CurrentDefectIndex
        {
            get => currentDefectIndex;
            set
            {
                currentDefectIndex = value;
                OnPropertyChanged(nameof(CurrentDefectIndex));
            }
        }

        private int totalDefectCount;
        public int TotalDefectCount
        {
            get => totalDefectCount;
            set
            {
                totalDefectCount = value;
                OnPropertyChanged(nameof(TotalDefectCount));
            }
        }

        public ICommand PreviousDefectCommand { get; }
        public ICommand NextDefectCommand { get; }

        public DefectListViewModel(WaferInfo waferInfo, List<DefectInfo> defects = null)
        {
            DefectSpec = new ObservableCollection<DefectInfo>(defects ?? new List<DefectInfo>());
            TotalDefectCount = DefectSpec.Count;
            CurrentDefectIndex = 0;

            // wafer information update
            WaferInfo wafer = waferInfo;


            PreviousDefectCommand = new RelayCommand(ExecutePreviousDefect, CanExecutePreviousDefect);
            NextDefectCommand = new RelayCommand(ExecuteNextDefect, CanExecuteNextDefect);
        }


        private bool CanExecuteNextDefect()
        {
            if (SelectedDefect == null || DefectSpec.Count == 0) return false;
            int currentIndex = DefectSpec.IndexOf(SelectedDefect);
            return currentIndex < DefectSpec.Count - 1;
        }

        private void ExecuteNextDefect()
        {
            if (!CanExecuteNextDefect()) return;
            int currentIndex = DefectSpec.IndexOf(SelectedDefect);
            SelectedDefect = DefectSpec[currentIndex + 1];
        }

        private bool CanExecutePreviousDefect()
        {
            if (SelectedDefect == null || DefectSpec.Count == 0) return false;
            int currentIndex = DefectSpec.IndexOf(SelectedDefect);
            return currentIndex > 0;
        }

        private void ExecutePreviousDefect()
        {
            if (!CanExecutePreviousDefect()) return;
            int currentIndex = DefectSpec.IndexOf(SelectedDefect);
            SelectedDefect = DefectSpec[currentIndex - 1];
        }

        public void UpdateDefects(KlarfData klarfData)
        {
            DefectSpec = new ObservableCollection<DefectInfo>(klarfData.Defects);
            TotalDefectCount = DefectSpec.Count;
            SelectedDefect = DefectSpec.FirstOrDefault();
            OnPropertyChanged(nameof(DefectSpec));
        }

        private void UpdateDefectIndices()
        {
            if (SelectedDefect != null && DefectSpec.Contains(SelectedDefect))
            {
                CurrentDefectIndex = DefectSpec.IndexOf(SelectedDefect) + 1;
            }
            else
            {
                CurrentDefectIndex = 0;
            }
        }
    }
}