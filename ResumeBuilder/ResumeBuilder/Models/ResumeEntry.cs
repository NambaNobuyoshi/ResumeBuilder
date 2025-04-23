using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ResumeBuilder.Models
{
    public class ResumeEntry
    {
        public string Name { get; set; } = string.Empty; // 経歴書の識別名
        public string FileName { get; set; } = string.Empty; // ファイル名のみ（拡張子含む）
        public string Tag { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime LastModified { get; set; } = DateTime.Now;

        // プロフィール・職歴・資格・スキル
        public ProfileData Profile { get; set; } = new();
        public ObservableCollection<CareerItem> Careers { get; set; } = new();
        public ObservableCollection<QualificationItem> Qualifications { get; set; } = new();
        public ObservableCollection<SkillItem> Skills { get; set; } = new();

        // ファイルのフルパスは別途取得する想定
        public string GetFullPath(string baseDirectory)
        {
            return Path.Combine(baseDirectory, FileName);
        }

        // JSONからの読み書き
        public static ResumeEntry FromFile(string path)
        {
            var json = File.ReadAllText(path);
            var entry = JsonSerializer.Deserialize<ResumeEntry>(json);
            if (entry != null)
            {
                entry.FileName = Path.GetFileName(path); // ファイル名だけ保持
            }
            return entry!;
        }

        public void SaveToFile(string baseDirectory)
        {
            string fullPath = GetFullPath(baseDirectory);
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            File.WriteAllText(fullPath, json);
        }
    }

    public class ProfileData
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
    }

    public class CareerItem
    {
        public string Company { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public override string ToString() => $"{Company}（{Period}） - {Description}";
    }

    public class QualificationItem
    {
        public string Name { get; set; } = string.Empty;
        public override string ToString() => Name;
    }

    public class SkillItem
    {
        public string Name { get; set; } = string.Empty;
        public override string ToString() => Name;
    }
}
