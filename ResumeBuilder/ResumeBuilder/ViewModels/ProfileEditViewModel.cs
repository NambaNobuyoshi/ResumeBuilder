using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text;
using System.Windows;
using System;
using System.Windows.Input;
using ResumeBuilder.Infrastructure;
using System.IO;
using ResumeBuilder.Models;

namespace ResumeBuilder.ViewModels
{
    public class ProfileEditViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly ResumeEntry _entry;

        public ProfileEditViewModel() { } // デザイナ用 ctor
        public ProfileEditViewModel(NavigationStore navigationStore, ResumeEntry entry)
        {
            _navigationStore = navigationStore;
            _entry = entry;

            // 初期値を設定
            EditedName = _entry.Profile.Name;
            EditedEmail = _entry.Profile.Email;
            EditedSummary = _entry.Profile.Summary;

            SaveCommand = new RelayCommand(_ => Save());
            BackCommand = new RelayCommand(_ => NavigateBack());
        }

        // 表示用
        public string OriginalName => _entry.Profile.Name;
        public string OriginalEmail => _entry.Profile.Email;
        public string OriginalSummary => _entry.Profile.Summary;

        // 編集用プロパティ
        public string EditedName { get; set; }
        public string EditedEmail { get; set; }
        public string EditedSummary { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }

        private void Save()
        {
            // ① バックアップ
            string oldName = _entry.Profile.Name;
            string oldEmail = _entry.Profile.Email;
            string oldSummary = _entry.Profile.Summary;
            DateTime oldTime = _entry.LastModified;

            // ② エントリを更新
            _entry.Profile.Name = EditedName;
            _entry.Profile.Email = EditedEmail;
            _entry.Profile.Summary = EditedSummary;
            _entry.LastModified = DateTime.Now;

            try
            {
                // ③ JSONファイルを保存（共通設定のパスを使用）
                string dataDirectory = AppSettings.ResumeDataDirectory; // ← 共通設定として定義されている想定
                _entry.SaveToFile(dataDirectory);

                // ④ 保存成功後、ホーム画面に戻る
                _navigationStore.CurrentViewModel = new EntryHomeViewModel(_navigationStore, _entry);
            }
            catch (Exception ex)
            {
                // ⑤ 保存失敗時はロールバック
                _entry.Profile.Name = oldName;
                _entry.Profile.Email = oldEmail;
                _entry.Profile.Summary = oldSummary;
                _entry.LastModified = oldTime;

                MessageBox.Show($"保存に失敗しました: {ex.Message}",
                                "保存エラー",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }



        private void NavigateBack()
        {
            _navigationStore.CurrentViewModel = new EntryHomeViewModel(_navigationStore, _entry);
        }
    }
}