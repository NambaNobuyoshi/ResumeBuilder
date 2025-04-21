using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using ResumeBuilder.Infrastructure;

namespace ResumeBuilder.ViewModels
{
    public class DataSelectorViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        
        // exe が置かれているディレクトリを基点にする
        private static readonly string DataDir = Path.Combine(AppContext.BaseDirectory, "ResumeData");

        public DataSelectorViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            Directory.CreateDirectory(DataDir);

            // Load existing entries
            Entries = new ObservableCollection<ResumeEntry>(
                Directory.EnumerateFiles(DataDir, "*.json")
                        .Select(path => ResumeEntry.FromFile(path)));

            NewCommand = new RelayCommand(_ => CreateNew());
            OpenCommand = new RelayCommand(_ => Open(), _ => SelectedEntry != null);
            DeleteCommand = new RelayCommand(_ => Delete(), _ => SelectedEntry != null);
        }

        public ObservableCollection<ResumeEntry> Entries { get; }

        private ResumeEntry? _selectedEntry;
        public ResumeEntry? SelectedEntry
        {
            get => _selectedEntry;
            set => SetProperty(ref _selectedEntry, value);
        }

        // Commands
        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand DeleteCommand { get; }

        private void CreateNew()
        {
            var entry = ResumeEntry.CreateNew();
            // _navigationStore.CurrentViewModel = new EntryHomeViewModel(_navigationStore, entry);
        }
        private void Open()
        {
            // if (SelectedEntry != null) _navigationStore.CurrentViewModel = new EntryHomeViewModel(_navigationStore, SelectedEntry);
        }
        private void Delete()
        {
            if (SelectedEntry == null) return;
            File.Delete(SelectedEntry.FilePath);
            Entries.Remove(SelectedEntry);
        }
    }
}