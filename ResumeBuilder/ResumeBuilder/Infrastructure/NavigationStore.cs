using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ResumeBuilder.Infrastructure
{
    public class NavigationStore : INotifyPropertyChanged
    {
        private object? _currentViewModel;
        public object? CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (_currentViewModel == value) return;
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}