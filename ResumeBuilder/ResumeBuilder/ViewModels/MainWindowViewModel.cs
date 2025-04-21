using System;
using System.Windows;
using System.Windows.Input;
using ResumeBuilder.Infrastructure;

namespace ResumeBuilder.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;

        public MainWindowViewModel()
        {
            _navigationStore = new NavigationStore();
            _navigationStore.CurrentViewModel = new DataSelectorViewModel(_navigationStore);
            _navigationStore.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(NavigationStore.CurrentViewModel))
                    OnPropertyChanged(nameof(CurrentViewModel));
            };

            CloseCommand = new RelayCommand(_ => Application.Current.Shutdown());
        }

        public object CurrentViewModel => _navigationStore.CurrentViewModel;

        public ICommand CloseCommand { get; }
    }
}
