using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace AutoDoc.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            // Cria os comandos
            OpenDashboardCommand = new RelayCommand(() => CurrentView = new DashboardViewModel());
            OpenRepositoriesCommand = new RelayCommand(() => CurrentView = new RepositoriesViewModel());
            OpenConfigCommand = new RelayCommand(() => CurrentView = new ConfigViewModel());
            OpenExecutionCommand = new RelayCommand(() => CurrentView = new ExecutionViewModel());

            // Define a tela inicial
            CurrentView = new DashboardViewModel();
        }

        [ObservableProperty]
        private ViewModelBase currentView;

        public ICommand OpenDashboardCommand { get; }

        public ICommand OpenRepositoriesCommand { get; }

        public ICommand OpenConfigCommand { get; }

        public ICommand OpenExecutionCommand { get; }
    }
}
