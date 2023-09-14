namespace SSO.Application.Common.Settings
{
    public class CDNSettingConfigurationModel
    {
        public const string NAME = "CDNSetting";
        public string UploadBaseUrl { get; set; }
        public string DownloadBaseUrl { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Tag { get; set; }
        public string SecretKey { get; set; }
    }
}
