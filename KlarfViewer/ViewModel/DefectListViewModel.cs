using KlarfViewer.Model;
using KlarfViewer.Command;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace KlarfViewer.ViewModel
{
    public class DefectListViewModel : BaseViewModel
    {
        private KlarfData? klarfInfomation;

        public KlarfData KlarfInfomation
        {
            get => klarfInfomation;
            set => SetProperty(ref klarfInfomation, value);
        }
        public ObservableCollection<DefectInfo> Defects { get; private set; }

        private DefectInfo? selectedDefect;
        public DefectInfo SelectedDefect
        {
            get => selectedDefect;
            set
            {
                if (SetProperty(ref selectedDefect, value))
                {
                    UpdateDefectIndices();
                }
            }
        }

        // DefectID in Defects 
        private int currentDefectIndex;
        public int CurrentDefectIndex
        {
            get => currentDefectIndex;
            set => SetProperty(ref currentDefectIndex, value);
        }

        private int totalDefectCount;
        public int TotalDefectCount
        {
            get => totalDefectCount;
            set => SetProperty(ref totalDefectCount, value);
        }
        

        // Inner Defect of Die
        private int currentDefectIndexInDie;
        public int CurrentDefectIndexInDie
        {
            get => currentDefectIndexInDie;
            set => SetProperty(ref currentDefectIndexInDie, value);
        }

        private int totalDefectCountInDie;
        public int TotalDefectCountInDie
        {
            get => totalDefectCountInDie;
            set => SetProperty(ref totalDefectCountInDie, value);
        }

        // Total Die
        private int currentDieIndex;
        public int CurrentDieIndex
        {
            get => currentDieIndex;
            set => SetProperty(ref currentDieIndex, value);
        }

        private int totalDieCount;
        public int TotalDieCount
        {
            get => totalDieCount;
            set => SetProperty(ref totalDieCount, value);
        }

        public DefectNavigationCommands Commands { get; }

        // Constructor
        public DefectListViewModel()
        {
            Defects = new ObservableCollection<DefectInfo>();
            Commands = new DefectNavigationCommands(this);
        }


        // Create Data Grid from MainViewModel After 
        public void LoadData(KlarfData klarfData)
        {
            KlarfInfomation = klarfData;
            
            Defects.Clear();

            if (klarfInfomation?.Defects != null)
            {
                // Create Defect ObserveCollection 
                foreach (var defect in klarfInfomation.Defects)
                {
                    Defects.Add(defect);
                }
            }
            TotalDefectCount = Defects.Count;
            if (klarfInfomation == null) return;
            TotalDieCount = klarfInfomation.Wafer.TotalDies;
            OnPropertyChanged(nameof(Defects)); // defect list 

            // set default using LINQ 
            SelectedDefect = Defects.FirstOrDefault();
        }

        // wafer Map viewer Evant 
        public void SelectDefectAt(int xIndex, int yIndex)
        {
            if (Defects == null) return;
            SelectedDefect = Defects.FirstOrDefault(d => d.XIndex == xIndex && d.YIndex == yIndex) ?? SelectedDefect;
            return;
        }

        private void UpdateDefectIndices()
        {
            if (SelectedDefect != null && Defects.Contains(SelectedDefect))
            {
                CurrentDefectIndex = Defects.IndexOf(SelectedDefect) + 1;
                CurrentDefectIndexInDie = SelectedDefect.DefectIdInDie;
                if (selectedDefect == null) return;

                DieInfo? die = KlarfInfomation.Dies.FirstOrDefault(d => d.XIndex == selectedDefect.XIndex && d.YIndex == selectedDefect.YIndex);
                if (die == null) return;
                
                CurrentDieIndex = die.DieID;
                TotalDefectCountInDie = SelectedDefect.TotalDefectsInDie;                
            }
            else
            {
                CurrentDefectIndex = 0;
            }
        }
    }
}