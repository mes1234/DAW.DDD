namespace DAW.Repositories.DataAccess;

public class CacheOptions
{
    public const string Name = "Cache";

    public TimeSpan Ttl { get; set; } = TimeSpan.FromSeconds(0);
}