using System;
using System.IO;

namespace ResumeBuilder.Infrastructure
{
    public static class AppSettings
    {
        /// <summary>
        /// 職務経歴データ保存ディレクトリ
        /// </summary>
        public static string ResumeDataDirectory =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResumeData");
    }
}
