using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using ResumeBuilder.Infrastructure;
using ResumeBuilder.Models;

namespace ResumeBuilder.ViewModels
{
    public class DataSelectorViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        
        public DataSelectorViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;

            // データディレクトリの取得と作成
            string dataDir = AppSettings.ResumeDataDirectory;
            Directory.CreateDirectory(dataDir);

            // 既存のファイルを読み込み
            Entries = new ObservableCollection<ResumeEntry>(
                Directory.EnumerateFiles(dataDir, "*.json")
                         .Select(path => ResumeEntry.FromFile(path))
                         .Where(entry => entry != null));

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
            _navigationStore.CurrentViewModel = new EntryHomeViewModel(_navigationStore, null);
        }
        private void Open()
        {
            _navigationStore.CurrentViewModel = new EntryHomeViewModel(_navigationStore, SelectedEntry);
        }
        private void Delete()
        {
            if (SelectedEntry == null) return;

            // 確認ダイアログ
            var result = MessageBox.Show(
                $"「{SelectedEntry.Name}」を削除してもよろしいですか？\nこの操作は元に戻せません。",
                "削除の確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                string fullPath = SelectedEntry.GetFullPath(AppSettings.ResumeDataDirectory);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                Entries.Remove(SelectedEntry);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"削除に失敗しました: {ex.Message}", "削除エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}