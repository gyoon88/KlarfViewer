using KlarfViewer.Model;

namespace KlarfViewer.ViewModel
{
    public class DieViewModel : BaseViewModel
    {
        private readonly DieInfo die;

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }


        // readonly ?~?
        public int XIndex => die.XIndex;
        public int YIndex => die.YIndex;
        public bool IsDefective => die.IsDefective;

        public bool IsSelected
        {
            get => die.IsSelected;
            set
            {
                if (die.IsSelected != value)
                {
                    die.IsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public DieViewModel(DieInfo die)
        {
            this.die = die;
        }

    }
}
