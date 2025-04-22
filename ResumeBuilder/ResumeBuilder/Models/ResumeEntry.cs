using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace ResumeBuilder
{
    public class ResumeEntry
    {
        public ProfileData Profile { get; set; } = new ProfileData();
        public ObservableCollection<CareerItem> Careers { get; set; } = new();
        public ObservableCollection<QualificationItem> Qualifications { get; set; } = new();
        public ObservableCollection<SkillItem> Skills { get; set; } = new();

        public string Name { get; set; } = "新規データ";
        public DateTime LastModified { get; set; } = DateTime.Now;
        public string Tag { get; set; } = ""; // 職種など

        public string Note { get; set; } = "";

        public string FilePath { get; set; } = string.Empty;

        public static ResumeEntry CreateNew()
        {
            var entry = new ResumeEntry();
            entry.FilePath = Path.Combine("ResumeData", Guid.NewGuid() + ".json");
            entry.Save();
            return entry;
        }

        public static ResumeEntry FromFile(string path)
        {
            var json = File.ReadAllText(path);
            var entry = JsonSerializer.Deserialize<ResumeEntry>(json) ?? new ResumeEntry();
            entry.FilePath = path;
            return entry;
        }

        public void Save()
        {
            LastModified = DateTime.Now;
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }

    public class ProfileData
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Summary { get; set; } = "";
    }

    public class CareerItem
    {
        public string Company { get; set; } = "";
        public string Period { get; set; } = "";
        public string Description { get; set; } = "";

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