using CadastramentoPerformace.Core;

namespace CadastramentoPerformace.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand CadastramentoViewCommand { get; set; }
        public RelayCommand ServicosViewCommand { get; set; }
        public RelayCommand ConsultaViewCommand { get; set; }

        public CadastramentoViewModel CadastramentoVM { get; set; }
        public ServicosViewModel ServicosVM { get; set; }
        public ConsultaViewModel ConsultaVM { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            CadastramentoVM = new CadastramentoViewModel();
            ServicosVM = new ServicosViewModel();
            ConsultaVM = new ConsultaViewModel();

            CurrentView = ServicosVM;

            CadastramentoViewCommand = new RelayCommand(o =>
            {
                CurrentView = CadastramentoVM;
            });

            ServicosViewCommand = new RelayCommand(o =>
            {
                CurrentView = ServicosVM;
            });

            ConsultaViewCommand = new RelayCommand(o =>
            {
                CurrentView = ConsultaVM;
            });

        }
    }
}