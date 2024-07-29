namespace SkillCraft.Constants;

internal static class Schemes
{
  public const string Bearer = nameof(Bearer);
  public const string Session = nameof(Session);

  public static string[] GetEnabled(IConfiguration configuration)
  {
    List<string> schemes = new(capacity: 4)
    {
      //ApiKey, // TODO(fpion): ApiKey
      Bearer,
      Session
    };

    //if (configuration.GetValue<bool>("EnableBasicAuthentication"))
    //{
    //  schemes.Add(Basic);
    //} // TODO(fpion): Basic

    return [.. schemes];
  }
}
