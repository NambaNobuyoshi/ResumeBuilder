using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Windows;
using System.Windows.Input;
using ResumeBuilder.Infrastructure;
using ResumeBuilder.Models;
using System;

namespace ResumeBuilder.ViewModels
{
    public class CareerEditViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly ResumeEntry _entry;

        public CareerEditViewModel() { } // デザイナ用 ctor
        public CareerEditViewModel(NavigationStore navigationStore, ResumeEntry entry)
        {
            _navigationStore = navigationStore;
            _entry = entry;

            // バインド用コレクションをモデルとリンク
            Careers = _entry.Careers;
            Qualifications = _entry.Qualifications;
            Skills = _entry.Skills;

            // コマンド
            AddCareerCommand = new RelayCommand(_ => AddCareer(), _ => !string.IsNullOrWhiteSpace(NewCareerText));
            RemoveCareerCommand = new RelayCommand(_ => RemoveCareer(), _ => SelectedCareer != null);
            AddQualificationCommand = new RelayCommand(_ => AddQualification(), _ => !string.IsNullOrWhiteSpace(NewQualificationText));
            RemoveQualificationCommand = new RelayCommand(_ => RemoveQualification(), _ => SelectedQualification != null);
            AddSkillCommand = new RelayCommand(_ => AddSkill(), _ => !string.IsNullOrWhiteSpace(NewSkillText));
            RemoveSkillCommand = new RelayCommand(_ => RemoveSkill(), _ => SelectedSkill != null);
            SaveCommand = new RelayCommand(_ => Save());
            BackCommand = new RelayCommand(_ => NavigateBack());
        }

        // ObservableCollections
        public ObservableCollection<CareerItem> Careers { get; }
        public ObservableCollection<QualificationItem> Qualifications { get; }
        public ObservableCollection<SkillItem> Skills { get; }

        // 選択アイテム
        private CareerItem? _selectedCareer;
        public CareerItem? SelectedCareer
        {
            get => _selectedCareer;
            set
            {
                if (SetProperty(ref _selectedCareer, value))
                    CommandManager.InvalidateRequerySuggested(); // CanExecute を再評価
            }
        }

        private QualificationItem? _selectedQualification;
        public QualificationItem? SelectedQualification
        {
            get => _selectedQualification;
            set
            {
                if (SetProperty(ref _selectedQualification, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        private SkillItem? _selectedSkill;
        public SkillItem? SelectedSkill
        {
            get => _selectedSkill;
            set
            {
                if (SetProperty(ref _selectedSkill, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        // 追加用入力プロパティ
        public string NewCareerText { get; set; } = string.Empty;
        public string NewQualificationText { get; set; } = string.Empty;
        public string NewSkillText { get; set; } = string.Empty;

        // コマンド
        public ICommand AddCareerCommand { get; }
        public ICommand RemoveCareerCommand { get; }
        public ICommand AddQualificationCommand { get; }
        public ICommand RemoveQualificationCommand { get; }
        public ICommand AddSkillCommand { get; }
        public ICommand RemoveSkillCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }

        // 追加削除処理
        private void AddCareer()
        {
            Careers.Add(new CareerItem { Description = NewCareerText });
            NewCareerText = string.Empty; OnPropertyChanged(nameof(NewCareerText));
        }
        private void RemoveCareer() => Careers.Remove(SelectedCareer!);

        private void AddQualification()
        {
            Qualifications.Add(new QualificationItem { Name = NewQualificationText });
            NewQualificationText = string.Empty; OnPropertyChanged(nameof(NewQualificationText));
        }
        private void RemoveQualification() => Qualifications.Remove(SelectedQualification!);

        private void AddSkill()
        {
            Skills.Add(new SkillItem { Name = NewSkillText });
            NewSkillText = string.Empty; OnPropertyChanged(nameof(NewSkillText));
        }
        private void RemoveSkill() => Skills.Remove(SelectedSkill!);

        // 保存処理
        private void Save()
        {
            _entry.LastModified = DateTime.Now;
            try
            {
                _entry.SaveToFile(AppSettings.ResumeDataDirectory);
                _navigationStore.CurrentViewModel = new EntryHomeViewModel(_navigationStore, _entry);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"保存に失敗しました: {ex.Message}", "保存エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateBack()
        {
            _navigationStore.CurrentViewModel = new EntryHomeViewModel(_navigationStore, _entry);
        }
    }
}
