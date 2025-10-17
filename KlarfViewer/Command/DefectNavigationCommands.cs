using KlarfViewer.ViewModel;
using KlarfViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace KlarfViewer.Command
{
    public class DefectNavigationCommands
    {
        private readonly DefectListViewModel vm;

        public ICommand PrevGlobalCommand { get; }
        public ICommand NextGlobalCommand { get; }
        public ICommand PrevInDieCommand { get; }
        public ICommand NextInDieCommand { get; }
        public ICommand PrevGlobalDieCommand { get; }
        public ICommand NextGlobalDieCommand { get; }

        public DefectNavigationCommands(DefectListViewModel viewModel)
        {
            vm = viewModel;
            //vm.PropertyChanged += (s, e) =>
            //{
            //    (PrevGlobalCommand as RelayCommand)?.RaiseCanExecuteChanged();
            //    (NextGlobalCommand as RelayCommand)?.RaiseCanExecuteChanged();
            //    (PrevInDieCommand as RelayCommand)?.RaiseCanExecuteChanged();
            //    (NextInDieCommand as RelayCommand)?.RaiseCanExecuteChanged();
            //    (PrevGlobalDieCommand as RelayCommand)?.RaiseCanExecuteChanged();
            //    (NextGlobalDieCommand as RelayCommand)?.RaiseCanExecuteChanged();
            //};

            PrevGlobalCommand = new RelayCommand(ExecutePrevGlobal, CanExecutePrevGlobal);
            NextGlobalCommand = new RelayCommand(ExecuteNextGlobal, CanExecuteNextGlobal);
            PrevInDieCommand = new RelayCommand(ExecutePrevInDie, CanExecutePrevInDie);
            NextInDieCommand = new RelayCommand(ExecuteNextInDie, CanExecuteNextInDie);
            PrevGlobalDieCommand = new RelayCommand(ExecutePrevGlobalDie, CanExecutePrevGlobalDie);
            NextGlobalDieCommand = new RelayCommand(ExecuteNextGlobalDie, CanExecuteNextGlobalDie);
        }

        private List<DefectInfo> GetDefectsInCurrentDie()
        {
            if (vm.SelectedDefect == null) return new List<DefectInfo>();
            return vm.Defects.Where(d => d.XIndex == vm.SelectedDefect.XIndex && d.YIndex == vm.SelectedDefect.YIndex).ToList();
        }

        private void ExecutePrevGlobal() => vm.SelectedDefect = vm.Defects[vm.Defects.IndexOf(vm.SelectedDefect) - 1];
        private void ExecuteNextGlobal() => vm.SelectedDefect = vm.Defects[vm.Defects.IndexOf(vm.SelectedDefect) + 1];

        private void ExecutePrevInDie()
        {
            var defectsInDie = GetDefectsInCurrentDie();
            int currentIndexInDie = defectsInDie.IndexOf(vm.SelectedDefect);
            vm.SelectedDefect = defectsInDie[currentIndexInDie - 1];
        }

        private void ExecuteNextInDie()
        {
            var defectsInDie = GetDefectsInCurrentDie();
            int currentIndexInDie = defectsInDie.IndexOf(vm.SelectedDefect);
            vm.SelectedDefect = defectsInDie[currentIndexInDie + 1];
        }

        private void ExecuteNextGlobalDie()
        {
            if (vm.SelectedDefect == null) return;
            var die = vm.KlarfInfomation.Dies.FirstOrDefault(d => d.XIndex == vm.SelectedDefect.XIndex && d.YIndex == vm.SelectedDefect.YIndex);
            if (die == null) return;

            int currentDieID = die.DieID;
            var newDie = vm.KlarfInfomation.Dies.FirstOrDefault(d => d.DieID > currentDieID && d.IsDefective);
            if (newDie != null)
            {
                vm.SelectedDefect = vm.Defects.FirstOrDefault(d => d.XIndex == newDie.XIndex && d.YIndex == newDie.YIndex);
            }
        }

        private void ExecutePrevGlobalDie()
        {
            if (vm.SelectedDefect == null) return;
            var currentDie = vm.KlarfInfomation.Dies.FirstOrDefault(d => d.XIndex == vm.SelectedDefect.XIndex && d.YIndex == vm.SelectedDefect.YIndex);
            if (currentDie == null) return;

            int currentDieID = currentDie.DieID;
            var prevDie = vm.KlarfInfomation.Dies
                .Where(d => d.DieID < currentDieID && d.IsDefective)
                .OrderByDescending(d => d.DieID)
                .FirstOrDefault();
            if (prevDie != null)
            {
                vm.SelectedDefect = vm.Defects.FirstOrDefault(d => d.XIndex == prevDie.XIndex && d.YIndex == prevDie.YIndex);
            }
        }

        private bool CanExecuteNextGlobal() => vm.Defects.Any() && vm.SelectedDefect != vm.Defects.Last();
        private bool CanExecutePrevGlobal() => vm.Defects.Any() && vm.SelectedDefect != vm.Defects.First();

        private bool CanExecutePrevInDie()
        {
            if (vm.SelectedDefect == null) return false;
            var defectsInDie = GetDefectsInCurrentDie();
            return defectsInDie.IndexOf(vm.SelectedDefect) > 0;
        }

        private bool CanExecuteNextInDie()
        {
            if (vm.SelectedDefect == null) return false;
            var defectsInDie = GetDefectsInCurrentDie();
            return defectsInDie.IndexOf(vm.SelectedDefect) < defectsInDie.Count - 1;
        }

        private bool CanExecuteNextGlobalDie()
        {
            if (vm.SelectedDefect == null) return false;
            var die = vm.KlarfInfomation.Dies.FirstOrDefault(d => d.XIndex == vm.SelectedDefect.XIndex && d.YIndex == vm.SelectedDefect.YIndex);
            if (die == null) return false;
            return vm.KlarfInfomation.Dies.Any(d => d.DieID > die.DieID && d.IsDefective);
        }

        private bool CanExecutePrevGlobalDie()
        {
            if (vm.SelectedDefect == null) return false;
            var die = vm.KlarfInfomation.Dies.FirstOrDefault(d => d.XIndex == vm.SelectedDefect.XIndex && d.YIndex == vm.SelectedDefect.YIndex);
            if (die == null) return false;
            return die.DieID > 0 && vm.KlarfInfomation.Dies.Any(d => d.DieID < die.DieID && d.IsDefective);
        }
    }
}
