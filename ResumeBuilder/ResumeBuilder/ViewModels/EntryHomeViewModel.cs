using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ResumeBuilder.Infrastructure;
using ResumeBuilder.Models;

namespace ResumeBuilder.ViewModels
{
    public class EntryHomeViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly ResumeEntry _entry;

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

            EditProfileCommand = new RelayCommand(_ => NavigateToProfile());
            EditCareerCommand = new RelayCommand(_ => NavigateToCareer());
            PreviewCommand = new RelayCommand(
                _ => NavigateToPreview(),
                _ => CanPreview());
            BackCommand = new RelayCommand(_ => NavigateBack());
        }

        // プロフィール
        public string Name => _entry.Profile.Name;
        public string Email => _entry.Profile.Email;
        public string Summary => _entry.Profile.Summary;

        // リスト表示用
        public ObservableCollection<CareerItem> Careers => _entry.Careers;
        public ObservableCollection<QualificationItem> Qualifications => _entry.Qualifications;
        public ObservableCollection<SkillItem> Skills => _entry.Skills;

        public ICommand EditProfileCommand { get; }
        public ICommand EditCareerCommand { get; }
        public ICommand PreviewCommand { get; }
        public ICommand BackCommand { get; }

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