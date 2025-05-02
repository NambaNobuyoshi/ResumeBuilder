using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ResumeBuilder.Infrastructure;
using ResumeBuilder.Models;

namespace ResumeBuilder.ViewModels
{
    public class EntryHomeViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly ResumeEntry _entry;
        private string _selectedGenre;
        private string _editableNote;

        public EntryHomeViewModel() { } // デザイナ用 ctor
        public EntryHomeViewModel(NavigationStore nav, ResumeEntry? entry)
        {
            _navigationStore = nav;
            _entry = entry ?? new ResumeEntry      // ← 新規テンプレ
            {
                Name = "新規経歴書",
                FileName = $"resume_{Guid.NewGuid():N}.json",
                LastModified = DateTime.Now
            };

            _editableName = _entry.Name;
            _selectedGenre = _entry.Tag;
            _editableNote = _entry.Note;

            GenreOptions = new ObservableCollection<string> { "エンジニア", "PM", "営業" };

            SaveMetaCommand = new RelayCommand(
                _ => SaveMeta(),
                _ => CanSaveMeta());
            EditProfileCommand = new RelayCommand(_ => NavigateToProfile());
            EditCareerCommand = new RelayCommand(_ => NavigateToCareer());
            PreviewCommand = new RelayCommand(
                _ => NavigateToPreview(),
                _ => CanPreview());
            BackCommand = new RelayCommand(_ => NavigateBack());
            SaveNameCommand = new RelayCommand(
                _ => SaveName(),
                _ => !string.IsNullOrWhiteSpace(EditableName) && EditableName != _entry.Name);
        }

        // 職務経歴書名称
        private string _editableName;
        public string EditableName
        {
            get => _editableName;
            set
            {
                if (SetProperty(ref _editableName, value))
                    CommandManager.InvalidateRequerySuggested(); // Save ボタン活性/非活性を更新
            }
        }

        // プロフィール
        public string Name => _entry.Profile.Name;
        public string Email => _entry.Profile.Email;
        public string Summary => _entry.Profile.Summary;

        public ObservableCollection<string> GenreOptions { get; }

        public string SelectedGenre
        {
            get => _selectedGenre;
            set { if (SetProperty(ref _selectedGenre, value)) InvalidateSaveMeta(); }
        }

        public string EditableNote
        {
            get => _editableNote;
            set { if (SetProperty(ref _editableNote, value)) InvalidateSaveMeta(); }
        }

        // リスト表示用
        public ObservableCollection<CareerItem> Careers => _entry.Careers;
        public ObservableCollection<QualificationItem> Qualifications => _entry.Qualifications;
        public ObservableCollection<SkillItem> Skills => _entry.Skills;

        public ICommand SaveNameCommand { get; }
        public ICommand SaveMetaCommand { get; }
        public ICommand EditProfileCommand { get; }
        public ICommand EditCareerCommand { get; }
        public ICommand PreviewCommand { get; }
        public ICommand BackCommand { get; }

        private void SaveName()
        {
            _entry.Name = EditableName;
            _entry.LastModified = DateTime.Now;
            try
            {
                _entry.SaveToFile(AppSettings.ResumeDataDirectory);
                // 画面側のタイトルなどを更新したい場合
                OnPropertyChanged(nameof(Name));
            }
            catch (IOException ex)
            {
                MessageBox.Show($"名称の保存に失敗しました: {ex.Message}",
                                "保存エラー",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
        private bool CanSaveMeta() => _selectedGenre != _entry.Tag || _editableNote != _entry.Note;

        private void InvalidateSaveMeta() => CommandManager.InvalidateRequerySuggested();

        private void SaveMeta()
        {
            _entry.Tag = _selectedGenre;
            _entry.Note = _editableNote;
            _entry.LastModified = DateTime.Now;

            try
            {
                _entry.SaveToFile(AppSettings.ResumeDataDirectory);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"保存に失敗しました: {ex.Message}",
                                "保存エラー",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
        private void NavigateToProfile()
        {
            // プロフィール編集画面へ遷移
            _navigationStore.CurrentViewModel = new ProfileEditViewModel(_navigationStore, _entry);
        }
        private void NavigateToCareer()
        {
            // 職歴・資格編集画面へ遷移
            _navigationStore.CurrentViewModel = new CareerEditViewModel(_navigationStore, _entry);
        }
        private void NavigateToPreview()
        {
            // TODO: プレビュー画面へ遷移
            //_navigationStore.CurrentViewModel = new PreviewViewModel(_navigationStore, _entry);
        }

        private bool CanPreview()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Summary) &&
                   (Careers.Count > 0 || Qualifications.Count > 0 || Skills.Count > 0);
        }

        private void NavigateBack()
        {
            _navigationStore.CurrentViewModel = new DataSelectorViewModel(_navigationStore);
        }
    }
}