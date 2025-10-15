using KlarfViewer.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace KlarfViewer.ViewModel
{
    public class DefectListViewModel : BaseViewModel
    {
        private KlarfData klarfInfomation;

        public KlarfData KlarfInfomation
        {
            get => klarfInfomation;
            set => SetProperty(ref klarfInfomation, value);
        }
        public ObservableCollection<DefectInfo> Defects { get; private set; }

        private DefectInfo selectedDefect;
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


        public ICommand PrevGlobalCommand { get; }
        public ICommand NextGlobalCommand { get; }
        public ICommand PrevInDieCommand { get; }
        public ICommand NextInDieCommand { get; }

        public DefectListViewModel()
        {
            Defects = new ObservableCollection<DefectInfo>();
            PrevGlobalCommand = new RelayCommand(ExecutePrevGlobal, CanExecutePrevGlobal);
            NextGlobalCommand = new RelayCommand(ExecuteNextGlobal, CanExecuteNextGlobal);
            PrevInDieCommand = new RelayCommand(ExecutePrevInDie, CanExecutePrevInDie);
            NextInDieCommand = new RelayCommand(ExecuteNextInDie, CanExecuteNextInDie);
        }


        // Create Data Grid from MainViewModel After 
        public void LoadData(KlarfData klarfData)
        {
            KlarfInfomation = klarfData;
            
            Defects.Clear();

            if (klarfInfomation?.Defects != null)
            {
                foreach (var defect in klarfInfomation.Defects)
                {
                    Defects.Add(defect);
                }
            }
            TotalDefectCount = Defects.Count;
            OnPropertyChanged(nameof(Defects));

            // set default useing by LINQ 
            SelectedDefect = Defects.FirstOrDefault();
        }

        public void SelectDefectAt(int xIndex, int yIndex)
        {
            if (Defects == null) return;
            SelectedDefect = Defects.FirstOrDefault(d => d.XIndex == xIndex && d.YIndex == yIndex) ?? SelectedDefect;
        }

        private void UpdateDefectIndices()
        {
            if (SelectedDefect != null && Defects.Contains(SelectedDefect))
            {
                CurrentDefectIndex = Defects.IndexOf(SelectedDefect) + 1;

                //// Local Index
                //var defectsInDie = GetDefectsInCurrentDie();
                //CurrentDefectIndexInDie = defectsInDie.IndexOf(SelectedDefect) + 1;
                //TotalDefectCountInDie = defectsInDie.Count();

                CurrentDefectIndexInDie = SelectedDefect.DefectIdInDie;
                TotalDefectCountInDie = SelectedDefect.TotalDefectsInDie;
            }
            else
            {
                CurrentDefectIndex = 0;
            }
        }





        // In-Die Navigation
        private List<DefectInfo> GetDefectsInCurrentDie()
        {
            if (SelectedDefect == null) return new List<DefectInfo>();
            return Defects.Where(d => d.XIndex == SelectedDefect.XIndex && d.YIndex == SelectedDefect.YIndex).ToList();
        }

        // Global Defect Navigation
        private void ExecutePrevGlobal()
        {
            int currentIndex = Defects.IndexOf(SelectedDefect);
            SelectedDefect = Defects[currentIndex - 1];

        }
        private void ExecuteNextGlobal()
        {
            int currentIndex = Defects.IndexOf(SelectedDefect);
            SelectedDefect = Defects[currentIndex + 1];

        }

        // Inner Die Defect Navigation
        private void ExecutePrevInDie()
        {                        
            var defectsInDie = GetDefectsInCurrentDie();
            int currentIndexInDie = defectsInDie.IndexOf(SelectedDefect);
            SelectedDefect = defectsInDie[currentIndexInDie - 1];
        }
        private void ExecuteNextInDie()
        {
            var defectsInDie = GetDefectsInCurrentDie();
            int currentIndexInDie = defectsInDie.IndexOf(SelectedDefect);
            SelectedDefect = defectsInDie[currentIndexInDie + 1];
        }

        // Condition of Execute - Global Defect 
        private bool CanExecuteNextGlobal() => Defects.Any() && SelectedDefect != Defects.Last();
        private bool CanExecutePrevGlobal() => Defects.Any() && SelectedDefect != Defects.First();

        // Condition of Execute - Inner Die Defect 
        private bool CanExecutePrevInDie()
        {
            if (SelectedDefect == null) return false;
            var defectsInDie = GetDefectsInCurrentDie();
            return defectsInDie.IndexOf(SelectedDefect) > 0;
        }
        private bool CanExecuteNextInDie()
        {
            if (SelectedDefect == null) return false;
            var defectsInDie = GetDefectsInCurrentDie();
            return defectsInDie.IndexOf(SelectedDefect) < defectsInDie.Count - 1;
        }

    }
}