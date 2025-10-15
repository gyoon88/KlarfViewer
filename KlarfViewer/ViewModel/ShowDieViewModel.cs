using KlarfViewer.Model;
using System;
using System.Windows.Input;

namespace KlarfViewer.ViewModel
{
    public class ShowDieViewModel : BaseViewModel
    {
        private readonly DieInfo dieCursor;

        private double x;
        public double X
        {
            get => x;
            set => SetProperty(ref x, value);
        }

        private double y;
        public double Y
        {
            get => y;
            set => SetProperty(ref y, value);
        }

        private double width;
        public double Width
        {
            get => width;
            set => SetProperty(ref width, value);
        }

        private double height;
        public double Height
        {
            get => height;
            set => SetProperty(ref height, value);
        }

        public int XIndex => dieCursor.XIndex;
        public int YIndex => dieCursor.YIndex;
        public DieInfo OriginalDie => dieCursor;
        public bool IsDefective => dieCursor.IsDefective;

        public bool IsSelected
        {
            get => dieCursor.IsSelected;
            set
            {
                if (dieCursor.IsSelected != value)
                {
                    dieCursor.IsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        // Workaround for command binding
        // wafermapviewer에서 확인하는 ...
        public Action<DieInfo> DieClickedAction { get; set; }
        public ICommand ClickCommand { get; }

        public ShowDieViewModel(DieInfo die)
        {
            dieCursor = die;
            ClickCommand = new RelayCommand(() => DieClickedAction?.Invoke(OriginalDie), () => IsDefective);
        }
    }
}