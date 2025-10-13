using KlarfViewer.Model;

namespace KlarfViewer.ViewModel
{
    public class DieViewModel : BaseViewModel
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


        // readonly ?~?
        public int XIndex => dieCursor.XIndex;
        public int YIndex => dieCursor.YIndex;
        
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

        public DieViewModel(DieInfo die)
        {
            dieCursor = die;
        }

    }
}
