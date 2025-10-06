using System.ComponentModel;

namespace CSS_MagacinControl_App.ViewModels
{
    public class IdentiViewModel : INotifyPropertyChanged
    {
        // immutable/rarely-changed props can stay auto if you never change them
        public string SifraIdenta { get; set; }
        public string NazivIdenta { get; set; }
        public string Barkod { get; set; }
        public string BrojFakture { get; set; }
        public int Oznaka { get; set; }
        public string OznakaUsluge { get; set; }

        private decimal _kolicinaSaFakture;
        public decimal KolicinaSaFakture
        {
            get => _kolicinaSaFakture;
            set { if (_kolicinaSaFakture != value) { _kolicinaSaFakture = value; OnPropertyChanged(nameof(KolicinaSaFakture)); } }
        }

        private decimal _pripremljenaKolicina;
        public decimal PripremljenaKolicina
        {
            get => _pripremljenaKolicina;
            set
            {
                if (_pripremljenaKolicina != value)
                {
                    _pripremljenaKolicina = value;
                    OnPropertyChanged(nameof(PripremljenaKolicina));
                }
            }
        }

        private decimal _razlika;
        public decimal Razlika
        {
            get => _razlika;
            set { if (_razlika != value) { _razlika = value; OnPropertyChanged(nameof(Razlika)); } }
        }

        private bool _isRecentlyScanned;
        public bool IsRecentlyScanned
        {
            get => _isRecentlyScanned;
            set { if (_isRecentlyScanned != value) { _isRecentlyScanned = value; OnPropertyChanged(nameof(IsRecentlyScanned)); } }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}