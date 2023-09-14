namespace SSO.Application.Common.Settings
{
    public class BearerTokensConfigurationModel
    {
        public const string NAME = "BearerTokens";
        public string Key { get; set; }
        public string Issuer { get; set; }
        public int RefreshTokenExpirationMinutes { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
        public int ClockSkew { get; set; }
    }
}
