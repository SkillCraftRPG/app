namespace SkillCraft.Tools.ETL.Settings;

internal record ApiSettings
{
  public const string SectionKey = "Api";

  public string BaseUrl { get; set; } = string.Empty;
  public Uri BaseUri => new(BaseUrl, UriKind.Absolute);

  public BasicCredentials Basic { get; set; } = new();
}
